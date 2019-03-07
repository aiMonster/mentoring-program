using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.ExchangeProviders.Bitfinex;
using System;
using System.Collections.Generic;

namespace MentoringProgram.ConsoleClient
{
    public class MarketManager
    {
        BitfinexProvider _bitfinexProvider { get; }

        private Dictionary<Subscription, PairSubscription> Subscriptions = new Dictionary<Subscription, PairSubscription>();

        public MarketManager()
        {           
            _bitfinexProvider = new BitfinexProvider();
        }

        #region Subscriptions

        #region IMPLEMENT

        //public void AddMarketsToSubscription(Subscription subscription, params TradingMarket[] markets)
        //{
        //    var subscriptionToUpdate = Subscriptions[subscription];
        //    if(subscriptionToUpdate == null)
        //    {
        //        return;
        //    }

        //    markets = markets.Except(subscriptionToUpdate.MarketSubscriptions.Select(s => s.Market)).ToArray();

        //    foreach(var market in markets)
        //    {
        //        var provider = GetExchangeProvider(market);
        //        var subscrip = provider.Subscribe(subscriptionToUpdate.Pair, subscriptionToUpdate.Callback);

        //        subscriptionToUpdate.MarketSubscriptions.Add(new MarketSubscription(subscrip.Data.Id, market));
        //    }          
        //}
       
        //public void RemoveMarketsFromSubscription(Subscription subscription, params TradingMarket[] markets)
        //{
        //    var subscriptionToUpdate = Subscriptions[subscription];
        //    if(subscriptionToUpdate == null)
        //    {
        //        return;
        //    }

        //    var marketSubscriptions = subscriptionToUpdate.MarketSubscriptions.Where(s => markets.Contains(s.Market)).ToList();

        //    foreach (var market in marketSubscriptions)
        //    {
        //        var provider = GetExchangeProvider(market.Market);
        //        provider.Unsubscribe(market);

        //        subscriptionToUpdate.MarketSubscriptions.Remove(market);
        //    }

        //    if(subscriptionToUpdate.MarketSubscriptions.Count == 0)
        //    {
        //        Subscriptions.Remove(subscription);
        //    }
        //}

        #endregion

        public Subscription Subscribe(TradingPair pair, Action<TradeUpdate> callback, params TradingMarket[] markets)
        {
            var pairSubscription = new PairSubscription(pair, callback);

            foreach(var market in markets)
            {
                var provider = GetExchangeProvider(market);
                var marketSubscription = provider.Subscribe(pair, callback);

                //TODO: Check is subscription successfull
                pairSubscription.MarketSubscriptions.Add(new MarketSubscription(marketSubscription.Data.Id, market));
            }

            var subscription = new Subscription(Guid.NewGuid());

            Subscriptions.Add(subscription, pairSubscription);
            return subscription;
        }
        public Subscription Subscribe(TradingRule rule, Action callback, bool invokeOnce = true)
        {
            var currentLowestRate = GetLowestRate(rule.TradingMarkets, rule.Pair);
            var currentHighestRate = GetHighestRate(rule.TradingMarkets, rule.Pair);

            var wasInvoked = false;

            Action<TradeUpdate> alertCallback = delegate (TradeUpdate tradeUpdate)
            {
                if (!wasInvoked || !invokeOnce)
                {
                    if (rule.PriceDirection == PriceDirection.Down)
                    {
                        if (tradeUpdate.Price < rule.Boundary || currentLowestRate < rule.Boundary)
                        {
                            callback?.Invoke();
                            wasInvoked = true;
                        }
                    }
                    else if (rule.PriceDirection == PriceDirection.Up)
                    {
                        if (tradeUpdate.Price > rule.Boundary || currentHighestRate > rule.Boundary)
                        {
                            callback?.Invoke();
                            wasInvoked = true;
                        }
                    }
                }
            };

            var alertSubscription = Subscribe(rule.Pair, alertCallback, rule.TradingMarkets.ToArray());

            return alertSubscription;
        }

        public void Unsubscribe(Subscription subscription)
        {
            var subscriptionResult = Subscriptions[subscription];
            if(subscriptionResult != null)
            {
                foreach(var marketSubscription in subscriptionResult.MarketSubscriptions)
                {
                    var provider = GetExchangeProvider(marketSubscription.Market);
                    provider.Unsubscribe(marketSubscription);
                }
                Subscriptions.Remove(subscription);
            }
        }

        #endregion

        private IExchangeProvider GetExchangeProvider(TradingMarket marketType)
        {
            switch (marketType)
            {
                case TradingMarket.Bitfinex:
                    return _bitfinexProvider;
                case TradingMarket.Binance:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(marketType));
            }
        }               
       
        private Price GetLowestRate(List<TradingMarket> tradingMarkets, TradingPair pair)
        {
            var lowest = new Price(decimal.MaxValue);

            foreach(var market in tradingMarkets)
            {
                var exchangeProvider = GetExchangeProvider(market);

                var current = exchangeProvider.GetCurrentPrice(pair);
                lowest = current < lowest ? current : lowest;
            }

            return lowest;
        }

        private Price GetHighestRate(List<TradingMarket> tradingMarkets, TradingPair pair)
        {
            var highest = new Price(decimal.Zero);

            foreach (var market in tradingMarkets)
            {
                var exchangeProvider = GetExchangeProvider(market);

                var current = exchangeProvider.GetCurrentPrice(pair);
                highest = current > highest ? current : highest;
            }

            return highest;
        }        

    }
}
