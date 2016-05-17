using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business
{
    public class BELookupRuleManager
    {
        public dynamic GetMatchBE(int beLookupRuleDefinitionId, GenericRuleTarget ruleTarget)
        {
            RuleTree ruleTree = GetRuleTree(beLookupRuleDefinitionId);
            BELookupRule matchRule = ruleTree.GetMatchRule(ruleTarget) as BELookupRule;
            if (matchRule != null)
                return matchRule.BusinessEntityObject;
            else
                return null;
        }

        private RuleTree GetRuleTree(int beLookupRuleDefinitionId)
        {
            BELookupRuleDefinition beLookupRuleDefinition = GetRuleDefinition(beLookupRuleDefinitionId);
            List<dynamic> allEntities = null;
            List<BELookupRule> beLookupRules = new List<BELookupRule>();
            foreach(var entity in allEntities)
            {
                beLookupRules.Add(new BELookupRule
                    {
                        RuleDefinition = beLookupRuleDefinition,
                        BusinessEntityObject = entity
                    });
            }
            List<BaseRuleStructureBehavior> ruleStructureBehaviors = new List<BaseRuleStructureBehavior>();
            foreach (var ruleDefinitionCriteriaField in beLookupRuleDefinition.CriteriaFields)
            {
                BaseRuleStructureBehavior ruleStructureBehavior = CreateRuleStructureBehavior(ruleDefinitionCriteriaField);
                ruleStructureBehaviors.Add(ruleStructureBehavior);
            }
            return new RuleTree(beLookupRules, ruleStructureBehaviors);
        }

        private BELookupRuleDefinition GetRuleDefinition(int beLookupRuleDefinitionId)
        {
            throw new NotImplementedException();
        }

        private BaseRuleStructureBehavior CreateRuleStructureBehavior(BELookupRuleDefinitionCriteriaField ruleDefinitionCriteriaField)
        {
            IBELookupRuleStructureBehavior behavior = null;
            switch (ruleDefinitionCriteriaField.RuleStructureBehaviorType)
            {
                case MappingRuleStructureBehaviorType.ByKey: behavior = new BELookupRuleStructureBehaviorByKey(); break;
                case MappingRuleStructureBehaviorType.ByPrefix: behavior = new BELookupRuletructureBehaviorByPrefix(); break;
            }
            behavior.FieldPath = ruleDefinitionCriteriaField.FieldPath;
            return behavior as BaseRuleStructureBehavior;
        }

        public static Object GetRuleBEFieldValue(BELookupRule rule, string fieldPath)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");
            Type beType = rule.BusinessEntityObject.GetType();
            string[] propertyParts = fieldPath.Split('.');
            Object value = rule.BusinessEntityObject;
            foreach (var propPart in propertyParts)
            {
                value = value.GetType().GetProperty(propPart).GetValue(value);
                if (value == null)
                    break;
            }
            return value;
        }
    }

    public interface IBELookupRuleStructureBehavior
    {
        string FieldPath { set; }
    }

    public class BELookupRuleStructureBehaviorByKey : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<Object>, IBELookupRuleStructureBehavior
    {
        public string FieldPath { get; set; }

        protected override void GetKeysFromRule(BaseRule rule, out IEnumerable<object> keys)
        {
            var beLookupRule = rule as BELookupRule;
            if (beLookupRule == null)
                throw new NullReferenceException("beLookupRule");
            Object value = BELookupRuleManager.GetRuleBEFieldValue(beLookupRule, this.FieldPath);
            if (value == null)
                keys = null;
            else
            {
                IEnumerable<object> valueAsIEnumerable = value as IEnumerable<object>;
                if (valueAsIEnumerable != null)
                    keys = valueAsIEnumerable;
                else
                    keys = new List<Object> { value };
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

    public class BELookupRuletructureBehaviorByPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix, IBELookupRuleStructureBehavior
    {
        public string FieldPath { get; set; }

        protected override void GetPrefixesFromRule(BaseRule rule, out System.Collections.Generic.IEnumerable<string> prefixes)
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
