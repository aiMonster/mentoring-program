using MentoringProgram.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.ExchangeProviders.Bitfinex.Extensions
{
    internal static class TradingPairExtension
    {
        public static string ToBitfinexPair(this TradingPair pair)
        {
            return "t" + pair.Name.ToUpper();
        }
    }
}
