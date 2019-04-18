using MentoringProgram.Common.Models.SubscriptionIds;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ClientSubscription
    {
        public Subscription Subscription { get; }
        public Action Callback { get; }

        private ClientSubscription(Subscription subscription)
        {
            Subscription = subscription;
        }
        public ClientSubscription(Subscription subscription, Action callback)
        {
            Subscription = subscription;
            Callback = callback;
        }

        public static implicit operator ClientSubscription(RuleSubscriptionGuid ruleSubscriptionId) => new ClientSubscription(ruleSubscriptionId.Id);
    }
}
