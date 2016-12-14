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
        public override Guid ConfigId
        {
            get { return new Guid("49200E05-8AB8-4E28-9AB1-B0699CD32258"); }
        }
        public Guid AccountTypeId { get; set; }
        public override string VRActionExtensionType
        {
            get;
            set;
        }

        public override string ThresholdExtensionType
        {
            get;
            set;
        }

        static AccountBalanceAlertRuleBehavior _behavior = new AccountBalanceAlertRuleBehavior();
        public override VRBalanceAlertRuleBehavior Behavior
        {
            get { return _behavior; }
            set { }
        }
    }
}
