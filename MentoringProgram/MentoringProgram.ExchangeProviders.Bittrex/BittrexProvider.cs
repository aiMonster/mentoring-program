using Bittrex.Net;
using Bittrex.Net.Objects;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.ExchangeProviders.Bittrex.Extensions;
using MentoringProgram.ExchangeProviders.Bittrex.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.ExchangeProviders.Bittrex
{
    public class BittrexProvider : IExchangeProvider
    {
        private readonly BittrexSocketClient _bittrexSocketClient;
        private readonly BittrexClient _bittrexClient; 
        
        private Dictionary<TradingPair, List<Subscriber>> Subscriptions = new Dictionary<TradingPair, List<Subscriber>>();
        
        public event Action OnDisconnected; 

        public BittrexProvider()
        {
            _bittrexSocketClient = new BittrexSocketClient();            
            _bittrexClient = new BittrexClient();
        }

        public void Connect()
        {
            StartReceiveUpdates();
        }

        public Candle GetCurrentCandlePrice(TradingPair pair)
        {
            var candle = _bittrexClient.GetTicker(pair.ToBittrexPair()).Data;
            return new Candle((Price)candle.Bid, (Price)candle.Ask);    
        }

        //TODO: Check if pair is supported by this provider
        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {     
            if (!Subscriptions.ContainsKey(pair))
            {
                Subscriptions[pair] = new List<Subscriber>();
            }

            var newSubscriptionId = Guid.NewGuid();

            Subscriptions[pair].Add(new Subscriber(newSubscriptionId, callback));

            var subscription = new Subscription(newSubscriptionId, () => Unsubscribe(newSubscriptionId));           

            return new ResponseResult<Subscription>(subscription);
        }
                
        public void Unsubscribe(Guid subscriptionId)
        {
            var subscription = Subscriptions.FirstOrDefault(s => s.Value.Any(sub => sub.SubscriptionId == subscriptionId));
            if(subscription.Value == null)
            {
                return;
            }

            var subscriber = subscription.Value.FirstOrDefault(s => s.SubscriptionId == subscriptionId);
            subscription.Value.Remove(subscriber);
            if (!subscription.Value.Any())
            {
                Subscriptions.Remove(subscription.Key);
            }           
        }

        public void Dispose()
        {
            OnDisconnected();
            _bittrexClient.Dispose();
            _bittrexSocketClient.Dispose();
        }

        public override string ToString()
        {
            return "BittrexProvider";
        }

        #region Private members

        private void InvokeMarketUpdateCallbacks(BittrexStreamMarketSummary summary)
        {
            var pair = summary.MarketName.ToTradingPair();
            if (!Subscriptions.ContainsKey(pair))
            {
                return;
            }

            foreach (var subscriber in Subscriptions[pair])
            {
                subscriber.Callback?.Invoke(summary.ToTradeUpdate());
            }
        }

        private void StartReceiveUpdates()
        {
            _bittrexSocketClient.SubscribeToMarketSummariesUpdate(data =>
            {
                var subscribedPairs = data.Where(d => Subscriptions.Keys.Contains(d.MarketName.ToTradingPair())).ToList();

                foreach (var marketSummary in subscribedPairs)
                {
                    InvokeMarketUpdateCallbacks(marketSummary);
                }
            });
        }
        

        #endregion

    }
}
