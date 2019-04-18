using MentoringProgram.Common.Models.SubscriptionIds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MentoringProgram.Common.Models.Subscriptions
{
    public class ClientMarketSubscriptions
    {
        public List<ClientSubscription> ClientSubscriptions { get; } = new List<ClientSubscription>();
        public List<MarketSubscription> MarketSubscriptions { get; } = new List<MarketSubscription>();

        public bool ContainsRuleSubscription(RuleSubscriptionGuid ruleSubscriptionId) =>
            ClientSubscriptions.Any(s => s.Subscription.Id == ruleSubscriptionId);

        public ClientSubscription GetClientSubscription(RuleSubscriptionGuid ruleSubscriptionId) =>
            ClientSubscriptions.FirstOrDefault(s => s.Subscription.Id == ruleSubscriptionId);
    }    
}
