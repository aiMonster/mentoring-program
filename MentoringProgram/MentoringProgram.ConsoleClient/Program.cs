using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.Subscriptions;
using MentoringProgram.Common.Rules;
using MentoringProgram.Common.Rules.PriceReachedRule;
using MentoringProgram.Common.Rules.PriceReachedRule.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MentoringProgram.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Console.WriteLine("An exception was thrown!");
            };

            Console.WriteLine("Started ... we will subscribe for two rules");
            Console.WriteLine("Press enter to unsubscribe for one of them and see results ...\n");

            var manager = new MarketManager();           
            manager.ConnectToExchangeProviders(); 
            var s2 = SubscribeAndSetUpDefaultAlertAsync(manager).Result;
            //var s3 = SubscribeAndSetUpDefaultAlert(manager);
            
            Console.ReadLine();
            manager.UnsubscribeAsync(s2.Id).Wait();           
            Console.WriteLine("Should be unsubscribed of one only, press enter to exit ...");
            Console.ReadLine();
        }

        private static Subscription SubscribeAndDisplay(MarketManager marketManager)
        {
            var previous = default(decimal);

            Action<TradeUpdate> callback = update =>
            {                
                var current = update.CandlePrice.Ask;

                ConsoleColor color;
                if (previous > current.Value)
                {
                    color = ConsoleColor.Red;
                }
                else if (previous < current.Value)
                {
                    color = ConsoleColor.Green;
                }
                else
                {
                    color = ConsoleColor.Yellow;
                }

                Console.ForegroundColor = color;

                previous = current.Value;
                Console.WriteLine($"Price: {current}");
            };

            //var subscription = marketManager.Subscribe(GetPairFromUser(), callback, GetMarketsFromUser());
            //return subscription;
            return null;
        }

        private static async Task<Subscription> SubscribeAndSetUpAlertAsync(MarketManager marketManager)
        {
            var rule = new PriceReachedRule.Builder()
                                     .AddMarkets(GetMarketsFromUser())
                                     .SetPair(GetPairFromUser())
                                     .SetBoundary(GetBoundaryFromUser(), GetBoundaryDirectionFromUser(), PriceType.Ask)
                                     .Build();

            Action alertCallback = () => Console.WriteLine("Heeyy, we broke boundary, writing you on telegram and viber");
            
            var subscription = await marketManager.SubscribeRuleAsync(rule, alertCallback);
            return subscription;
        }

        private static async Task<Subscription> SubscribeAndSetUpDefaultAlertAsync(MarketManager marketManager)
        {
            var rule = new PriceReachedRule.Builder()
                                     .AddMarkets(TradingMarket.Bittrex)
                                     .SetPair(new TradingPair("btc", "eth"))
                                     .SetBoundary(new Price(0.01m), PriceDirection.Up, PriceType.Ask)
                                     .Build();
            Console.WriteLine("Set up default rule (Bittrex, BTC/ETH, will be notified when ask price will higher than 0.001)");
            Action alertCallback = () => Console.WriteLine("Heeyy, we broke boundary, writing you on telegram and viber");
                      
            var subscription = await marketManager.SubscribeRuleAsync(rule, alertCallback);

            return subscription;
        }

        #region Methods for getting user input

        private static TradingMarket[] GetMarketsFromUser()
        {
            Console.WriteLine("Choose trading market(s) (0-bitfinex, 1-bittrex) separated by ',': ");
            var selectedMarkets = Console.ReadLine()
                                    .Split(',')
                                    .Select(n => (TradingMarket)Convert.ToInt32(n))
                                    .ToArray();

            return selectedMarkets;
        }

        private static TradingPair GetPairFromUser()
        {
            Console.WriteLine("Choose base for pair (e.g. 'btc'): ");
            var @base = Console.ReadLine();
            Console.WriteLine("Choose quote for pair (e.g. 'usd' or 'eth'): ");
            var quote = Console.ReadLine();

            return new TradingPair(@base, quote);
        }

        private static Price GetBoundaryFromUser()
        {
            Console.WriteLine("Enter boundary value, e.g. 4.23: ");
            var boundary = (Price) decimal.Parse(Console.ReadLine());

            return boundary;
        }

        private static PriceDirection GetBoundaryDirectionFromUser()
        {
            Console.WriteLine("Enter boundary direction (0 - down, 1 - up): ");
            var direction = (PriceDirection)int.Parse(Console.ReadLine());

            return direction;
        }

        #endregion
    }
}
