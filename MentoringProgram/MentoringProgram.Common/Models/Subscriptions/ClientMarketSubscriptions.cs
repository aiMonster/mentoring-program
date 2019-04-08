using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ClientMarketSubscriptions
    {
        public List<ClientSubscription> ClientSubscriptions { get; } = new List<ClientSubscription>();
        public List<MarketSubscription> MarketSubscriptions { get; } = new List<MarketSubscription>();
    }    
}
