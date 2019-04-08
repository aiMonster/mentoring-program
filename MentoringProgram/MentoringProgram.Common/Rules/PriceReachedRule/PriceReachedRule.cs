using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Rules.PriceReachedRule.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Rules.PriceReachedRule
{
    public class PriceReachedRule : BaseRule
    {
        public Price Boundary { get; private set; }
        public PriceDirection PriceDirection { get; private set; }
        public PriceType PriceType { get; private set; }

        public override bool IsConditionMet(TradeUpdate tradeUpdate)
        {
            switch (PriceDirection)
            {
                case PriceDirection.Down:
                    return tradeUpdate.CandlePrice.Ask < Boundary ? true : false;
                case PriceDirection.Up:
                    return tradeUpdate.CandlePrice.Ask > Boundary ? true : false;
                default:
                    return false;
            }
        }

        public class Builder
        {
            PriceReachedRule rule = new PriceReachedRule();

            public Builder() { }

            public Builder(PriceReachedRule fromRule)
            {
                rule = fromRule;
            }

            public Builder SetPair(TradingPair pair)
            {
                rule.Pair = pair;
                return this;
            }

            public Builder SetBoundary(Price boundary, PriceDirection direction, PriceType priceType)
            {
                rule.Boundary = boundary;
                rule.PriceDirection = direction;
                rule.PriceType = priceType;
                return this;
            }

            public Builder AddMarkets(params TradingMarket[] tradingMarkets)
            {
                rule.TradingMarkets.AddRange(tradingMarkets);
                return this;
            }

            public PriceReachedRule Build() => rule;
        }
    }
}
