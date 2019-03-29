using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using System;

namespace MentoringProgram.Common.Wrappers
{
    public class AlwaysOn : IExchangeProvider
    {
        private readonly IExchangeProvider exchangeProvider;

        public AlwaysOn(IExchangeProvider provider)
        {
            exchangeProvider = provider;
            exchangeProvider.OnDisconnected += HandleDisconnecting;
        }

        public event Action OnDisconnected
        {
            add
            {
                exchangeProvider.OnDisconnected += value;
            }
            remove
            {
                exchangeProvider.OnDisconnected -= value;
            }
        }

        private void HandleDisconnecting()
        {
            Console.WriteLine("AlwaysOn: Handled disconnecting and connnected again");
            exchangeProvider.Connect();
        }
        public void Connect()
        {
            exchangeProvider.Connect();
        }

        public void Dispose()
        {
            exchangeProvider.Dispose();
        }

        public Candle GetCurrentCandlePrice(TradingPair pair)
        {
            return exchangeProvider.GetCurrentCandlePrice(pair);
        }

        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            return exchangeProvider.Subscribe(pair, callback);
        }

        public void Unsubscribe(Guid subscriptionId)
        {
            exchangeProvider.Unsubscribe(subscriptionId);
        }
    }

    public static class AlwaysOnExtension
    {
        public static IExchangeProvider AlwaysOn(this IExchangeProvider provider)
        {
            return new AlwaysOn(provider);
        }
    }
}
