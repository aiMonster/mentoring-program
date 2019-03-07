using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.MarketClients;
using System;
using System.Linq;

namespace MentoringProgram.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started ...");
            
            var marketManager = new MarketManager();

            SubscribeAndDisplay(marketManager);

            SubscribeAndSetUpAlert(marketManager);

            Console.ReadLine();
            Console.WriteLine("Finished ...");            
        }

        private static void SubscribeAndDisplay(MarketManager marketManager)
        {
            var previous = default(decimal);
            Action<TradeUpdate> callback = delegate (TradeUpdate update)
            {                
                var current = update.Price.Value;

                ConsoleColor color;
                if (previous > current)
                {
                    color = ConsoleColor.Red;
                }
                else if (previous < current)
                {
                    color = ConsoleColor.Green;
                }
                else
                {
                    color = ConsoleColor.Yellow;
                }

                Console.ForegroundColor = color;

                previous = current;
                Console.WriteLine($"Price: {current}");
            };

            var resp = marketManager.Subscribe(GetPairFromUser(), callback, GetMarketsFromUser());

            //marketManager.AddMarketsToSubscription(resp, TradingMarket.Bitfinex, TradingMarket.Bitfinex);
            //marketManager.RemoveMarketsFromSubscription(resp, TradingMarket.Binance);
        }

        private static void SubscribeAndSetUpAlert(MarketManager marketManager)
        {
            var rule = new TradingRule.Builder()
                                     .AddMarkets(GetMarketsFromUser())
                                     .SetPair(GetPairFromUser())
                                     .SetBoundary(GetBoundaryFromUser(), GetBoundaryDirectionFromUser())
                                     .Build();

            Action alertCallback = delegate
            {
                Console.WriteLine("Heeyy, we broke boundary, writing you on telegram and viber");
            };
            var alertSubscripitonId = marketManager.Subscribe(rule, alertCallback);
        }

        #region Methods for getting user input

        private static TradingMarket[] GetMarketsFromUser()
        {
            Console.WriteLine("Choose trading market(s) (0-bitfinex, 1-binance) separated by ',': ");
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
            Console.WriteLine("Choose quote for pair (e.g. 'usd'): ");
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

        //OBSOLETE
        private static TradingRule GetRuleInfoFromUser()
        {
            var ruleBuilder = new TradingRule.Builder();

            Console.WriteLine("Choose trading market(s) (0-bitfinex, 1-binance) separated by ',': ");
            var selectedMarkets = Console.ReadLine()
                                    .Split(',')
                                    .Select(n => (TradingMarket)Convert.ToInt32(n))
                                    .ToArray();
            ruleBuilder.AddMarkets(selectedMarkets);

            Console.WriteLine("Choose base for pair (e.g. 'btc'): ");
            var @base = Console.ReadLine();
            Console.WriteLine("Choose quote for pair (e.g. 'usd'): ");
            var quote = Console.ReadLine();
            ruleBuilder.SetPair(new TradingPair(@base, quote));

            Console.WriteLine("Enter boundary (or 'no' if you don't want): ");
            var boundary = Console.ReadLine();
            decimal parsedBoundary;
            if (boundary != "no" && decimal.TryParse(boundary, out parsedBoundary))
            {
                ruleBuilder.SetBoundary((Price)parsedBoundary, PriceDirection.Down);
            }

            return ruleBuilder.Build();
        }

        #endregion
    }
}
