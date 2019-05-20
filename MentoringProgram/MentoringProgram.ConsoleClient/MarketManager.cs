using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Helpers;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.Common.Rules;
using MentoringProgram.Common.Wrappers;
using MentoringProgram.ConsoleClient.Util;
using MentoringProgram.ExchangeProviders.Bitfinex;
using MentoringProgram.ExchangeProviders.Bittrex;
using MentoringProgram.ExchangeProviders.Fake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MentoringProgram.ConsoleClient
{
    public class MarketManager
    {
        private IEnumerable<IExchangeProvider> _exchangeProviders;      

        private SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private Dictionary<BaseRule, ClientMarketSubscriptions> Subscriptions = new Dictionary<BaseRule, ClientMarketSubscriptions>();              

        public MarketManager(IEnumerable<IExchangeProvider> exhangeProviders)
        {
            _exchangeProviders = exhangeProviders.Select(provider => provider.AttachLoger($"Logger1 ({ provider.ToString() })")
                                                                             .AttachAutoResubscribeWrapper()
                                                                             .AttachLoger($"Logger2 ({ provider.ToString() })")
                                                                             .AttachAlwaysOn());
        }

        public void ConnectToExchangeProviders()
        {
            Parallel.ForEach(_exchangeProviders, async (provider) => await provider.ConnectAsync());               
        }
        public void Notify(BaseRule rule, TradeUpdate update)
        {
            if (!Subscriptions.ContainsKey(rule))
            {
                return; 
            }

            if(!rule.IsConditionMet(update))
            {
                return;
            }

            foreach(var clientSubscription in Subscriptions[rule].ClientSubscriptions)
            {
                Console.WriteLine(update.CandlePrice.Ask);
                clientSubscription.Callback?.Invoke();                
            }            
        }

        public async Task<Subscription> SubscribeRuleAsync(BaseRule rule, Action callback)
        {
            if (!Subscriptions.ContainsKey(rule))
            {
                Subscriptions[rule] = new ClientMarketSubscriptions();

                foreach(var market in rule.TradingMarkets)
                {
                    var provider = GetExchangeProvider(market);
                    var response = await provider.SubscribeAsync(rule.Pair, (update) => Notify(rule, update));
                    var marketSubscription = new MarketSubscription(response.Data, market);
                    Subscriptions[rule].MarketSubscriptions.Add(marketSubscription);
                }
            }

            var subscriptionId = Guid.NewGuid();
            var subscription = new Subscription(subscriptionId, async () => await UnsubscribeAsync(subscriptionId));

            var clientSubscription = new ClientSubscription(subscription, callback);
            Subscriptions[rule].ClientSubscriptions.Add(clientSubscription);

            return subscription;
        }

        public Task UnsubscribeAsync(RuleSubscriptionGuid ruleSubscriptionId)
        {
            return ThreadSafeRunner.Run( async () => 
            {
                var subscription = Subscriptions.FirstOrDefault(v => v.Value.ContainsRuleSubscription(ruleSubscriptionId));
                if (subscription.Value == null)
                {
                    return;
                }

                var subscriber = subscription.Value.GetClientSubscription(ruleSubscriptionId);
                subscription.Value.ClientSubscriptions.Remove(subscriber);

                if (!subscription.Value.ClientSubscriptions.Any())
                {
                    foreach (var marketSubscription in subscription.Value.MarketSubscriptions)
                    {
                        var provider = GetExchangeProvider(marketSubscription.Market);
                        await provider.UnsubscribeAsync(marketSubscription.Id);
                    }
                }

                Subscriptions.Remove(subscription.Key);

            }, Semaphore);
        }

        

        private IExchangeProvider GetExchangeProvider(TradingMarket marketType) => 
            _exchangeProviders.Single(p => p.Type == marketType);      
    }  
}
