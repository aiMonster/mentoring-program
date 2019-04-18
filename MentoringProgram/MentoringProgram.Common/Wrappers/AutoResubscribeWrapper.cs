using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MentoringProgram.Common.Wrappers
{  
    public class AutoResubscribeWrapper : BaseWrapper
    {
        private Dictionary<TradingPair, ResubscribeSubscription> PairSubscriptions { get; } = new Dictionary<TradingPair, ResubscribeSubscription>();

        public AutoResubscribeWrapper(IExchangeProvider provider) : base(provider) { }

        public override void Connect()
        { 
            base.Connect();
            foreach (var subscription in PairSubscriptions)
            {
                var response = base.Subscribe(subscription.Key, subscription.Value.Callback);
                subscription.Value.ProviderSubscription = response.Data;
            }
        }

        public override ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {            
            var response = base.Subscribe(pair, callback);
            PairSubscriptions[pair] = new ResubscribeSubscription
            {
                ProviderSubscription = response.Data,
                ClientSubscription = response.Data,
                Callback = callback
            };

            return response;
        }

        public override void Unsubscribe(PairSubscriptionGuid pairSubscriptionId)
        {
            var pairSubscription = PairSubscriptions.FirstOrDefault(s => s.Value.ClientSubscription.Id == pairSubscriptionId);
            if(pairSubscription.Value != null)
            {
                base.Unsubscribe(pairSubscription.Value.ProviderSubscription.Id);
                PairSubscriptions.Remove(pairSubscription.Key);
            }            
        }

        public override void Dispose()
        {
            PairSubscriptions.Clear();
            base.Dispose();
        }
    }

    public static class SubscriptionAutoResubscribeWrapperExtension
    {
        public static IExchangeProvider AttachAutoResubscribeWrapper(this IExchangeProvider provider)
        {
            return new AutoResubscribeWrapper(provider);
        }
    }
}
