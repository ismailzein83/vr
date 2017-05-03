using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

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
    }
}
