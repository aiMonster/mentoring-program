using MentoringProgram.Common.Models;
using MentoringProgram.Common.Wrappers;
using MentoringProgram.ExchangeProviders.Fake;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace MentoringProgram.Tests
{
    [TestClass]
    public class AutoResubscribeAndAlwaysOnWrappersTest
    {
        [TestMethod]
        public async Task AutoResubscribeShouldResubscribeAfterDisconnection()
        {
            var testProvider = new FakeProvider();
            var wrappedProvider = testProvider.AttachAutoResubscribeWrapper().AttachAlwaysOn();

            await wrappedProvider.SubscribeAsync(TradingPair.BTCUSD, null);
            testProvider.Disconnect();

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }
    }
}
