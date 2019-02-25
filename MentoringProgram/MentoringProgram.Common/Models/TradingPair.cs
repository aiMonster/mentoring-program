namespace MentoringProgram.Common.Models
{
    public class TradingPair
    {
        public string Name { get; set; }
        public string Title { get; set; }

        public TradingPair() { }

        public TradingPair(string name, string title)
        {
            Name = name;
            Title = title;
        }

        public TradingPair(string name) : this(name, "Undefined") { }
    }
}
