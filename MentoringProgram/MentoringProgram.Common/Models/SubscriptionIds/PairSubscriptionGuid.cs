using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.SubscriptionIds
{
    public class PairSubscriptionGuid
    {
        public Guid Id { get; }

        public PairSubscriptionGuid()
        {
            Id = new Guid();
        }

        private PairSubscriptionGuid(Guid id)
        {
            Id = id;
        }

        public static implicit operator Guid(PairSubscriptionGuid pairSubscription) => pairSubscription.Id;
        public static implicit operator PairSubscriptionGuid(Guid id) => new PairSubscriptionGuid(id);

    }
}
