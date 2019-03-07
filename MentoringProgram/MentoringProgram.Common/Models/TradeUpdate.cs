namespace MentoringProgram.Common.Models
{
    public class TradeUpdate
    {       
        public TradingPair Pair { get; }        
        public Price Price { get;}

        public TradeUpdate(TradingPair pair, Price price)
        {
            Pair = pair;
            Price = price;
        }
    }
}
