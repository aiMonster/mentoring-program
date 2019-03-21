using CryptoExchange.Net.Sockets;
using System.Collections.Generic;

namespace MentoringProgram.ExchangeProviders.Bitfinex.Models
{
    internal class SubscriptionSubscribers
    {
        public UpdateSubscription ApiSubscription { get; }
        public List<Subscriber> Subscribers { get; } = new List<Subscriber>();

        public SubscriptionSubscribers(UpdateSubscription updateSubscription)
        {
            ApiSubscription = ApiSubscription;
        }
    }
}
