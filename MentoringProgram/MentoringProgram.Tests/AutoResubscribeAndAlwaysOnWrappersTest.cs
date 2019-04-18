using MentoringProgram.Common.Models;
using MentoringProgram.Common.Wrappers;
using MentoringProgram.Tests.ExchangeProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MentoringProgram.Tests
{
    [TestClass]
    public class AutoResubscribeAndAlwaysOnWrappersTest
    {
        [TestMethod]
        public void ShouldHaveOneSubscription()
        {
            var testProvider = new TestExchangeProvider();
            var wrappedProvider = testProvider.AttachAutoResubscribeWrapper().AttachAlwaysOn();

            wrappedProvider.Subscribe(TradingPair.BTCUSD, null);
            testProvider.Disconnect();

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }
    }
}
