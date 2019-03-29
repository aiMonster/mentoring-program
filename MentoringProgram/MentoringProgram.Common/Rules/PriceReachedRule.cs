using MentoringProgram.Common.Enums;
using MentoringProgram.Common.Models;
using System;

namespace MentoringProgram.Common.Rules
{
    public class PriceReachedRule : BaseRule
    {
        public Price Boundary { get; private set; }
        public PriceDirection PriceDirection { get; private set; }
        public PriceType PriceType { get; private set; }     

        public override bool IsConditionMet(TradeUpdate tradeUpdate)
        {
            if (PriceDirection == PriceDirection.Down)
            {
                if (tradeUpdate.CandlePrice.Ask < Boundary)
                {
                    return true;
                }
            }
            else if (PriceDirection == PriceDirection.Up)
            {
                if (tradeUpdate.CandlePrice.Ask > Boundary)
                {
                    return true;
                }
            }

            return false;
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

            public PriceReachedRule Build()
            {
                //TODO: validate all invalid fields
                if (string.IsNullOrWhiteSpace(rule.Pair.Name))
                {
                    throw new ArgumentOutOfRangeException(nameof(rule.Pair.Name));
                }

                return rule;
            }
        }
    }
}
