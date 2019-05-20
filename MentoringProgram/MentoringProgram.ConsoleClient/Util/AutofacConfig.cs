using Autofac;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.ExchangeProviders.Bitfinex;
using MentoringProgram.ExchangeProviders.Bittrex;
using MentoringProgram.ExchangeProviders.Fake;
using System.Reflection;

namespace MentoringProgram.ConsoleClient.Util
{
    public class AutofacConfig
    {
        public static IContainer Container { get; }

        static AutofacConfig()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BitfinexProvider>().AsImplementedInterfaces();
            builder.RegisterType<BittrexProvider>().AsImplementedInterfaces();
            builder.RegisterType<FakeProvider>().AsImplementedInterfaces();

            builder.RegisterType<MarketManager>();

            Container = builder.Build();
        }
    }
}
