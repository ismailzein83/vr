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
        public GenericRuleDefinitionCriteriaField GenericRuleDefinitionCriteriaField { get; set; }

        protected override void GetKeysFromRule(IVRRule rule, out IEnumerable<object> keys)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");
            IGenericRule genericRule = rule as IGenericRule;
            if (genericRule == null)
                throw new Exception(String.Format("rule is not of type IGenericRule. it is of type '{0}'", rule.GetType()));
            keys = GenericRuleManager<GenericRule>.GetCriteriaFieldValues(genericRule, this.GenericRuleDefinitionCriteriaField);
        }

        protected override bool TryGetKeyFromTarget(object target, out object key)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            GenericRuleTarget genericRuleTarget = target as GenericRuleTarget;
            if (genericRuleTarget == null)
                throw new Exception(String.Format("target is not of type GenericRuleTarget. it is of type '{0}'", target.GetType()));
            return GenericRuleManager<GenericRule>.TryGetTargetFieldValue(genericRuleTarget, this.GenericRuleDefinitionCriteriaField.FieldName, out key);
        }

        public override BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new GenericRuleStructureBehaviorByKey
            {
                GenericRuleDefinitionCriteriaField = this.GenericRuleDefinitionCriteriaField
            };
        }
    }
}
