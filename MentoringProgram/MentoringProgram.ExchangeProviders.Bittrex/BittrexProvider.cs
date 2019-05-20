using Bittrex.Net;
using Bittrex.Net.Objects;
using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.ExchangeProviders.Bittrex.Extensions;
using MentoringProgram.ExchangeProviders.Bittrex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentoringProgram.ExchangeProviders.Bittrex
{
    public class BittrexProvider : IExchangeProvider
    {
        private object Locker { get; set; } = new object();
        private readonly BittrexSocketClient _bittrexSocketClient;
        private readonly BittrexClient _bittrexClient;

        private Dictionary<Subscriber, TradingPair> Subscriptions = new Dictionary<Subscriber, TradingPair>();        
        
        public event Action OnDisconnected;

        public TradingMarket Type { get => TradingMarket.Bittrex; }

        public BittrexProvider()
        {
            _bittrexSocketClient = new BittrexSocketClient();            
            _bittrexClient = new BittrexClient();
        }

        public async Task ConnectAsync()
        {
            _bittrexSocketClient.OnDisconnected += OnDisconnected;
            await StartReceiveUpdatesAsync();
        }

        public Candle GetCurrentCandlePrice(TradingPair pair)
        {
            var candle = _bittrexClient.GetTicker(pair.ToBittrexPair()).Data;
            return new Candle((Price)candle.Bid, (Price)candle.Ask);    
        }
       
        public async Task<ResponseResult<Subscription>> SubscribeAsync(TradingPair pair, Action<TradeUpdate> callback)
        {  
            var newSubscriptionId = Guid.NewGuid();
            var subscriber = new Subscriber(newSubscriptionId, callback);

            Subscriptions.Add(subscriber, pair);            

            var subscription = new Subscription(newSubscriptionId, async () => await UnsubscribeAsync(newSubscriptionId));

            await Task.CompletedTask;
            return new ResponseResult<Subscription>(subscription);
        }
                
        public async Task UnsubscribeAsync(PairSubscriptionGuid pairSubscriptionId)
        {
            lock (Locker)
            {
                if (Subscriptions.ContainsKey(pairSubscriptionId))
                {
                    Subscriptions.Remove(pairSubscriptionId);
                }
            }

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            OnDisconnected();

            _bittrexSocketClient.OnDisconnected -= OnDisconnected;
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

            var callbacks = Subscriptions.Where(s => s.Value.Equals(pair)).Select(s => s.Key.Callback);
            
            foreach (var callback in callbacks)
            {
                callback?.Invoke(summary.ToTradeUpdate());
            }
        }

        private async Task StartReceiveUpdatesAsync()
        {
            await _bittrexSocketClient.SubscribeToMarketSummariesUpdateAsync(data =>
            {
                var subscribedPairs = data.Where(d => Subscriptions.Values.Contains(d.MarketName.ToTradingPair())).ToList();
                
                foreach (var marketSummary in subscribedPairs)
                {
                    InvokeMarketUpdateCallbacks(marketSummary);
                }
            });
        }        

        #endregion

    }
}
