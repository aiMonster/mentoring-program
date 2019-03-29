using MentoringProgram.Common.DataStructures;
using MentoringProgram.Common.Models;

namespace MentoringProgram.Common.Rules
{
    public abstract class BaseRule
    {
        public MarketsList TradingMarkets { get; protected set; } = new MarketsList();
        public TradingPair Pair { get; protected set; }

        public abstract bool IsConditionMet(TradeUpdate tradeUpdate);
    }
}
