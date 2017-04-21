using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcNotificationManager
    {
        static VRAlertRuleTypeManager s_vrAlertRuleTypeManager = new VRAlertRuleTypeManager();

        static SecurityManager s_securityManager =  new SecurityManager();

        public Guid GetDAProfCalcNotificationTypeId(Guid alertRuleTypeId, Guid dataAnalysisItemDefinitionId)
        {
            VRAlertRuleType alertRuleType = s_vrAlertRuleTypeManager.GetVRAlertRuleType(alertRuleTypeId);
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
        public DAProfCalcAlertRuleTypeSettings GetProfCalcAlertRuleTypeSettings(Guid alertRuleTypeId)
        {
            var alertRuleType = GetVRAlertRuleDAProfCalc(alertRuleTypeId);
            DAProfCalcAlertRuleTypeSettings alertRuleSettings = alertRuleType.Settings as DAProfCalcAlertRuleTypeSettings;
            if (alertRuleSettings == null)
                throw new Exception(String.Format("alertRuleType.Settings is not of type DAProfCalcAlertRuleTypeSettings. it is of type '0'", alertRuleSettings.GetType()));

            return alertRuleSettings;
        }
        public RequiredPermissionSettings GetViewInstanceRequiredPermissions(Guid alertRuleTypeId)
        {
            var alertRuleTypeSettings = GetProfCalcAlertRuleTypeSettings(alertRuleTypeId);
            return alertRuleTypeSettings.DAProfCalcSecurity.ViewPermission;
        }
        public bool DoesUserHaveViewAccess(int userId)
        {
            var daprofCalc = GetCachedVRAlertRulesDAProfCalc();
            foreach (var dap in daprofCalc)
            {
                var settings =  GetProfCalcAlertRuleTypeSettings(dap.Key);
                if (DoesUserHaveAccess(userId,settings, (sec) => sec.ViewPermission))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveStartNewInstanceAccess(int userId)
        {
            var daprofCalc = GetCachedVRAlertRulesDAProfCalc();
            foreach (var dap in daprofCalc)
            {
                var settings = GetProfCalcAlertRuleTypeSettings(dap.Key);
                if (DoesUserHaveAccess(userId, settings, (sec) => sec.StartInstancePermission))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveStartSpecificInstanceAccess(int userId, Guid alertRuleTypeId)
        {
            var settings = GetProfCalcAlertRuleTypeSettings(alertRuleTypeId);
            return DoesUserHaveAccess(userId, settings, (sec) => sec.StartInstancePermission);
        }

        public VRAlertRuleType GetVRAlertRuleDAProfCalc(Guid alertRuleTypeId)
        {
            return this.GetCachedVRAlertRulesDAProfCalc().GetRecord(alertRuleTypeId);
        }
        private Dictionary<Guid, VRAlertRuleType> GetCachedVRAlertRulesDAProfCalc()
        {
            return s_vrAlertRuleTypeManager.GetCachedOrCreate("DAProfCalcNotificationManager_GetCachedVRAlertRuleDAProfCalc", () =>
            {
                Dictionary<Guid, VRAlertRuleType> rules = new Dictionary<Guid, VRAlertRuleType>();
                 var allAlertRuleType = s_vrAlertRuleTypeManager.GetCachedVRAlertRuleTypes().Values;
                 foreach (var a in allAlertRuleType)
                 {
                      DAProfCalcAlertRuleTypeSettings alertRuleSettings = a.Settings as DAProfCalcAlertRuleTypeSettings;
                      if (alertRuleSettings != null)
                          rules.Add(a.VRAlertRuleTypeId, a);
                 }
                 return rules;
             
             });
        }
        private bool DoesUserHaveAccess(int userId, DAProfCalcAlertRuleTypeSettings daProfCalcAlertRuleTypeSettings, Func<DAProfCalcAlertRuleTypeSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            if (daProfCalcAlertRuleTypeSettings != null && daProfCalcAlertRuleTypeSettings.DAProfCalcSecurity != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(daProfCalcAlertRuleTypeSettings.DAProfCalcSecurity), userId);
            return true;
        }
    }
}