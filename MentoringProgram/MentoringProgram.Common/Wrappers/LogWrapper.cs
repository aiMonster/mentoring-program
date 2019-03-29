using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using System;

namespace MentoringProgram.Common.Wrappers
{
    public class LogWrapper : IExchangeProvider
    {
        private readonly IExchangeProvider exchangeProvider;

        public LogWrapper(IExchangeProvider provider)
        {
            exchangeProvider = provider;
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

        public void Connect()
        {
            Console.WriteLine("LogWrapper: Connecting to exchange provider - " + exchangeProvider.ToString());
            exchangeProvider.Connect();
        }

        public void Dispose()
        {
            exchangeProvider.Dispose();
            Console.WriteLine("LogWrapper: Our provider was disposed - " + exchangeProvider.ToString());
        }

        public Candle GetCurrentCandlePrice(TradingPair pair)
        {
            return exchangeProvider.GetCurrentCandlePrice(pair);
        }

        public ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            Console.WriteLine("LogWrapper: Somebody has subscribed to pair - " + pair.ToString());
            return exchangeProvider.Subscribe(pair, callback);
        }

        public void Unsubscribe(Guid subscriptionId)
        {
            Console.WriteLine("LogWrapper: Somebody has unsubscribed");
            exchangeProvider.Unsubscribe(subscriptionId);
        }
    }

    public static class LogWrapperExtension
    {
        public static IExchangeProvider AttachLoger(this IExchangeProvider provider)
        {
            return new LogWrapper(provider);
        }
    }
}
