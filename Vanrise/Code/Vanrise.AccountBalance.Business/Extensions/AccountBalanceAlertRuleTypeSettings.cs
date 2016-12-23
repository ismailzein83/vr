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
        public override Guid NotificationTypeId
        {
            get
            {
                return new Guid("987F1100-3498-4338-BE98-9EBE115D0A58");
            }

        }
        public override Guid ConfigId
        {
            get { return new Guid("ba79cb79-d058-4382-88fc-db1c154b5374"); }
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
