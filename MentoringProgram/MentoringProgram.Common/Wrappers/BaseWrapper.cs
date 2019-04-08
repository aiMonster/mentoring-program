using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using System;

namespace MentoringProgram.Common.Wrappers
{
    public class BaseWrapper : IExchangeProvider
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

        public virtual ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback) => exchangeProvider.Subscribe(pair, callback);

        public virtual void Unsubscribe(Guid subscriptionId) => exchangeProvider.Unsubscribe(subscriptionId);

        public override string ToString() => exchangeProvider.ToString();        
    }
}
