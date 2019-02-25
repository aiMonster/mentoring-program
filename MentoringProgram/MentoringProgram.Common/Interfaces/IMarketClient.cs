using MentoringProgram.Common.Models;
using System;
using System.Collections.Generic;

namespace MentoringProgram.Common.Interfaces
{
    public interface IMarketClient : IDisposable
    {
        string Name { get; }
        List<TradingPair> _aviablePairs { get; }
        TradingPair GetPairByName(string name);
        ResponseResult<Guid> SubscribeToTraidingPair(TradingPair pair, Action<TradeSimple> callBack);
        void UnsubscribeTraidingPair(Guid id);
        ResponseResult<Guid> SetUpAlert(TradingRule rule, bool callOnce = true);
        void RemoveAlert(Guid id);
    }
}
