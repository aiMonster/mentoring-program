using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ProviderSubscription
    {
        public Subscription Subscription { get; set; }
        public List<PairSubscription> Subsciptions { get; set; } = new List<PairSubscription>();
    }
}
