using Bitfinex.Net;
using CryptoExchange.Net.Sockets;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using NativeBitfinexClient = Bitfinex.Net.BitfinexClient;

namespace MentoringProgram.MarketClients
{
    public class BitfinexClient : IMarketClient
    {
        private readonly BitfinexSocketClient _bitfinexSocketClient;
        private readonly NativeBitfinexClient _bitfinexClient;

        private Dictionary<Guid, UpdateSubscription> Subscriptions { get; set; } = new Dictionary<Guid, UpdateSubscription>();
        private Dictionary<Guid, UpdateSubscription> Alerts { get; set; } = new Dictionary<Guid, UpdateSubscription>();

        public List<TradingPair> _aviablePairs { get; }
        public string Name { get; }

        //TODO: get Name from config file
        public BitfinexClient()
        {
            Name = "Bitfinex market";

            _bitfinexSocketClient = new BitfinexSocketClient();
            _bitfinexClient = new NativeBitfinexClient();

            _aviablePairs = _bitfinexClient.GetSymbols().Data.ToList().Select(s => new TradingPair(s)).ToList();
        }

        public TradingPair GetPairByName(string name) =>        
             _aviablePairs.FirstOrDefault(p => p.Name == name.ToLower());    

        public ResponseResult<Guid> SubscribeToTraidingPair(TradingPair pair, Action<TradeSimple> callBack)
        {           
            var subscription = _bitfinexSocketClient.SubscribeToTradeUpdates("t" + pair.Name.ToUpper(), (data) =>
            {
                var input = data.Select(d => new TradeSimple()
                {
                    Price = d.Price,
                    Amount = d.Amount
                }).First();

                callBack?.Invoke(input);               
            });

            if (!subscription.Success)
            {
                return new ResponseResult<Guid>(subscription.Error.Message);
            }
            else
            {
                var id = Guid.NewGuid();
                Subscriptions.Add(id, subscription.Data);

                return new ResponseResult<Guid>(id);
            }
        }

        public void UnsubscribeTraidingPair(Guid id)
        {           
            var subscripition = Subscriptions[id];
            if(subscripition != null)
            {
                subscripition.Close();
                Subscriptions.Remove(id);
            }
        }
       
        public ResponseResult<Guid> SetUpAlert(TradingRule rule, bool callOnce = true)
        {           
            var currentRate = _bitfinexClient.GetTrades(rule.Pair.Name.ToUpper(), 1).Data.First().Price;
            var wasShown = false;

            var subscription = _bitfinexSocketClient.SubscribeToTradeUpdates("t" + rule.Pair.Name.ToUpper(), (data) =>
            {
                var input = data.Select(d => new TradeSimple()
                {
                    Price = d.Price,
                    Amount = d.Amount
                }).First();

                if(!wasShown || !callOnce)
                {
                    if (currentRate < rule.Boundary)
                    {
                        if (input.Price >= rule.Boundary)
                        {
                            rule.Callback?.Invoke();
                            wasShown = true;
                        }
                    }
                    else if (currentRate > rule.Boundary)
                    {
                        if (input.Price <= rule.Boundary)
                        {
                            rule.Callback?.Invoke();
                            wasShown = true;
                        }
                    }
                    else
                    {
                        rule.Callback?.Invoke();
                        wasShown = true;
                    }
                }
            });
           
            if (!subscription.Success)
            {
                return new ResponseResult<Guid>(subscription.Error.Message);
            }
            else
            {
                var id = Guid.NewGuid();
                Alerts.Add(id, subscription.Data);

                return new ResponseResult<Guid>(id);
            }
        }

        public void RemoveAlert(Guid id)
        {
            var alert = Alerts[id];
            if (alert != null)
            {
                alert.Close();
                Alerts.Remove(id);
            }
        }

        public void Dispose()
        {
            _bitfinexSocketClient.Dispose();
            _bitfinexClient.Dispose();
        }        
    }
}
