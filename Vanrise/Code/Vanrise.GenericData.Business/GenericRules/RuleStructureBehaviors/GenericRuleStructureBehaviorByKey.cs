using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business.GenericRules.RuleStructureBehaviors
{
    public class GenericRuleStructureBehaviorByKey : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<Object>, IGenericRuleStructureBehavior
    {
        public GenericRuleDefinitionCriteriaField Field { get; set; }

        protected override void GetKeysFromRule(BaseRule rule, out IEnumerable<object> keys)
        {
            GenericRule genericRule = rule as GenericRule;
            List<Object> fieldValues;
            genericRule.Criteria.FieldsValues.TryGetValue(this.Field.FieldName, out fieldValues);
            keys = fieldValues;
        }

        protected override bool TryGetKeyFromTarget(object target, out object key)
        {
            return GenericRuleManager<GenericRule>.TryGetTargetFieldValue(target as GenericRuleTarget, this.Field, out key);
        }
    }
}
