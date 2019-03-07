using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models
{
    public class PairSubscription
    {
        public TradingPair Pair { get; }
        public Action<TradeUpdate> Callback { get; }
        public List<MarketSubscription> MarketSubscriptions { get; } = new List<MarketSubscription>();

        public PairSubscription(TradingPair pair, Action<TradeUpdate> callback)
        {
            Pair = pair;
            Callback = callback;
        }
    }
}
