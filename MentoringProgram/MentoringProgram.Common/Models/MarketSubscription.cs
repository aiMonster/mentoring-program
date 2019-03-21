using MentoringProgram.Common.Enums;
using System;

namespace MentoringProgram.Common.Models
{
    public class MarketSubscription : Subscription
    {
        public TradingMarket Market { get; }

        public MarketSubscription(Guid id, TradingMarket market) : base(id, null)
        {
            Market = market;
        }
    }
}
