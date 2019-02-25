using System;

namespace MentoringProgram.Common.Models
{
    public class TradingRule
    {
        public TradingPair Pair { get; set; }
        public decimal Boundary { get; set; }
        public Action Callback { get; set; } 
    }
}
