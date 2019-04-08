using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ClientSubscription
    {
        public Subscription Subscription { get; }
        public Action Callback { get; }

        public ClientSubscription(Subscription subscription, Action callback)
        {
            Subscription = subscription;
            Callback = callback;
        }
    }
}
