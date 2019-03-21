namespace MentoringProgram.Common.Models
{
    public struct TradeUpdate
    {       
        public TradingPair Pair { get; }        
        public Candle CandlePrice { get;}

        public TradeUpdate(TradingPair pair, Candle candlePrice)
        {
            Pair = pair;
            CandlePrice = candlePrice;
        }
    }
}
