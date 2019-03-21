using Bittrex.Net.Objects;
using MentoringProgram.Common.Models;

namespace MentoringProgram.ExchangeProviders.Bittrex.Extensions
{
    internal static class BittrexStreamMarketSummaryExtension
    {
        public static TradeUpdate ToTradeUpdate(this BittrexStreamMarketSummary summary)
        {            
            var pair = summary.MarketName.ToTradingPair();
            var candle = new Candle((Price)summary.Bid, (Price)summary.Ask);

            return new TradeUpdate(pair, candle);
        }
    }
}
