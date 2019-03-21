using MentoringProgram.Common.Models;
using System;

namespace MentoringProgram.Common.Interfaces
{
    public interface IExchangeProvider : IDisposable
    {
        event Action OnDisconnected;      
        ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback);
        void Unsubscribe(Guid subscriptionId);
        Candle GetCurrentCandlePrice(TradingPair pair);
    }
}
