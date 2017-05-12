using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B2903DAC-1980-4C21-82FB-8DCC72E5068D"); }
        }

        public Guid RuleDefinitionId { get; set; }

        public string OverriddenTitle { get; set; }

        public GenericRuleDefinitionCriteria OverriddenCriteriaDefinition { get; set; }

        public VRObjectVariableCollection OverriddenObjects { get; set; }

        public GenericRuleDefinitionSettings OverriddenSettingsDefinition { get; set; }

        public GenericRuleDefinitionSecurity OverriddenSecurity { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(GenericRuleDefinitionOverriddenConfigurationBehavior);
        }

        #region Private Classes

        private class GenericRuleDefinitionOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
                List<VRLoggableEntityBase> loggableEntities = new List<VRLoggableEntityBase>();
                foreach (var config in context.Configs)
                {
                    GenericRuleDefinitionOverriddenConfiguration ruleDefinitionConfig = config.Settings.ExtendedSettings.CastWithValidate<GenericRuleDefinitionOverriddenConfiguration>("ruleDefinitionConfig", config.OverriddenConfigurationId);

                    var ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionConfig.RuleDefinitionId);
                    ruleDefinition.ThrowIfNull("ruleDefinition", ruleDefinitionConfig.RuleDefinitionId);
                    ruleDefinition = ruleDefinition.VRDeepCopy();
                    if (!String.IsNullOrEmpty(ruleDefinitionConfig.OverriddenTitle))
                    {
                        ruleDefinition.Name = ruleDefinitionConfig.OverriddenTitle;
                        ruleDefinition.Title = ruleDefinitionConfig.OverriddenTitle;
                    }
                    if (ruleDefinitionConfig.OverriddenCriteriaDefinition != null)
                        ruleDefinition.CriteriaDefinition = ruleDefinitionConfig.OverriddenCriteriaDefinition;
                    if (ruleDefinitionConfig.OverriddenObjects != null)
                        ruleDefinition.Objects = ruleDefinitionConfig.OverriddenObjects;
                    if (ruleDefinitionConfig.OverriddenSettingsDefinition != null)
                        ruleDefinition.SettingsDefinition = ruleDefinitionConfig.OverriddenSettingsDefinition;
                    if (ruleDefinitionConfig.OverriddenSecurity != null)
                        ruleDefinition.Security = ruleDefinitionConfig.OverriddenSecurity;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", ruleDefinition.GenericRuleDefinitionId, ruleDefinition.Name, Serializer.Serialize(ruleDefinition));
                    loggableEntities.Add(new GenericRuleManager<GenericRule>.GenericRuleLoggableEntity(ruleDefinitionConfig.RuleDefinitionId));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Details]))
merge	[genericdata].[GenericRuleDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Name],[Details])
	values(s.[ID],s.[Name],s.[Details]);", scriptBuilder);
                context.AddEntityScript("[genericdata].[GenericRuleDefinition]", script);
                string loggableEntityScriptEntityName;
                var loggableEntitiesScript = new Vanrise.Common.Business.VRLoggableEntityManager().GenerateLoggableEntitiesScript(loggableEntities, out loggableEntityScriptEntityName);
                context.AddEntityScript(String.Format("{0} - Generic Rules", loggableEntityScriptEntityName), loggableEntitiesScript);
            }
        }

        #endregion
    }
}
