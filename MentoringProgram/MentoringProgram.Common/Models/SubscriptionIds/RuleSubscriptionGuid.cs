using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.SubscriptionIds
{
    public class RuleSubscriptionGuid
    {
        public Guid Id { get; }

        public RuleSubscriptionGuid()
        {
            Id = new Guid();
        }

        private RuleSubscriptionGuid(Guid id)
        {
            Id = id;
        }

        public static implicit operator Guid(RuleSubscriptionGuid RuleSubscription) => RuleSubscription.Id;
        public static implicit operator RuleSubscriptionGuid(Guid id) => new RuleSubscriptionGuid(id);
    }
}
