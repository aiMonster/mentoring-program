using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using System;

namespace MentoringProgram.ExchangeProviders.Bittrex.Models
{
    internal struct Subscriber
    {
        public Guid SubscriptionId { get; }
        public Action<TradeUpdate> Callback { get; }

        private Subscriber(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
            Callback = null;
        }

        public Subscriber(Guid subscriptionId, Action<TradeUpdate> callback)
        {
            SubscriptionId = subscriptionId;
            Callback = callback;
        }

        public override bool Equals(object obj)
        {
            var isValidCast = obj is Subscriber;
            if (!isValidCast)
            {
                throw new InvalidCastException();
            }

            var subscriptionEvent = (Subscriber)obj;
            return SubscriptionId == subscriptionEvent.SubscriptionId;
        }

        public override int GetHashCode()
        {
            return SubscriptionId.GetHashCode();
        }

        public static implicit operator Subscriber(Guid id) => new Subscriber(id);
        public static implicit operator Subscriber(PairSubscriptionGuid pairSubscription) => new Subscriber(pairSubscription.Id);
    }
}
