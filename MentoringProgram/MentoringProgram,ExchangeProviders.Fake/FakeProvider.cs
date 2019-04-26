using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MentoringProgram.ExchangeProviders.Fake
{
    public class FakeProvider : IExchangeProvider
    {
        private Dictionary<Subscription, CancellationTokenSource> Subscriptions = new Dictionary<Subscription, CancellationTokenSource>();
        private Timer AutoDisconnectionTimer { get; set; }
        private object Locker { get; set; } = new object();

        #region Public Helpers

        /// <summary>
        /// Disconnect every * time span
        /// To auto disconnecting set to zero
        /// </summary>
        public TimeSpan DisconnectionTimeout { get; private set; } = TimeSpan.Zero;

        public void SetDisconnectionTimeout(TimeSpan timeout)
        {
            DisconnectionTimeout = timeout;

            AutoDisconnectionTimer?.Dispose();

            if (timeout != TimeSpan.Zero)
            {
                var callback = new TimerCallback((obj) => Dispose());
                AutoDisconnectionTimer = new Timer(callback, null, 0, DisconnectionTimeout.Milliseconds);
            }
        }

        public int SubscriptionsCount => Subscriptions.Count;

        public void Disconnect() => Dispose();

        #endregion

        #region IExchangeProvider Members

        public event Action OnDisconnected;

        public Task ConnectAsync() => Task.CompletedTask;        

        public Candle GetCurrentCandlePrice(TradingPair pair) => GetRandomCandle();    
        
        public async Task<ResponseResult<Subscription>> SubscribeAsync(TradingPair pair, Action<TradeUpdate> callback)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;


            var task = Task.Run(async () =>
            {
                while (true)
                {
                    callback?.Invoke(new TradeUpdate(pair, GetRandomCandle()));
                    await Task.Delay(2000);
                }
            }, token);
           


            var id = Guid.NewGuid();
            var subscription = new Subscription(id, async () => await UnsubscribeAsync(id));
            Subscriptions.Add(subscription, cancellationTokenSource);

            //Loading imitation
            await Task.Delay(1000);

            return new ResponseResult<Subscription>(subscription);
        }

        public async Task UnsubscribeAsync(PairSubscriptionGuid pairSubscriptionId)
        {
            lock (Locker)
            {
                if (Subscriptions.Any(s => s.Key.Id == pairSubscriptionId))
                {
                    var subscription = Subscriptions.First(s => s.Key.Id == pairSubscriptionId);
                    subscription.Value.Cancel();
                    Subscriptions.Remove(subscription.Key);
                }
            }

            //Loading imitation
            await Task.Delay(1000);
        }

        public void Dispose()
        {
            foreach (var s in Subscriptions)
            {
                s.Value.Cancel();
            }
            Subscriptions.Clear();

            OnDisconnected?.Invoke();
        }

        #endregion

        #region Private Members

        private Candle GetRandomCandle()
        {
            var rnd = new Random();
            var a = (decimal)rnd.NextDouble();
            var b = (decimal)rnd.NextDouble();
            return new Candle((Price)a, (Price)b);
        }

        #endregion
       
    }
}
