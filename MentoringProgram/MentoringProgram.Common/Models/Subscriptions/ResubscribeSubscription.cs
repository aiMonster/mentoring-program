using System;
using System.Collections.Generic;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ResubscribeSubscription
    {
        public Subscription ProviderSubscription { get; set; }
        public Subscription ClientSubscription { get; set; }
        public Action<TradeUpdate> Callback { get; set; }
    }
}
