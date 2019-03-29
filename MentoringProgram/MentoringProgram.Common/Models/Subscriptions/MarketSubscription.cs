using MentoringProgram.Common.Enums;
using System;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class MarketSubscription : Subscription
    {
        public TradingMarket Market { get; }

        public MarketSubscription(Guid id, TradingMarket market) : base(id, null)
        {
            Market = market;
        }

        public MarketSubscription(Subscription subscription, TradingMarket market) : base(subscription)
        {
            Market = market;
        }
    }
}
