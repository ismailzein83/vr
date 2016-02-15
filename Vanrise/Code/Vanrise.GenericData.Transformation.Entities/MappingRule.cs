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
        public MappingRuleSettings Settings { get; set; }

        public override string GetSettingsDescription(GenericRuleDefinitionSettings settingsDefinition)
        {
            if (Settings != null && Settings.Value != null)
            {
                var mappingRuleDefinitionSettings = settingsDefinition as MappingRuleDefinitionSettings;
                if (mappingRuleDefinitionSettings != null && mappingRuleDefinitionSettings.FieldType != null)
                    return mappingRuleDefinitionSettings.FieldType.GetDescription(Settings.Value);
            }
            return null;
        }
    }
}
