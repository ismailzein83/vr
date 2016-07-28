using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleDefinition
    {
        public int GenericRuleDefinitionId { get; set; }

        public string Name { get; set; }

        public GenericRuleDefinitionCriteria CriteriaDefinition { get; set; }

        public VRObjectVariableCollection Objects { get; set; }

        public GenericRuleDefinitionSettings SettingsDefinition { get; set; }
    }
}
