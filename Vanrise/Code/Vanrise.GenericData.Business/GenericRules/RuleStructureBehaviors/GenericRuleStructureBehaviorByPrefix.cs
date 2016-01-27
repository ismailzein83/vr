using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business.GenericRules.RuleStructureBehaviors
{
    public class GenericRuleStructureBehaviorByPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix, IGenericRuleStructureBehavior
    {
        public GenericRuleDefinitionCriteriaField Field { get; set; }

        protected override void GetPrefixesFromRule(BaseRule rule, out System.Collections.Generic.IEnumerable<string> prefixes)
        {
            GenericRule genericRule = rule as GenericRule;
            List<Object> fieldValues;
            if (genericRule.Criteria.FieldsValues.TryGetValue(this.Field.FieldName, out fieldValues) && fieldValues != null)
                prefixes = fieldValues.Select(itm => itm as string);
            else
                prefixes = null;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            object fieldValue;
            if (GenericRuleManager<GenericRule>.TryGetTargetFieldValue(target as GenericRuleTarget, this.Field, out fieldValue))
            {
                value = fieldValue as string;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
