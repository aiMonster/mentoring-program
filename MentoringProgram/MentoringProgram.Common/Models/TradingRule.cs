using MentoringProgram.Common.DataStructures;
using MentoringProgram.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MentoringProgram.Common.Models
{    
    public class TradingRule
    {
        public MarketsList TradingMarkets { get; private set; } = new MarketsList();
        public TradingPair Pair { get; private set; }
        public Price Boundary { get; private set; }
        public PriceDirection PriceDirection { get; private set; }       
        public PriceType PriceType { get; private set; }

        private TradingRule() { }

        public class Builder
        {
            TradingRule rule = new TradingRule();

            public Builder() { }

            public Builder(TradingRule fromRule)
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

            public TradingRule Build()
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
