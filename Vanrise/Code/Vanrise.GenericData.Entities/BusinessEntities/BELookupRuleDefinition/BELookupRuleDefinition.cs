using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BELookupRuleDefinition
    {
        public int BELookupRuleDefinitionId { get; set; }

        public string Name { get; set; }

        public Guid BusinessEntityDefinitionId { get; set; }

        public List<BELookupRuleDefinitionCriteriaField> CriteriaFields { get; set; }
    }

    public class BELookupRuleDefinitionCriteriaField
    {
        public string Title { get; set; }

        public string FieldPath { get; set; }

        public MappingRuleStructureBehaviorType RuleStructureBehaviorType { get; set; }
    }

    public class BELookupRule : Vanrise.Rules.BaseRule
    {
        public BELookupRuleDefinition RuleDefinition { get; set; }

        public dynamic BusinessEntityObject { get; set; }
    }
}
