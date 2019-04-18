using Bitfinex.Net;
using CryptoExchange.Net.Sockets;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.ExchangeProviders.Bitfinex.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.ExchangeProviders.Bitfinex
{
    public class BitfinexProvider : IExchangeProvider
    {
        private readonly BitfinexSocketClient _bitfinexSocketClient;
        private readonly BitfinexClient _bitfinexClient;

        private Dictionary<Subscription, UpdateSubscription> Subscriptions = new Dictionary<Subscription, UpdateSubscription>();        

        public event Action OnDisconnected;

        public BitfinexProvider()
        {
            _bitfinexSocketClient = new BitfinexSocketClient();
            _bitfinexClient = new BitfinexClient();
        }

        public void Connect()
        {
          
        }

        public Candle GetCurrentCandlePrice(TradingPair pair)
        {
            var result = _bitfinexClient.GetTicker(pair.ToBitfinexPair()).Data.FirstOrDefault();
            return new Candle((Price)result.Bid, (Price)result.Ask);
        }

        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            var response = _bitfinexSocketClient.SubscribeToTickerUpdates(pair.ToBitfinexPair(), (data) =>
            {
                var candle = data.ToCandle();
                var update = new TradeUpdate(pair, candle);
                callback?.Invoke(update);
            });

            if (!response.Success)
            {
                return new ResponseResult<Subscription>(response.Error.Message);
            }

            var id = Guid.NewGuid();
            var subscription = new Subscription(id, () => Unsubscribe(id));
            Subscriptions.Add(subscription, response.Data);

            return new ResponseResult<Subscription>(subscription);
        }
        
        public void Unsubscribe(PairSubscriptionGuid pairSubscriptionId)
        {            
            if(Subscriptions.ContainsKey(pairSubscriptionId))
            {
                var subscription = Subscriptions[pairSubscriptionId];
                subscription.Close();
                Subscriptions.Remove(pairSubscriptionId);
            }
        }

        public void Dispose()
        {
            OnDisconnected();
            _bitfinexSocketClient.Dispose();
            _bitfinexClient.Dispose();
        }

        public override string ToString()
        {
            return "BitfinexProvider";
        }
    }
}
