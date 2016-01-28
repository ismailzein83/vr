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
        public string FieldName { get; set; }

        protected override void GetKeysFromRule(BaseRule rule, out IEnumerable<object> keys)
        {
            keys = GenericRuleManager<GenericRule>.GetCriteriaFieldValues(rule as GenericRule, this.FieldName);
        }

        protected override bool TryGetKeyFromTarget(object target, out object key)
        {
            return GenericRuleManager<GenericRule>.TryGetTargetFieldValue(target as GenericRuleTarget, this.FieldName, out key);
        }
    }
}
