using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class MappingRule : GenericRule
    {
        public const string RULE_DEFINITION_TYPE_NAME = "VR_MappingRule";
        public MappingRuleSettings Settings { get; set; }

        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            if (Settings != null && Settings.Value != null)
            {
                var mappingRuleDefinitionSettings = context.RuleDefinitionSettings as MappingRuleDefinitionSettings;
                if (mappingRuleDefinitionSettings != null && mappingRuleDefinitionSettings.FieldType != null)
                    return mappingRuleDefinitionSettings.FieldType.GetDescription(Settings.Value);
            }
            return null;
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            if (Settings == null || Settings.Value == null) return false; // Should a null exception be thrown instead?
            var mappingRuleDefinitionSettings = ruleDefinitionSettings as MappingRuleDefinitionSettings;
            return mappingRuleDefinitionSettings.FieldType.IsMatched(Settings.Value, settingsFilterValue);
        }

        public override bool IsRuleStillValid(IGenericRuleIsRuleStillValidContext context)
        {
            if (Settings != null && Settings.Value != null)
            {
                var mappingRuleDefinitionSettings = context.RuleDefinitionSettings as MappingRuleDefinitionSettings;
                if (mappingRuleDefinitionSettings != null && mappingRuleDefinitionSettings.FieldType != null)
                    return mappingRuleDefinitionSettings.FieldType.IsStillAvailable(new DataRecordFieldTypeIsStillAvailableContext { EntityId = Settings.Value });
            }

            return false;
        }
    }
}
