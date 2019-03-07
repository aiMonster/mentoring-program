using Bitfinex.Net;
using CryptoExchange.Net.Sockets;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.ExchangeProviders.Bitfinex
{
    public class BitfinexProvider : IExchangeProvider
    {
        private readonly BitfinexSocketClient _bitfinexSocketClient;
        private readonly BitfinexClient _bitfinexClient;

        private Dictionary<Subscription, UpdateSubscription> Subscriptions { get; } = new Dictionary<Subscription, UpdateSubscription>();

        public BitfinexProvider()
        {
            _bitfinexSocketClient = new BitfinexSocketClient();
            _bitfinexClient = new BitfinexClient();
        }

        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            var response = _bitfinexSocketClient.SubscribeToTradeUpdates("t" + pair.ToUpper().Name, (data) =>
            {
                var input = data.Select(d => new TradeUpdate(pair, new Price(d.Price))).First();
                callback?.Invoke(input);
            });

            if (!response.Success)
            {
                return new ResponseResult<Subscription>(response.Error.Message);
            }
            else
            {
                var subscription = new Subscription(Guid.NewGuid());
                Subscriptions.Add(subscription, response.Data);

                return new ResponseResult<Subscription>(subscription);
            }
        }

        public void Unsubscribe(Subscription subscription)
        {
            var result = Subscriptions[subscription];
            if(result != null)
            {
                result.Close();
                Subscriptions.Remove(subscription);
            }
        }

        public void Dispose()
        {
            _bitfinexSocketClient.Dispose();
            _bitfinexClient.Dispose();
        }

        public Price GetCurrentPrice(TradingPair pair)
        {
            var value = _bitfinexClient.GetTrades(pair.ToUpper().Name, 1).Data.First().Price;
            return new Price(value);
        }
    }
}
