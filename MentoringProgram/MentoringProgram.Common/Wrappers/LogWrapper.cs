using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using System;

namespace MentoringProgram.Common.Wrappers
{
    public class LogWrapper : BaseWrapper
    {       
        private string Name { get; }
        public LogWrapper(IExchangeProvider provider, string name) : base(provider)
        {
            Name = name;
        }
  
        public override void Connect()
        {
            Console.WriteLine($"{Name}: Connecting to exchange provider - {base.ToString()}");
            base.Connect();
        }

        public override void Dispose()
        {            
            Console.WriteLine($"{Name}: Our provider was disposed - {base.ToString()}");
            base.Dispose();
        }

        public override ResponseResult<Subscription> Subscribe(TradingPair pair, Action<TradeUpdate> callback)
        {
            Console.WriteLine($"{Name}: Somebody has subscribed to pair - {pair.ToString()} ({base.ToString()})");
            return base.Subscribe(pair, callback);
        }

        public override void Unsubscribe(Guid subscriptionId)
        {
            Console.WriteLine($"{Name}: Somebody has unsubscribed ({base.ToString()})");            
            base.Unsubscribe(subscriptionId);
        }
    }

    public static class LogWrapperExtension
    {
        public static IExchangeProvider AttachLoger(this IExchangeProvider provider, string name = "LoggWrapper")
        {
            return new LogWrapper(provider, name);
        }
    }
}
