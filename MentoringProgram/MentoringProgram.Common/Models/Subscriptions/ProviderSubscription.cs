using MentoringProgram.Common.Models.SubscriptionIds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ProviderSubscription
    {
        public Subscription Subscription { get; set; }
        public List<PairSubscription> Subsciptions { get; set; } = new List<PairSubscription>();

        public bool ContainsPairSubscription(PairSubscriptionGuid pairSubscriptionId) => 
            Subsciptions.Any(s => s.Subscription.Id == pairSubscriptionId);

        public PairSubscription GetPairSubscription(PairSubscriptionGuid pairSubscriptionId) =>
            Subsciptions.FirstOrDefault(s => s.Subscription.Id == pairSubscriptionId);
    }
}
