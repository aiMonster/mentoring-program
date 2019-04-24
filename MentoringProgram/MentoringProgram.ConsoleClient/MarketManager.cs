using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.Common.Rules;
using MentoringProgram.Common.Wrappers;
using MentoringProgram.ExchangeProviders.Bitfinex;
using MentoringProgram.ExchangeProviders.Bittrex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MentoringProgram.ConsoleClient
{
    public class MarketManager
    {
        IExchangeProvider _bitfinexProvider { get; }
        IExchangeProvider _bittrexProvider { get; }

        IExchangeProvider _testProvider { get; set; }

        private SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private Dictionary<BaseRule, ClientMarketSubscriptions> Subscriptions = new Dictionary<BaseRule, ClientMarketSubscriptions>();

        public MarketManager()
        {
            _bitfinexProvider = new BitfinexProvider().AttachLoger("Logger1")
                                                      .AttachAutoResubscribeWrapper()
                                                      .AttachSubscriptionDublicatesWrapper()
                                                      .AttachLoger("Logger2")
                                                      .AttachAlwaysOn();

            _bittrexProvider = new BittrexProvider().AttachLoger("Log1")
                                                    .AttachAutoResubscribeWrapper()
                                                    .AttachSubscriptionDublicatesWrapper()
                                                    .AttachLoger("Log2")
                                                    .AttachAlwaysOn();
         
        }

        public void ConnectToExchangeProviders()
        {
            _bitfinexProvider.Connect();
            _bittrexProvider.Connect();
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
            var subscription = new Subscription(subscriptionId, () => UnsubscribeAsync(subscriptionId));

            var clientSubscription = new ClientSubscription(subscription, callback);
            Subscriptions[rule].ClientSubscriptions.Add(clientSubscription);

            return subscription;
        }

        public async Task UnsubscribeAsync(RuleSubscriptionGuid ruleSubscriptionId)
        {
            await Semaphore.WaitAsync();
            try
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
            }
            finally
            {
                Semaphore.Release();
            }            
        }

        private IExchangeProvider GetExchangeProvider(TradingMarket marketType)
        {
            switch (marketType)
            {
                case TradingMarket.Bitfinex:
                    return _bitfinexProvider;
                case TradingMarket.Bittrex:
                    return _bittrexProvider;
                default:
                    throw new ArgumentOutOfRangeException(nameof(marketType));
            }
        }
    }  
}
