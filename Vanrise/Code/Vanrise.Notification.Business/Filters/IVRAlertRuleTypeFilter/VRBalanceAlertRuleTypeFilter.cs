using System;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.Notification.Business
{
    public class VRBalanceAlertRuleTypeFilter : IVRAlertRuleTypeFilter
    {
        public bool IsMatch(VRAlertRuleType alertRuleType)
        {
            alertRuleType.ThrowIfNull("alertRuleType", alertRuleType.VRAlertRuleTypeId);
            alertRuleType.Settings.ThrowIfNull("alertRuleType.Settings", alertRuleType.VRAlertRuleTypeId);

            var vrBalanceAlertRuleTypeSettings = alertRuleType.Settings as VRBalanceAlertRuleTypeSettings;
            if (vrBalanceAlertRuleTypeSettings == null)
                return false;

            return true;
        }
    }
}
