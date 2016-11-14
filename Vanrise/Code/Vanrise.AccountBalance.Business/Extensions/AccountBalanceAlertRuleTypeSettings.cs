using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business.Extensions
{
    public class AccountBalanceAlertRuleTypeSettings : VRBalanceAlertRuleTypeSettings
    {
        public Guid AccountTypeId { get; set; }
        public override string VRActionExtensionType
        {
            get;
            set;
        }

        public override string ThresholdExtensionType
        {
            get;set;
        }

        static AccountBalanceAlertRuleBehavior _behavior = new AccountBalanceAlertRuleBehavior();
        public override VRBalanceAlertRuleBehavior Behavior
        {
            get { return _behavior; }
            set { }
        }
    }
}
