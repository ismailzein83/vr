using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceAlertRuleGetEntityNameContext : IVRBalanceAlertRuleGetEntityNameContext
    {
        public string EntityId { get; set; }

        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
    }
}
