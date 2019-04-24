using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Threading.Tasks;

namespace MentoringProgram.Common.Wrappers
{
    public abstract class BaseWrapper : IExchangeProvider
    {
        private readonly IExchangeProvider exchangeProvider;

        protected BaseWrapper(IExchangeProvider provider)
        {
            exchangeProvider = provider;
        }

        public virtual event Action OnDisconnected
        {
            add { exchangeProvider.OnDisconnected += value; }            
            remove { exchangeProvider.OnDisconnected -= value; }
        }

        public virtual void Connect() => exchangeProvider.Connect();
      
        public virtual void Dispose() => exchangeProvider.Dispose();

        public virtual Candle GetCurrentCandlePrice(TradingPair pair) => exchangeProvider.GetCurrentCandlePrice(pair);

        public virtual  Task<ResponseResult<Subscription>> SubscribeAsync(TradingPair pair, Action<TradeUpdate> callback) => exchangeProvider.SubscribeAsync(pair, callback);

        public virtual Task UnsubscribeAsync(PairSubscriptionGuid pairSubscriptionId) => exchangeProvider.UnsubscribeAsync(pairSubscriptionId);

        public override string ToString() => exchangeProvider.ToString();        

    }
}
