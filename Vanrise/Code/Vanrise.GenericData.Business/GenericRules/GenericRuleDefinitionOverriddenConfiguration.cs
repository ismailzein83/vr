using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Data;

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
                GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
                List<GenericRuleDefinition> ruleDefinitions = new List<GenericRuleDefinition>();
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
                    ruleDefinitions.Add(ruleDefinition);
                    
                }
                GenerateScript(ruleDefinitions, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<GenericRuleDefinitionOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).RuleDefinitionId).Distinct();
                GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
                List<GenericRuleDefinition> ruleDefinitions = new List<GenericRuleDefinition>();
                foreach (var id in ids)
                {
                    var ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(id);
                    ruleDefinition.ThrowIfNull("ruleDefinition", id);
                    ruleDefinitions.Add(ruleDefinition);
                }
                GenerateScript(ruleDefinitions, context.AddEntityScript);
            } 

            private void GenerateScript(List<GenericRuleDefinition> ruleDefinitions, Action<string, string> addEntityScript)
            {
                IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
                dataManager.GenerateScript(ruleDefinitions, addEntityScript);
                List<VRLoggableEntityBase> loggableEntities = new List<VRLoggableEntityBase>();
                foreach (var ruleDefinition in ruleDefinitions)
                {
                    loggableEntities.Add(new GenericRuleManager<GenericRule>.GenericRuleLoggableEntity(ruleDefinition.GenericRuleDefinitionId));
                }
                string loggableEntityScriptEntityName;
                var loggableEntitiesScript = new Vanrise.Common.Business.VRLoggableEntityManager().GenerateLoggableEntitiesScript(loggableEntities, out loggableEntityScriptEntityName);
                addEntityScript(String.Format("{0} - Generic Rules", loggableEntityScriptEntityName), loggableEntitiesScript);
            }
        }

        #endregion
    }
}
