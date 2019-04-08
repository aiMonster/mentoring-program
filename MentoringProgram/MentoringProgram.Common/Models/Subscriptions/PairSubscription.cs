using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class PairSubscription
    {
        public Subscription Subscription { get; }
        public Action<TradeUpdate> Callback { get; }

        public PairSubscription(Subscription subscription, Action<TradeUpdate> callback)
        {
            Subscription = subscription;
            Callback = callback;
        }
    }
}
