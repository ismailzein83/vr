﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class MappingRule : GenericRule
    {
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
            if (settingsFilterValue != null)
            {
                var mappingRuleDefinitionSettings = ruleDefinitionSettings as MappingRuleDefinitionSettings;
                object settingsValue = (Settings.Value != null) ? new List<object> { Settings.Value } : null;
                return mappingRuleDefinitionSettings.FieldType.IsMatched(settingsValue, settingsFilterValue);
            }
            return true;
        }
    }
}
