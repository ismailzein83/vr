using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleManager<T> : Vanrise.Rules.RuleManager<T> where T : GenericRule
    {
        public T GetMatchRule(int ruleDefinitionId, GenericRuleTarget target)
        {
            var ruleTree = GetRuleTree(ruleDefinitionId);
            return ruleTree.GetMatchRule(target) as T;
        }

        private RuleTree GetRuleTree(int ruleDefinitionId)
        {
            GenericRuleDefinition ruleDefinition = null;
            IEnumerable<T> rules = null;
            List<BaseRuleStructureBehavior> ruleStructureBehaviors = new List<BaseRuleStructureBehavior>();
            foreach (var ruleDefinitionCriteriaField in ruleDefinition.CriteriaDefinition.Fields)
            {
                BaseRuleStructureBehavior ruleStructureBehavior = CreateRuleStructureBehavior(ruleDefinitionCriteriaField);
                ruleStructureBehaviors.Add(ruleStructureBehavior);
            }
            return new RuleTree(rules, ruleStructureBehaviors);
        }

        private BaseRuleStructureBehavior CreateRuleStructureBehavior(GenericRuleDefinitionCriteriaField ruleDefinitionCriteriaField)
        {
            GenericRules.RuleStructureBehaviors.IGenericRuleStructureBehavior behavior = null;
            switch (ruleDefinitionCriteriaField.RuleStructureBehaviorType)
            {
                case MappingRuleStructureBehaviorType.ByKey: behavior = new GenericRules.RuleStructureBehaviors.GenericRuleStructureBehaviorByKey(); break;
                case MappingRuleStructureBehaviorType.ByPrefix: behavior = new GenericRules.RuleStructureBehaviors.GenericRuleStructureBehaviorByPrefix(); break;
            }
            behavior.Field = ruleDefinitionCriteriaField;
            return behavior as BaseRuleStructureBehavior;
        }

        public static bool TryGetTargetFieldValue(GenericRuleTarget target, GenericRuleDefinitionCriteriaField ruleDefinitionCriteriaField, out Object value)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (ruleDefinitionCriteriaField == null)
                throw new ArgumentNullException("ruleDefinitionCriteriaField");
            if (String.IsNullOrEmpty(ruleDefinitionCriteriaField.FieldName))
                throw new ArgumentNullException("ruleDefinitionCriteriaField.FieldName");
            if (String.IsNullOrEmpty(ruleDefinitionCriteriaField.FieldDataRecordName))
                throw new ArgumentNullException("ruleDefinitionCriteriaField.FieldDataRecordName");
            if(target.DataRecords == null)
                throw new ArgumentNullException("target.DataRecords");
            DataRecord dataRecord;
            if (!target.DataRecords.TryGetValue(ruleDefinitionCriteriaField.FieldDataRecordName, out dataRecord))
                throw new Exception(String.Format("Data Record '{0}' is not available in target.DataRecords", ruleDefinitionCriteriaField.FieldDataRecordName));
            return dataRecord.FieldsValues.TryGetValue(ruleDefinitionCriteriaField.FieldName, out value);
        }
    }
}
