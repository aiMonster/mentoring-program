using MentoringProgram.Common.Models.SubscriptionIds;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class PairSubscription
    {
        public Subscription Subscription { get; }
        public Action<TradeUpdate> Callback { get; }

        private PairSubscription(Subscription subscription)
        {
            Subscription = subscription;            
        }

        public PairSubscription(Subscription subscription, Action<TradeUpdate> callback)
        {
            Subscription = subscription;
            Callback = callback;
        }
               
        public static implicit operator PairSubscription(PairSubscriptionGuid pairSubscription) => new PairSubscription(pairSubscription.Id);
    }
}
