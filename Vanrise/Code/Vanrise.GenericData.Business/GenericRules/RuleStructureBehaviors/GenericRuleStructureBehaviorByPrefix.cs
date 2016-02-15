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
        public string FieldName { get; set; }

        protected override void GetPrefixesFromRule(BaseRule rule, out System.Collections.Generic.IEnumerable<string> prefixes)
        {
            IEnumerable<Object> fieldValues = GenericRuleManager<GenericRule>.GetCriteriaFieldValues(rule as GenericRule, this.FieldName);
            if (fieldValues != null)
                prefixes = fieldValues.Select(itm => itm as string);
            else
                prefixes = null;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            object fieldValue;
            if (GenericRuleManager<GenericRule>.TryGetTargetFieldValue(target as GenericRuleTarget, this.FieldName, out fieldValue))
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

        public override BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new GenericRuleStructureBehaviorByPrefix
            {
                FieldName = this.FieldName
            };
        }
    }
}
