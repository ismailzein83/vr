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
        public GenericRuleDefinitionCriteriaField GenericRuleDefinitionCriteriaField { get; set; }

        protected override void GetPrefixesFromRule(IVRRule rule, out System.Collections.Generic.IEnumerable<string> prefixes)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");
            IGenericRule genericRule = rule as IGenericRule;
            if (genericRule == null)
                throw new Exception(String.Format("rule is not of type IGenericRule. it is of type '{0}'", rule.GetType()));
            IEnumerable<Object> fieldValues = GenericRuleManager<GenericRule>.GetCriteriaFieldValues(genericRule, this.GenericRuleDefinitionCriteriaField);
            if (fieldValues != null)
                prefixes = fieldValues.Select(itm => itm as string);
            else
                prefixes = null;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            GenericRuleTarget genericRuleTarget = target as GenericRuleTarget;
            if (genericRuleTarget == null)
                throw new Exception(String.Format("target is not of type GenericRuleTarget. it is of type '{0}'", target.GetType()));
            object fieldValue;
            if (GenericRuleManager<GenericRule>.TryGetTargetFieldValue(genericRuleTarget, this.GenericRuleDefinitionCriteriaField.FieldName, out fieldValue))
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
                GenericRuleDefinitionCriteriaField = this.GenericRuleDefinitionCriteriaField
            };
        }
    }
}
