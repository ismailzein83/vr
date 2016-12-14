using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRBalanceAlertRuleTypeSettings : VRGenericAlertRuleTypeSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string VRActionExtensionType { get; set; }

        public abstract string ThresholdExtensionType { get; set; }

        public abstract VRBalanceAlertRuleBehavior Behavior { get; set; }

        public override string SettingEditor
        {
            get
            {
                return "vr-notification-vrbalancealertrule-settings";
            }
        }
    }


}