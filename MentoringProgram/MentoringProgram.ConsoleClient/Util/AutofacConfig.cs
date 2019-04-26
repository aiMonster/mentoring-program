using Autofac;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.ExchangeProviders.Bitfinex;
using MentoringProgram.ExchangeProviders.Bittrex;

namespace MentoringProgram.ConsoleClient.Util
{
    public class AutofacConfig
    {
        public static IContainer Container { get; }

        static AutofacConfig()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BitfinexProvider>().As<IBitfinexProvider>();
            builder.RegisterType<BittrexProvider>().As<IBittrexProvider>();

            builder.RegisterType<MarketManager>();

            Container = builder.Build();
        }
    }
}
