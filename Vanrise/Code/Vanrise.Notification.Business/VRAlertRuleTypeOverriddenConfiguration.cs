using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Common;

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
                StringBuilder scriptBuilder = new StringBuilder();
                VRAlertRuleTypeManager ruleTypeManager = new VRAlertRuleTypeManager();
                List<VRLoggableEntityBase> loggableEntities = new List<VRLoggableEntityBase>();
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
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", ruleType.VRAlertRuleTypeId, ruleType.Name, Serializer.Serialize(ruleType.Settings));
                    loggableEntities.Add(new VRAlertRuleManager.VRAlertRuleLoggableEntity(ruleTypeConfig.RuleTypeId));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VRNotification].[VRAlertRuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
                context.AddEntityScript("[VRNotification].[VRAlertRuleType]", script);
                string loggableEntityScriptEntityName;
                var loggableEntitiesScript = new Vanrise.Common.Business.VRLoggableEntityManager().GenerateLoggableEntitiesScript(loggableEntities, out loggableEntityScriptEntityName);
                context.AddEntityScript(String.Format("{0} - Alert Rules", loggableEntityScriptEntityName), loggableEntitiesScript);
            }
        }

        #endregion
    }
}
