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

namespace MentoringProgram.Tests.ExchangeProviders
{
    public class TestExchangeProvider : IExchangeProvider
    {
        public event Action OnDisconnected;

        private Dictionary<Subscription, CancellationTokenSource> Subscriptions = new Dictionary<Subscription, CancellationTokenSource>();
      
        public void Connect()
        {
            
        }

        public void Dispose()
        {
            foreach(var s in Subscriptions)
            {
                s.Value.Cancel();
            }
            Subscriptions.Clear();
            OnDisconnected?.Invoke();
        }

        public Candle GetCurrentCandlePrice(TradingPair pair) => GetRandomCandle();
        
        private Candle GetRandomCandle()
        {
            var rnd = new Random();
            var a = (decimal)rnd.NextDouble();
            var b = (decimal)rnd.NextDouble();
            return new Candle((Price)a, (Price)b);
        }

        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            var task = new Task(() => 
            {                
                while (true)
                {           
                    callback?.Invoke(new TradeUpdate(pair, GetRandomCandle()));
                    Task.Delay(2000).Wait();                    
                }                
            }, token);

            task.Start();


            var id = Guid.NewGuid();
            var subscription = new Subscription(id, () => Unsubscribe(id));
            Subscriptions.Add(subscription, cancellationTokenSource);

            return new ResponseResult<Subscription>(subscription);
        }

        public void Unsubscribe(PairSubscriptionGuid pairSubscriptionId)
        {
            if (Subscriptions.Any(s => s.Key.Id == pairSubscriptionId))
            {
                var subscription = Subscriptions.First(s => s.Key.Id == pairSubscriptionId);
                subscription.Value.Cancel();
                Subscriptions.Remove(subscription.Key);
            }
        }

        public int SubscriptionsCount => Subscriptions.Count;

        public void Disconnect() => Dispose();
        
    }
}
