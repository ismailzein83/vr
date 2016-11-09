using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business.BELookupRules.RuleStructureBehaviors
{
    public class BELookupRuletructureBehaviorByPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix, IBELookupRuleStructureBehavior
    {
        public string FieldPath { get; set; }

        protected override void GetPrefixesFromRule(IVRRule rule, out System.Collections.Generic.IEnumerable<string> prefixes)
        {
            var beLookupRule = rule as BELookupRule;
            if (beLookupRule == null)
                throw new NullReferenceException("beLookupRule");
            Object value = BELookupRuleManager.GetRuleBEFieldValue(beLookupRule, this.FieldPath);
            if (value == null)
                prefixes = null;
            else
            {
                IEnumerable<string> valueAsIEnumerable = value as IEnumerable<string>;
                if (valueAsIEnumerable != null)
                    prefixes = valueAsIEnumerable;
                else
                    prefixes = new List<string> { value as string };
            }
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            object fieldValue;
            if (GenericRuleManager<GenericRule>.TryGetTargetFieldValue(target as GenericRuleTarget, this.FieldPath, out fieldValue))
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
            return new BELookupRuletructureBehaviorByPrefix
            {
                FieldPath = this.FieldPath
            };
        }
    }
}
