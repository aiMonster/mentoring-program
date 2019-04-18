using MentoringProgram.Tests.ExchangeProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Wrappers;

namespace MentoringProgram.Tests
{
    [TestClass]
    public class DuplicatesWrapperTest
    {
        [TestMethod]
        public void ShouldCreateTwoSubscriptions()
        {
            var testProvider = new TestExchangeProvider();
            testProvider.Subscribe(TradingPair.BTCUSD, null);
            testProvider.Subscribe(TradingPair.BTCUSD, null);
           
            var expected = 2;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }

        [TestMethod]
        public void ShouldCreateOnlyOneSubscription()
        {
            var testProvider = new TestExchangeProvider();   
            var wrappedProvider = testProvider.AttachSubscriptionDublicatesWrapper();

            wrappedProvider.Subscribe(TradingPair.BTCUSD, null);
            wrappedProvider.Subscribe(TradingPair.BTCUSD, null);

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }

        [TestMethod]
        public void WrapperShouldDisposeOnlyOneSubscription()
        {
            var testProvider = new TestExchangeProvider();
            var wrappedProvider = testProvider.AttachSubscriptionDublicatesWrapper();

            wrappedProvider.Subscribe(TradingPair.BTCUSD, null);

            var subscription = wrappedProvider.Subscribe(TradingPair.BTCUSD, null);
            subscription.Data.Dispose();

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }

        [TestMethod]
        public void ProviderShouldDisposeOnlyOneSubscription()
        {
            var testProvider = new TestExchangeProvider();

            testProvider.Subscribe(TradingPair.BTCUSD, null);

            var subscription = testProvider.Subscribe(TradingPair.BTCUSD, null);
            subscription.Data.Dispose();

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }
    }
}
