using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Business
{
    class DAProfCalcVRAlertRuleTypeFilter : IVRAlertRuleTypeFilter
    {
        public bool IsMatch(VRAlertRuleType alertRuleType)
        {
            if (alertRuleType == null)
                throw new NullReferenceException("alertRuleType");

            if (alertRuleType.Settings == null)
                throw new NullReferenceException("alertRuleType.Settings");

            if (alertRuleType.Settings.ConfigId != DAProfCalcAlertRuleTypeSettings.s_ConfigId)
                return false;
            if (!new DAProfCalcNotificationManager().DoesUserHaveStartSpecificInstanceAccess(ContextFactory.GetContext().GetLoggedInUserId(), alertRuleType.VRAlertRuleTypeId))
                return false;
            return true;
        }
    }
}
