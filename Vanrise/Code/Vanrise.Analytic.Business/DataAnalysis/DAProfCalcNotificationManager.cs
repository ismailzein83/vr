using System;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcNotificationManager
    {
        public Guid GetDAProfCalcNotificationTypeId(Guid alertRuleTypeId, Guid dataAnalysisItemDefinitionId)
        {
            VRAlertRuleTypeManager vrAlertRuleTypeManager = new VRAlertRuleTypeManager();
            VRAlertRuleType alertRuleType = vrAlertRuleTypeManager.GetVRAlertRuleType(alertRuleTypeId);
            DAProfCalcAlertRuleTypeSettings alertRuleSettings = alertRuleType.Settings as DAProfCalcAlertRuleTypeSettings;
            if (alertRuleSettings == null)
                throw new Exception(String.Format("alertRuleType.Settings is not of type DAProfCalcAlertRuleTypeSettings. it is of type '0'", alertRuleSettings.GetType()));

            if (alertRuleSettings.DAProfCalcItemNotifications == null)
                throw new NullReferenceException(string.Format("alertRuleSettings.DAProfCalcItemNotifications is null for alertRuleTypeId:{0}", alertRuleTypeId));

            DAProfCalcItemNotification daProfCalcItemNotification = alertRuleSettings.DAProfCalcItemNotifications.FindRecord(itm => itm.DataAnalysisItemDefinitionId == dataAnalysisItemDefinitionId);
            if (daProfCalcItemNotification == null)
                throw new NullReferenceException(string.Format("daProfCalcItemNotification is null for alertRuleTypeId:{0} and dataAnalysisItemDefinitionId:{1}", alertRuleTypeId, dataAnalysisItemDefinitionId));

            return daProfCalcItemNotification.NotificationTypeId;
        }
    }
}