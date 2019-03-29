using MentoringProgram.Common.Models;
using System;

namespace MentoringProgram.ExchangeProviders.Bittrex.Extensions
{
    internal static class TradingPairExtension
    {
        public static TradingPair ToTradingPair(this string marketName)
        {
            try
            {
                var pairParts = marketName.Split('-');
                var pair = new TradingPair(pairParts[0], pairParts[1]);
                return pair;
            }
            catch
            {
                throw new ArgumentOutOfRangeException();
            }
            
        }

        public static string ToBittrexPair(this TradingPair pair)
        {
            return pair.Base.ToUpper() + "-" + pair.Quote.ToUpper();
        }
    }
}
