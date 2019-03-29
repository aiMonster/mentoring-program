using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using System;

namespace MentoringProgram.Common.Interfaces
{
    public interface IExchangeProvider : IDisposable
    {
        event Action OnDisconnected;
        void Connect();
        ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback);
        void Unsubscribe(Guid subscriptionId);
        Candle GetCurrentCandlePrice(TradingPair pair);
    }
}
