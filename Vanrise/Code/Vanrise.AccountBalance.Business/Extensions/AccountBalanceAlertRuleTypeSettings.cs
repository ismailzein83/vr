using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business.Extensions
{
    public class AccountBalanceAlertRuleTypeSettings : VRBalanceAlertRuleTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("ba79cb79-d058-4382-88fc-db1c154b5374"); } }

        public override Guid NotificationTypeId { get; set; }

        public Guid AccountTypeId { get; set; }

        public override string ThresholdExtensionType { get; set; }


        static AccountBalanceAlertRuleBehavior _behavior = new AccountBalanceAlertRuleBehavior();
        public override VRBalanceAlertRuleBehavior Behavior
        {
            get { return _behavior; }
            set { }
        }

        public override VRActionTargetType GetActionTargetType()
        {
            var accountTypeManager = new AccountTypeManager();
            return accountTypeManager.GetAccountTypeExtendedSettings<AccountTypeExtendedSettings>(this.AccountTypeId).GetActionTargetType();
        }
    }
}
