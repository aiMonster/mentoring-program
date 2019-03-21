using Bitfinex.Net.Objects;
using MentoringProgram.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.ExchangeProviders.Bitfinex.Extensions
{
    internal static class BitfinexMarketOverviewExtension
    {
        public static Candle ToCandle(this BitfinexMarketOverview marketOverview)
        {
           return new Candle((Price)marketOverview.Bid, (Price)marketOverview.Ask);            
        }

    }
}
