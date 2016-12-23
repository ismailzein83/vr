using System;
using Vanrise.Common;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceAlertRuleTypeFilter : IVRAlertRuleTypeFilter
    {
        Guid _configId { get { return new Guid("ba79cb79-d058-4382-88fc-db1c154b5374"); } }
        public bool IsMatch(VRAlertRuleType alertRuleType)
        {
            alertRuleType.ThrowIfNull("alertRuleType", "");
            alertRuleType.Settings.ThrowIfNull("alertRuleType.Settings", "");

            if (alertRuleType.Settings.ConfigId != _configId)
                return false;

            return true;
        }
    }
}
