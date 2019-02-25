using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Interfaces;
using MentoringProgram.Common.Models;
using MentoringProgram.MarketClients;
using System;

namespace MentoringProgram.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started ...");

            TradingMarket market;
            
            Console.WriteLine("Choose traiding market (0-bitfinex): ");
            var marketNum = Convert.ToInt32(Console.ReadLine());
            market = (TradingMarket)marketNum;            

            IMarketClient marketClient;
            switch (market)
            {               
                case TradingMarket.Bitfinex:
                    marketClient = new BitfinexClient();
                    break;
                default:
                    marketClient = new BitfinexClient();
                    break;
            }
            Console.WriteLine("You chose next market: {0}", marketClient.Name);

            TradingPair pair;
            do
            {
                Console.WriteLine("Choose currency pair (e.g. 'btcusd'): ");
                var name = Console.ReadLine();
                pair = marketClient.GetPairByName(name);
            }
            while (pair == null);
            Console.WriteLine("Info for {0}/{1} :", pair.Title, pair.Name);


            Console.WriteLine("Enter boundary (or 'no' if you don't want): ");
            var boundary = Console.ReadLine();

            decimal parsedBoundary;
            if(boundary != "no" && decimal.TryParse(boundary, out parsedBoundary))
            {
                var rule = new TradingRule();
                rule.Pair = pair;
                rule.Boundary = parsedBoundary;
                rule.Callback = () => Console.WriteLine("We broke boundary, sending smth to telegram or viber!");

                marketClient.SetUpAlert(rule, false);
            }

            var previous = default(decimal);
            using (marketClient)
            {               
                var subscriptionResult = marketClient.SubscribeToTraidingPair(pair, (data) =>
                {
                    var current = data.Price;

                    ConsoleColor color;
                    if(previous > current)
                    {
                        color = ConsoleColor.Red;
                    }
                    else if( previous < current)
                    {
                        color = ConsoleColor.Green;
                    }
                    else
                    {
                        color = ConsoleColor.Yellow;
                    }

                    Console.ForegroundColor = color;

                    previous = current;                  
                    Console.WriteLine(current);                    
                });
                Console.ReadLine();               
                marketClient.UnsubscribeTraidingPair(subscriptionResult.Data);               
            }

            Console.WriteLine("Finished ...");            
        }

        
    }
}
