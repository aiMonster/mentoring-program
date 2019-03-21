using MentoringProgram.Common.DataStructures;
using MentoringProgram.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MentoringProgram.Tests
{
    [TestClass]
    class StructTest
    {        
        [TestMethod]
        public void InsertDublicateToDictionary()
        {


            //var dictionary = new Dictionary<TestClass, int>();
            //var key1 = new TestClass(TradingMarket.Bitfinex, 1);
            //var key2 = new TestClass(TradingMarket.Bitfinex, 1);


            var dictionary = new Dictionary<MarketsList, int>();
            var key1 = new MarketsList();
            var key2 = new MarketsList(TradingMarket.Bittrex, TradingMarket.Bitfinex);
            //var key3 = new MarketsList(TradingMarket.Bittrex, TradingMarket.Bitfinex);
            key1.Add(TradingMarket.Bitfinex);
            //key1.Add(TradingMarket.Bittrex);
            dictionary.Add(key1, 1);
            dictionary.Add(key2, 2);
            //dictionary.Add(key3, 3);
        }
    }

    //public struct TestClass
    //{
    //    public List <TradingMarket> Markets;
    //    public int Number { get; set; }

    //    public TestClass(TradingMarket market, int num)
    //    {
    //        Markets = new List<TradingMarket>() { market };
    //        Number = num;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        var isValidCast = obj is TestClass;
    //        if (!isValidCast)
    //        {
    //            throw new InvalidCastException();
    //        }

    //        var testClass = (TestClass)obj;
    //        return true;

    //        return base.Equals(obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return 5.GetHashCode();
    //    }
    //}



    //public class MarketsList : IEnumerable<TradingMarket>
    //{
        
    //    private readonly HashSet<TradingMarket> _markets;
    //    public int Count => _markets.Count;

    //    //public MarketsList()
    //    //{
    //    //    _markets = new HashSet<TradingMarket>();
    //    //}

    //    public MarketsList(IEnumerable<TradingMarket> markets)
    //    {
    //        _markets = new HashSet<TradingMarket>(markets);
    //    }

    //    public MarketsList(params TradingMarket[] markets)
    //    {
    //        _markets = new HashSet<TradingMarket>(markets);
    //    }

    //    public void Add(TradingMarket market) => _markets.Add(market);        

    //    public override bool Equals(object obj)
    //    {
    //        if(obj is MarketsList marketList)
    //        {
    //            return _markets.SetEquals(marketList);
    //        }

    //        return false; 
    //    }

    //    public override int GetHashCode()
    //    {
    //        var hashcode = default(int);
    //        foreach(var item in _markets)
    //        {
    //            hashcode = unchecked(hashcode ^ item.GetHashCode());                
    //        }

    //        return hashcode;
    //    }

    //    public IEnumerator<TradingMarket> GetEnumerator() => _markets.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}
}
