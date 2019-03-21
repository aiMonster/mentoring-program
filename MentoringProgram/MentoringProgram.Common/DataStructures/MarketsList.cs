using MentoringProgram.Common.Enums;
using System.Collections;
using System.Collections.Generic;

namespace MentoringProgram.Common.DataStructures
{
    public class MarketsList : IEnumerable<TradingMarket>
    {
        private readonly HashSet<TradingMarket> _markets;

        public int Count => _markets.Count;
               
        public MarketsList(IEnumerable<TradingMarket> markets)
        {
            _markets = new HashSet<TradingMarket>(markets);
        }

        public MarketsList(params TradingMarket[] markets)
        {
            _markets = new HashSet<TradingMarket>(markets);
        }

        public void Add(TradingMarket market) => _markets.Add(market);

        public void AddRange(IEnumerable<TradingMarket> markets)
        {
            foreach(var market in markets)
            {
                Add(market);
            }
        }

        public TradingMarket[] ToArray()
        {
            var markets = new TradingMarket[Count];
            _markets.CopyTo(markets);
            return markets;
        }

        public override bool Equals(object obj)
        {
            if (obj is MarketsList marketList)
            {
                return _markets.SetEquals(marketList);
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hashcode = default(int);
            foreach (var item in _markets)
            {
                hashcode = unchecked(hashcode ^ item.GetHashCode());
            }

            return hashcode;
        }

        public IEnumerator<TradingMarket> GetEnumerator() => _markets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
