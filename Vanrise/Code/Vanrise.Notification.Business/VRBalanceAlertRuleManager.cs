using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
namespace Vanrise.Notification.Business
{
    public class VRBalanceAlertRuleManager : VRGenericAlertRuleManager
    {
        public IEnumerable<VRBalanceAlertRuleThresholdConfig> GetVRBalanceAlertThresholdConfigs(string extensionType)
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<VRBalanceAlertRuleThresholdConfig>(extensionType);
        }
        public string GetBalanceAlertRuleSettingDescription(long alertRuleId)
        {
            var abalanceAlertRules = GetBalanceAlertRulesActionsDescription();
            return abalanceAlertRules.GetRecord(alertRuleId);
        }


        #region Private Methods

        private Dictionary<long, string> GetBalanceAlertRulesActionsDescription()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<VRAlertRuleManager.CacheManager>().GetOrCreateObject("GetBalanceAlertRulesActionsDescription",
               () =>
               {
                   VRAlertRuleManager vrAlertRuleManager = new VRAlertRuleManager();
                   var alertRules = vrAlertRuleManager.GetCachedVRAlertRules();
                   Dictionary<long, string> balanceAlertRulesActionsByRuleId = new Dictionary<long, string>();
                   if (alertRules != null)
                   {
                       foreach (var alertRule in alertRules)
                       {

                           var alertRuleSettings = alertRule.Value.Settings.ExtendedSettings as VRBalanceAlertRuleSettings;
                           if (alertRuleSettings != null)
                           {
                               StringBuilder ruleActionsDescription = new StringBuilder();
                               foreach (var thresholdAction in alertRuleSettings.ThresholdActions)
                               {
                                   if (ruleActionsDescription.Length > 0)
                                   {
                                       ruleActionsDescription.Append(",");
                                   }
                                   foreach (var action in thresholdAction.Actions)
                                   {
                                       ruleActionsDescription.AppendFormat("{0} ", action.ActionName);
                                       if (thresholdAction.ThresholdDescription != null)
                                       {
                                           ruleActionsDescription.AppendFormat("({0}) ", thresholdAction.ThresholdDescription);
                                       }
                                   }
                               }
                               balanceAlertRulesActionsByRuleId.Add(alertRule.Key, ruleActionsDescription.ToString());
                           }
                        
                       }
                   }
                   return balanceAlertRulesActionsByRuleId;
               });
        }
        #endregion
    }
}
