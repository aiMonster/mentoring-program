using MentoringProgram.Common.Models;
using MentoringProgram.Common.Models.SubscriptionIds;
using MentoringProgram.Common.Models.Subscriptions;
using System;
using System.Threading.Tasks;

namespace MentoringProgram.Common.Interfaces
{
    public interface IExchangeProvider : IDisposable
    {
        event Action OnDisconnected;
        void Connect();
        Task<ResponseResult<Subscription>> SubscribeAsync(TradingPair pair, Action<TradeUpdate> callback);
        Task UnsubscribeAsync(PairSubscriptionGuid pairSubscriptionId);
        Candle GetCurrentCandlePrice(TradingPair pair);
    }
}
