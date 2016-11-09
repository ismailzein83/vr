using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business.BELookupRules.RuleStructureBehaviors
{
    public class BELookupRuleStructureBehaviorByKey : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<Object>, IBELookupRuleStructureBehavior
    {
        public string FieldPath { get; set; }

        protected override void GetKeysFromRule(IVRRule rule, out IEnumerable<object> keys)
        {
            var beLookupRule = rule as BELookupRule;
            if (beLookupRule == null)
                throw new NullReferenceException("beLookupRule");
            Object value = BELookupRuleManager.GetRuleBEFieldValue(beLookupRule, this.FieldPath);
            if (value == null)
                keys = null;
            else
            {
                string valueAsString = value as string;
                if (valueAsString != null)
                    keys = new List<Object> { value };
                else
                {
                    IEnumerable valueAsIEnumerable = value as IEnumerable;
                    if (valueAsIEnumerable != null)
                        keys = valueAsIEnumerable.Cast<Object>();
                    else
                        keys = new List<Object> { value };
                }
            }
        }

        protected override bool TryGetKeyFromTarget(object target, out object key)
        {
            return GenericRuleManager<GenericRule>.TryGetTargetFieldValue(target as GenericRuleTarget, this.FieldPath, out key);
        }

        public override BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new BELookupRuleStructureBehaviorByKey
            {
                FieldPath = this.FieldPath
            };
        }
    }
}
