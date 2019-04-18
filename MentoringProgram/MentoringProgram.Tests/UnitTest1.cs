using System;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MentoringProgram.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ThrowsExceptionForInvalidInput()
        {      
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new TradingPair("", "usd");
            }); 
        }

        [TestMethod]
        public void ThrowsExceptionForEmptyInput()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new TradingPair();
            });
        }

        [TestMethod]
        public void PairNameIsNotEmptyForInvalidData()
        {
            var pair = new TradingPair();
            Assert.AreNotEqual(pair.Name, string.Empty);
        }

        [TestMethod]
        public void PairNameIsNotEmptyForValidData()
        {
            var pair = new TradingPair("btc", "usd");
            Assert.AreNotEqual(pair.Name, string.Empty);
        }

        [TestMethod]
        public void BuilderShouldThrowException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                //How to handle input structures?
                //var rule = new PriceReachedRule.Builder().SetPair(TradingPair.BTCUSD).Build();
                //var rule2 = new TradingRule.Builder(rule).SetPair(new TradingPair()).Build();
            });
        }
    }
}
