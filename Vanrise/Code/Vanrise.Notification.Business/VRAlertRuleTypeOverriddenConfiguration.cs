using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Notification.Data;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("AA2AD3F2-ED37-4212-BE3E-2CC507BC1205"); }
        }

        public Guid RuleTypeId { get; set; }

        public string OverriddenName { get; set; }

        public VRAlertRuleTypeSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(VRAlertRuleTypeOverriddenConfigurationBehavior);
        }

        #region Private Classes

        private class VRAlertRuleTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                VRAlertRuleTypeManager ruleTypeManager = new VRAlertRuleTypeManager();
                List<VRAlertRuleType> ruleTypes = new List<VRAlertRuleType>();
                foreach (var config in context.Configs)
                {
                    VRAlertRuleTypeOverriddenConfiguration ruleTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<VRAlertRuleTypeOverriddenConfiguration>("ruleTypeConfig", config.OverriddenConfigurationId);

                    var ruleType = ruleTypeManager.GetVRAlertRuleType(ruleTypeConfig.RuleTypeId);
                    ruleType.ThrowIfNull("ruleType", ruleTypeConfig.RuleTypeId);
                    ruleType = ruleType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(ruleTypeConfig.OverriddenName))
                        ruleType.Name = ruleTypeConfig.OverriddenName;
                    if (ruleTypeConfig.OverriddenSettings != null)
                        ruleType.Settings = ruleTypeConfig.OverriddenSettings;
                    ruleTypes.Add(ruleType);                    
                }
                GenerateScript(ruleTypes, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<VRAlertRuleTypeOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).RuleTypeId).Distinct();
                VRAlertRuleTypeManager ruleTypeManager = new VRAlertRuleTypeManager();
                List<VRAlertRuleType> ruleTypes = new List<VRAlertRuleType>();
                foreach (var id in ids)
                {
                    var ruleType = ruleTypeManager.GetVRAlertRuleType(id);
                    ruleType.ThrowIfNull("ruleType", id);
                    ruleTypes.Add(ruleType);
                }
                GenerateScript(ruleTypes, context.AddEntityScript);
            } 

            private void GenerateScript( List<VRAlertRuleType> ruleTypes, Action<string, string> addEntityScript)
            {
                IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();
                dataManager.GenerateScript(ruleTypes, addEntityScript);
                List<VRLoggableEntityBase> loggableEntities = new List<VRLoggableEntityBase>();
                foreach (var ruleType in ruleTypes)
                {
                    loggableEntities.Add(new VRAlertRuleManager.VRAlertRuleLoggableEntity(ruleType.VRAlertRuleTypeId));
                }
                string loggableEntityScriptEntityName;
                var loggableEntitiesScript = new Vanrise.Common.Business.VRLoggableEntityManager().GenerateLoggableEntitiesScript(loggableEntities, out loggableEntityScriptEntityName);
                addEntityScript(String.Format("{0} - Alert Rules", loggableEntityScriptEntityName), loggableEntitiesScript);
            }
        }

        #endregion
    }
}
