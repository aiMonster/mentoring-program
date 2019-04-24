using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MentoringProgram.Common.Wrappers
{    
    public class SubscriptionDublicatesWrapper : BaseWrapper
    {
        private SemaphoreSlim SemaphoreForSubscribe = new SemaphoreSlim(1, 1);
        private SemaphoreSlim SemaphoreForUnsubscribe = new SemaphoreSlim(1, 1);
        private Dictionary<TradingPair, ProviderSubscription> PairSubscriptions { get; } = new Dictionary<TradingPair, ProviderSubscription>();

        public SubscriptionDublicatesWrapper(IExchangeProvider provider) : base(provider) { }

        public override async Task<ResponseResult<Subscription>> SubscribeAsync(TradingPair pair, Action<TradeUpdate> callback)
        {
            await SemaphoreForSubscribe.WaitAsync();
            try
            {
                if (!PairSubscriptions.ContainsKey(pair))
                {
                    var response = await base.SubscribeAsync(pair, NotifySubscribers);
                    PairSubscriptions.Add(pair, new ProviderSubscription { Subscription = response.Data });
                }
            }
            finally
            {
                SemaphoreForSubscribe.Release();
            }            
           
            var id = Guid.NewGuid();
            var subscription = new Subscription(id, async () => await UnsubscribeAsync(id));

            PairSubscriptions[pair].Subsciptions.Add(new PairSubscription(subscription, callback));
            return new ResponseResult<Subscription>(subscription);
        }
                
        public override async Task UnsubscribeAsync(PairSubscriptionGuid pairSubscriptionId)
        {
            await SemaphoreForUnsubscribe.WaitAsync();

            try
            {
                var subscription = PairSubscriptions.FirstOrDefault(s => s.Value.ContainsPairSubscription(pairSubscriptionId));
                if (subscription.Value == null)
                {
                    return;
                }

                var subscriber = subscription.Value.GetPairSubscription(pairSubscriptionId);
                subscription.Value.Subsciptions.Remove(subscriber);

                if (!subscription.Value.Subsciptions.Any())
                {
                    await base.UnsubscribeAsync(subscription.Value.Subscription.Id);
                    PairSubscriptions.Remove(subscription.Key);
                }
            }
            finally
            {
                SemaphoreForUnsubscribe.Release();
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
