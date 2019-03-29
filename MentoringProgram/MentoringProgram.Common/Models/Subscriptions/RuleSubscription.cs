using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class RuleSubscription
    {
        public Subscription Subscription { get; set; }
        public List<MarketSubscription> MarketSubscriptions { get; set; }

        public override int GetHashCode() => Subscription.GetHashCode();
    }

}
