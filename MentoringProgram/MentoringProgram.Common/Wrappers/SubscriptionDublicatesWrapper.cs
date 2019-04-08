using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.Common.Wrappers
{    
    public class SubscriptionDublicatesWrapper : BaseWrapper
    {        
        private Dictionary<TradingPair, ProviderSubscription> PairSubscriptions { get; } = new Dictionary<TradingPair, ProviderSubscription>();

        public SubscriptionDublicatesWrapper(IExchangeProvider provider) : base(provider) { }

        public override ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            if (!PairSubscriptions.ContainsKey(pair))
            {
                var response = base.Subscribe(pair, NotifySubscribers);
                PairSubscriptions.Add(pair, new ProviderSubscription { Subscription = response.Data });
            }
           
            var id = Guid.NewGuid();
            var subscription = new Subscription(id, () => Unsubscribe(id));

            PairSubscriptions[pair].Subsciptions.Add(new PairSubscription(subscription, callback));
            return new ResponseResult<Subscription>(subscription);
        }
                
        public override void Unsubscribe(Guid subscriptionId)
        {
            var subscription = PairSubscriptions.FirstOrDefault(s => s.Value.Subsciptions.Any(ss => ss.Subscription.Id == subscriptionId));
            if(subscription.Value == null)
            {
                return;
            }

            var subscriber = subscription.Value.Subsciptions.FirstOrDefault(s => s.Subscription.Id == subscriptionId);
            subscription.Value.Subsciptions.Remove(subscriber);

            if (!subscription.Value.Subsciptions.Any())
            {
                base.Unsubscribe(subscription.Value.Subscription.Id);
                PairSubscriptions.Remove(subscription.Key);
            }            
        }

        private void NotifySubscribers(TradeUpdate update)
        {
            if (PairSubscriptions.ContainsKey(update.Pair))
            {
                foreach (var subscriber in PairSubscriptions[update.Pair].Subsciptions)
                {
                    subscriber.Callback?.Invoke(update);
                }
            }
        }
    }

    public static class SubscriptionDublicatesWrapperExtension
    {
        public static IExchangeProvider AttachSubscriptionDublicatesWrapper(this IExchangeProvider provider)
        {
            return new SubscriptionDublicatesWrapper(provider);
        }
    }
}
