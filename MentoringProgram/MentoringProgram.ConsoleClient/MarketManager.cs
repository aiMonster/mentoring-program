﻿using MentoringProgram.Common.Enums;
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

        private Dictionary<BaseRule, ClientMarketSubscriptions> Subscriptions = new Dictionary<BaseRule, ClientMarketSubscriptions>();

        public MarketManager()
        {
            _bitfinexProvider = new BitfinexProvider().AttachLoger("Logger1")
                                                      .AttachSubscriptionDublicatesWrapper()
                                                      .AttachLoger("Logger2")
                                                      .AttachAlwaysOn();

            _bittrexProvider = new BittrexProvider().AttachLoger("Log1")
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

        public Subscription SubscribeRule(BaseRule rule, Action callback)
        {
            if (!Subscriptions.ContainsKey(rule))
            {
                Subscriptions[rule] = new ClientMarketSubscriptions();

                foreach(var market in rule.TradingMarkets)
                {
                    var provider = GetExchangeProvider(market);
                    var response = provider.Subscribe(rule.Pair, (update) => Notify(rule, update));
                    var marketSubscription = new MarketSubscription(response.Data, market);
                    Subscriptions[rule].MarketSubscriptions.Add(marketSubscription);
                }
            }

            var subscriptionId = Guid.NewGuid();
            var subscription = new Subscription(subscriptionId, () => Unsubscribe(subscriptionId));

            var clientSubscription = new ClientSubscription(subscription, callback);
            Subscriptions[rule].ClientSubscriptions.Add(clientSubscription);

            return subscription;
        }

        public void Unsubscribe(Guid subscriptionId)
        {
            var subscription = Subscriptions.FirstOrDefault(v => v.Value.ClientSubscriptions.Any(ss => ss.Subscription.Id == subscriptionId));
            if (subscription.Value == null)
            {
                return;
            }

            var subscriber = subscription.Value.ClientSubscriptions.FirstOrDefault(s => s.Subscription.Id == subscriptionId);
            subscription.Value.ClientSubscriptions.Remove(subscriber);

            if (!subscription.Value.ClientSubscriptions.Any())
            {
                foreach(var marketSubscription in subscription.Value.MarketSubscriptions)
                {
                    var provider = GetExchangeProvider(marketSubscription.Market);
                    provider.Unsubscribe(marketSubscription.Id);
                }
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
