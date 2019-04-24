using Microsoft.VisualStudio.TestTools.UnitTesting;
using MentoringProgram.Common.Models;
using MentoringProgram.Common.Wrappers;
using MentoringProgram.ExchangeProviders.Fake;
using System.Threading.Tasks;

namespace MentoringProgram.Tests
{
    [TestClass]
    public class DuplicatesWrapperTest
    {
        [TestMethod]
        public async Task WithoutDuplicatesWrapperShouldCreateTwoSubscriptions()
        {
            var testProvider = new FakeProvider();
            await testProvider.SubscribeAsync(TradingPair.BTCUSD, null);
            await testProvider.SubscribeAsync(TradingPair.BTCUSD, null);
           
            var expected = 2;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }

        [TestMethod]
        public async Task WithDuplicatesWrapperShouldCreateOnlyOneSubscription()
        {
            var testProvider = new FakeProvider();   
            var wrappedProvider = testProvider.AttachSubscriptionDublicatesWrapper();

            await wrappedProvider.SubscribeAsync(TradingPair.BTCUSD, null);
            await wrappedProvider.SubscribeAsync(TradingPair.BTCUSD, null);

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }

        [TestMethod]
        public async Task WrapperShouldDisposeOnlyOneSubscription()
        {
            var testProvider = new FakeProvider();
            var wrappedProvider = testProvider.AttachSubscriptionDublicatesWrapper();

            await wrappedProvider.SubscribeAsync(TradingPair.BTCUSD, null);

            var subscription = await wrappedProvider.SubscribeAsync(TradingPair.BTCUSD, null);
            subscription.Data.Dispose();

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }

        [TestMethod]
        public async Task ProviderShouldDisposeOnlyOneSubscription()
        {
            var testProvider = new FakeProvider();

            await testProvider.SubscribeAsync(TradingPair.BTCUSD, null);

            var subscription = await testProvider.SubscribeAsync(TradingPair.BTCUSD, null);
            subscription.Data.Dispose();

            var expected = 1;
            var real = testProvider.SubscriptionsCount;

            Assert.AreEqual(expected, real);
        }
    }
}
