using Bitfinex.Net;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.ExchangeProviders.Bitfinex.Extensions;
using MentoringProgram.ExchangeProviders.Bitfinex.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.ExchangeProviders.Bitfinex
{
    public class BitfinexProvider : IExchangeProvider
    {
        private readonly BitfinexSocketClient _bitfinexSocketClient;
        private readonly BitfinexClient _bitfinexClient;       
        private Dictionary<TradingPair, SubscriptionSubscribers> Subscriptions = new Dictionary<TradingPair, SubscriptionSubscribers>();

        public event Action OnDisconnected;

        public BitfinexProvider()
        {
            _bitfinexSocketClient = new BitfinexSocketClient();
            _bitfinexClient = new BitfinexClient();
        }

        public Candle GetCurrentCandlePrice(TradingPair pair)
        {
            var result = _bitfinexClient.GetTicker("t" + pair.Name.ToLower()).Data.FirstOrDefault();
            return new Candle((Price)result.Bid, (Price)result.Ask);
        }

        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            if (!Subscriptions.ContainsKey(pair))
            {
                var response = _bitfinexSocketClient.SubscribeToTickerUpdates("t" + pair.Name.ToUpper(), (data) =>
                {
                    var candle = data.ToCandle();
                    var update = new TradeUpdate(pair, candle);
                    NotifyPairSubscribers(pair, update);
                });

                if (!response.Success)
                {
                    return new ResponseResult<Subscription>(response.Error.Message);
                }

                Subscriptions.Add(pair, new SubscriptionSubscribers(response.Data));                
            }

            var subscriber = new Subscriber(Guid.NewGuid(), callback);           

            Subscriptions[pair].Subscribers.Add(subscriber);

            var subscription = new Subscription(subscriber.SubscriptionId, () => Unsubscribe(subscriber.SubscriptionId));

            return new ResponseResult<Subscription>(subscription);
        }
        
        public void Unsubscribe(Guid subscriptionId)
        {
            var pairSubscriptions = Subscriptions.Where(s => s.Value.Subscribers.Any(ss => ss.SubscriptionId == subscriptionId)).FirstOrDefault();
            if(pairSubscriptions.Value == null)
            {
                return;
            }

            var toDelete = pairSubscriptions.Value.Subscribers.Where(s => s.SubscriptionId == subscriptionId).FirstOrDefault();
            pairSubscriptions.Value.Subscribers.Remove(toDelete);

            if (!pairSubscriptions.Value.Subscribers.Any())
            {
                pairSubscriptions.Value.ApiSubscription.Close();
                Subscriptions.Remove(pairSubscriptions.Key);
            }
        }

        public void Dispose()
        {
            OnDisconnected();
            _bitfinexSocketClient.Dispose();
            _bitfinexClient.Dispose();
        }

        private void NotifyPairSubscribers(TradingPair pair, TradeUpdate update)
        {
            var subscription = Subscriptions[pair];
            if (subscription != null)
            {
                foreach (var s in subscription.Subscribers)
                {
                    s.Callback?.Invoke(update);
                }
            }
        }

    }
}
