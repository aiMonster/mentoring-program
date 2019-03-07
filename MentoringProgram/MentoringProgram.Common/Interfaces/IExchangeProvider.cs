using MentoringProgram.Common.Models;
using System;

namespace MentoringProgram.Common.Interfaces
{
    public interface IExchangeProvider
    {
        Price GetCurrentPrice(TradingPair pair);
        ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback);
        void Unsubscribe(Subscription subscription);
    }
}
