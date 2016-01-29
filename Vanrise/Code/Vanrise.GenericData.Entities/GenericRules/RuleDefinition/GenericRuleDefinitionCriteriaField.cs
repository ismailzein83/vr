using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public enum MappingRuleStructureBehaviorType { ByKey, ByPrefix }

    public class GenericRuleDefinitionCriteriaField
    {
        public string FieldName { get; set; }

        public string Title { get; set; }

        public DataRecordFieldType FieldType { get; set; }

        public MappingRuleStructureBehaviorType RuleStructureBehaviorType { get; set; }

        public int Priority { get; set; }
    }
}
