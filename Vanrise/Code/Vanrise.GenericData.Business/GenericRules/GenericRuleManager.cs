using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleManager<T> : Vanrise.Rules.RuleManager<T, GenericRuleDetail>, IGenericRuleManager where T : GenericRule
    {
        public IDataRetrievalResult<GenericRuleDetail> GetFilteredRules(DataRetrievalInput<GenericRuleQuery> input)
        {
            IEnumerable<T> allRules = GetAllRules().FindAllRecords(itm => itm.DefinitionId == input.Query.RuleDefinitionId);
            return DataRetrievalManager.Instance.ProcessResult(input, allRules.ToBigResult(input, null, GenericRuleDetailMapper));
        }

        public GenericRule GetGenericRule(int ruleId)
        {
            return GetAllRules().GetRecord(ruleId);
        }

        public T GetMatchRule(int ruleDefinitionId, GenericRuleTarget target)
        {
            var ruleTree = GetRuleTree(ruleDefinitionId);
            return ruleTree.GetMatchRule(target) as T;
        }

        private RuleTree GetRuleTree(int ruleDefinitionId)
        {
            String cacheName = String.Format("GenericRuleManager<T>_GetRuleTree_{0}", ruleDefinitionId);
            return GetCachedOrCreate(cacheName, () =>
            {
                GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
                GenericRuleDefinition ruleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);
                IEnumerable<T> rules = GetAllRules().FindAllRecords(itm => itm.DefinitionId == ruleDefinitionId);
                List<BaseRuleStructureBehavior> ruleStructureBehaviors = new List<BaseRuleStructureBehavior>();
                foreach (var ruleDefinitionCriteriaField in ruleDefinition.CriteriaDefinition.Fields.OrderBy(itm => itm.Priority))
                {
                    BaseRuleStructureBehavior ruleStructureBehavior = CreateRuleStructureBehavior(ruleDefinitionCriteriaField);
                    ruleStructureBehaviors.Add(ruleStructureBehavior);
                }
                return new RuleTree(rules, ruleStructureBehaviors);
            });

        }

        private BaseRuleStructureBehavior CreateRuleStructureBehavior(GenericRuleDefinitionCriteriaField ruleDefinitionCriteriaField)
        {
            GenericRules.RuleStructureBehaviors.IGenericRuleStructureBehavior behavior = null;
            switch (ruleDefinitionCriteriaField.RuleStructureBehaviorType)
            {
                case MappingRuleStructureBehaviorType.ByKey: behavior = new GenericRules.RuleStructureBehaviors.GenericRuleStructureBehaviorByKey(); break;
                case MappingRuleStructureBehaviorType.ByPrefix: behavior = new GenericRules.RuleStructureBehaviors.GenericRuleStructureBehaviorByPrefix(); break;
            }
            behavior.FieldName = ruleDefinitionCriteriaField.FieldName;
            return behavior as BaseRuleStructureBehavior;
        }

        public static IEnumerable<Object> GetCriteriaFieldValues(GenericRule rule, string fieldName)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");
            if (rule.Criteria == null)
                throw new ArgumentNullException("rule.Criteria");
            if (rule.Criteria.FieldsValues == null)
                throw new ArgumentNullException("rule.Criteria.FieldsValues");
            GenericRuleCriteriaFieldValues genericRuleCriteriaFieldValues;
            if (rule.Criteria.FieldsValues.TryGetValue(fieldName, out genericRuleCriteriaFieldValues))
            {
                return genericRuleCriteriaFieldValues.GetValues();
            }
            else
                return null;
        }

        public static bool TryGetTargetFieldValue(GenericRuleTarget target, string fieldName, out Object value)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if(target.TargetFieldValues == null)
                throw new ArgumentNullException("target.TargetFieldValues");
            return target.TargetFieldValues.TryGetValue(fieldName, out value);
        }

        private IEnumerable<GenericRuleDefinitionCriteriaField> GetRuleDefinitionCriteriaFields(int ruleDefinitionId)
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.InsertOperationOutput<GenericRuleDetail> AddGenericRule(GenericRule rule)
        {
            return this.AddRule(rule as T) as Vanrise.Entities.InsertOperationOutput<GenericRuleDetail>;
        }

        private GenericRuleDetail GenericRuleDetailMapper(GenericRule genericRule)
        {
            GenericRuleDetail ruleDetail = new GenericRuleDetail();
            ruleDetail.Entity = genericRule;
            return ruleDetail;
        }

        protected override GenericRuleDetail MapToDetails(T rule)
        {
            return new GenericRuleDetail(){
                Entity = rule
            };
        }
    }
}
