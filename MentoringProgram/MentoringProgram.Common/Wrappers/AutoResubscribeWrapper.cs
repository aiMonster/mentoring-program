using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MentoringProgram.Common.Wrappers
{  
    public class AutoResubscribeWrapper : BaseWrapper
    {
        private SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private Dictionary<TradingPair, ResubscribeSubscription> PairSubscriptions { get; } = new Dictionary<TradingPair, ResubscribeSubscription>();

        public AutoResubscribeWrapper(IExchangeProvider provider) : base(provider) { }

        public override async Task ConnectAsync()
        { 
            await base.ConnectAsync();
            foreach (var subscription in PairSubscriptions)
            {
                var response = await base.SubscribeAsync(subscription.Key, subscription.Value.Callback);
                subscription.Value.ProviderSubscription = response.Data;
            }
        }

        public override async Task<ResponseResult<Subscription>> SubscribeAsync(TradingPair pair, Action<TradeUpdate> callback)
        {            
            var response = await base.SubscribeAsync(pair, callback);
            PairSubscriptions[pair] = new ResubscribeSubscription
            {
                ProviderSubscription = response.Data,
                ClientSubscription = response.Data,
                Callback = callback
            };

            return response;
        }

        public override async Task UnsubscribeAsync(PairSubscriptionGuid pairSubscriptionId)
        {
            await Semaphore.WaitAsync();
            try
            {
                var pairSubscription = PairSubscriptions.FirstOrDefault(s => s.Value.ClientSubscription.Id == pairSubscriptionId);
                if (pairSubscription.Value != null)
                {
                    await base.UnsubscribeAsync(pairSubscription.Value.ProviderSubscription.Id);
                    PairSubscriptions.Remove(pairSubscription.Key);
                }
            }
            finally
            {
                Semaphore.Release();
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
