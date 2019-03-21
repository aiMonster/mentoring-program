﻿using System;

namespace MentoringProgram.Common.Models
{
    public struct TradingPair
    {
        public string Base { get; }
        public string Quote { get; }
        public string Name { get { return Base + Quote; } }

        public TradingPair(string @base, string quote)
        {
            //TODO: Need validation on correct pair, e.g. bbc/dds - not valid
            if(string.IsNullOrWhiteSpace(@base))
            {
                throw new ArgumentOutOfRangeException(nameof(@base));
            }

            if (string.IsNullOrWhiteSpace(quote))
            {
                throw new ArgumentOutOfRangeException(nameof(quote));
            }
           
            Base = @base;
            Quote = quote;
        }

        public TradingPair UseBase(string @base) =>        
            new TradingPair(@base, Quote);

        public TradingPair UseQuote(string quote) =>
            new TradingPair(Base, quote);

        public override bool Equals(object obj)
        {
            var isValidCast = obj is TradingPair;
            if (!isValidCast)
            {
                throw new InvalidCastException();
            }

            var pair = (TradingPair)obj;

            return Name.Equals(pair.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.ToLower().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Base.ToUpper()}/{Quote.ToUpper()}";
        }

        public static TradingPair BTCUSD
        {
            get {  return new TradingPair("btc", "usd"); }
        }
    }
}
