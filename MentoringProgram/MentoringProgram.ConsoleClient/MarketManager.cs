using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.Common.Rules;
using MentoringProgram.Common.Wrappers;
using MentoringProgram.ExchangeProviders.Bitfinex;
using MentoringProgram.ExchangeProviders.Bittrex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.ConsoleClient
{
    
    public class MarketManager
    {
        IExchangeProvider _bitfinexProvider { get; }
        IExchangeProvider _bittrexProvider { get; }

        private Dictionary<BaseRule, RuleSubscription> Subscriptions = new Dictionary<BaseRule, RuleSubscription>();

        public MarketManager()
        {
            //Note that if we change alwaysOn with Log, result will be completele different as we use ToString
            //Looks like we should override this method in AlwaysOn as well
            //new BitfinexProvider().AlwaysOn().Log();
            _bitfinexProvider = new BitfinexProvider().AttachLoger().AlwaysOn();
            _bittrexProvider = new BittrexProvider().AttachLoger().AlwaysOn();
        }

        public void ConnectToExchangeProviders()
        {
            _bitfinexProvider.Connect();
            _bittrexProvider.Connect();
        }

        public Subscription SubscribeRule(BaseRule rule, Action callback)
        {
            if (Subscriptions.ContainsKey(rule))
            {
                return Subscriptions[rule].Subscription;
            }

            Action<TradeUpdate> marketCallback = (update) =>
            {
                Console.WriteLine(update.CandlePrice.Ask);
                if (rule.IsConditionMet(update))
                {
                    callback?.Invoke();
                }
            };

            var subscriptionId = Guid.NewGuid();

            var ruleSubscription = new RuleSubscription();
            ruleSubscription.Subscription = new Subscription(subscriptionId, () => Unsubscribe(subscriptionId));
            ruleSubscription.MarketSubscriptions = new List<MarketSubscription>();

            foreach (var market in rule.TradingMarkets)
            {
                var provider = GetExchangeProvider(market);
                var subscription = provider.Subscribe(rule.Pair, marketCallback);

                var marketSubscription = new MarketSubscription(subscription.Data, market);

                ruleSubscription.MarketSubscriptions.Add(marketSubscription);
            }

            return ruleSubscription.Subscription;
        }       

        public void Unsubscribe(Guid subscriptionId)
        {
            var subscription = Subscriptions.FirstOrDefault(v => v.Value.Subscription.Id == subscriptionId);
            if(subscription.Value == null)
            {
                return;
            }

            foreach(var marketSubscription in subscription.Value.MarketSubscriptions)
            {
                marketSubscription.Dispose();
            }

            Subscriptions.Remove(subscription.Key);
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
