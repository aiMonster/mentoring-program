namespace MentoringProgram.Common.Models
{
    public struct Candle
    { 
        public Price Bid { get; }
        public Price Ask { get; }

        public Candle(Price bid, Price ask)
        {
            Bid = bid;
            Ask = ask;
        }
    }
}
