using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;
using Vanrise.Common;
using Vanrise.Entities;
using System.Globalization;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleManager<T> : Vanrise.Rules.RuleManager<T, GenericRuleDetail>, IGenericRuleManager where T : GenericRule
    {
        #region Public Methods

        public IDataRetrievalResult<GenericRuleDetail> GetFilteredRules(DataRetrievalInput<GenericRuleQuery> input)
        {
            var ruleDefinition = GetRuleDefinition(input.Query.RuleDefinitionId);


            Func<T, bool> filterExpression = (rule) => rule.DefinitionId == input.Query.RuleDefinitionId
                && (string.IsNullOrEmpty(input.Query.Description) || (!string.IsNullOrEmpty(rule.Description) && rule.Description.IndexOf(input.Query.Description, StringComparison.OrdinalIgnoreCase) >= 0))
                && (!input.Query.EffectiveDate.HasValue || (rule.BeginEffectiveTime <= input.Query.EffectiveDate.Value && (!rule.EndEffectiveTime.HasValue || input.Query.EffectiveDate.Value < rule.EndEffectiveTime)))
                && (input.Query.CriteriaFieldValues == null || RuleCriteriaFilter(rule, ruleDefinition, input.Query.CriteriaFieldValues))
                && (input.Query.SettingsFilterValue == null || RuleSettingsFilter(rule, ruleDefinition, input.Query.SettingsFilterValue));

            var allRules = GetAllRules();
            return DataRetrievalManager.Instance.ProcessResult(input, allRules.ToBigResult(input, filterExpression, (rule) => MapToDetails(rule)));
        }

        public GenericRule GetGenericRule(int ruleId)
        {
            return GetAllRules().GetRecord(ruleId);
        }

        private struct GetGenericRulesByDefinitionIdCacheName
        {
            public Guid RuleDefinitionId { get; set; }
        }

        public IEnumerable<GenericRule> GetGenericRulesByDefinitionId(Guid ruleDefinitionId)
        {
            var cacheName = new GetGenericRulesByDefinitionIdCacheName { RuleDefinitionId = ruleDefinitionId };
            return GetCachedOrCreate(cacheName, () =>
            {
                GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
                GenericRuleDefinition ruleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);
                return GetAllRules().FindAllRecords(itm => itm.DefinitionId == ruleDefinitionId);
            });
        }

        public T GetMatchRule(Guid ruleDefinitionId, GenericRuleTarget target)
        {
            var ruleTree = GetRuleTree(ruleDefinitionId);
            var criteriaEvaluationInfos = GetCachedCriteriaEvaluationInfos(ruleDefinitionId);
            return GetMatchRule<T>(ruleTree, criteriaEvaluationInfos, target);
        }

        //private static DataRecordFieldType GetCritieriaFieldType(GenericRuleDefinitionCriteria ruleCriteriaDefinition, string fieldName)
        //{
        //    var ruleDefinition = GetRuleDefinition(rule.DefinitionId);

        //    if (ruleDefinition.CriteriaDefinition == null)
        //        throw new NullReferenceException(String.Format("ruleDefinition.CriteriaDefinition {0}", rule.DefinitionId));

        //    if (ruleCriteriaDefinition.Fields == null)
        //        throw new NullReferenceException(String.Format("ruleCriteriaDefinition.Fields {0}", rule.DefinitionId));

        //    var fieldDefinition = ruleDefinition.CriteriaDefinition.Fields.FirstOrDefault(itm => itm.FieldName == fieldName);
        //    if (fieldDefinition == null)
        //        throw new NullReferenceException(String.Format("fieldDefinition . Rule Definition Id {0}. Field Name {1}", rule.DefinitionId, fieldName));

        //    if (fieldDefinition.FieldType == null)
        //        throw new NullReferenceException(String.Format("fieldDefinition.FieldType . Rule Definition Id {0}. Field Name {1}", rule.DefinitionId, fieldName));
        //    return fieldDefinition.FieldType;
        //}

        private static GenericRuleDefinition GetRuleDefinition(Guid ruleDefinitionId)
        {
            var ruleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(ruleDefinitionId);

            if (ruleDefinition == null)
                throw new NullReferenceException(String.Format("ruleDefinition {0}", ruleDefinitionId));
            return ruleDefinition;
        }

        public Vanrise.Entities.InsertOperationOutput<GenericRuleDetail> AddGenericRule(GenericRule rule)
        {
            return this.AddRule(rule as T) as Vanrise.Entities.InsertOperationOutput<GenericRuleDetail>;
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericRuleDetail> UpdateGenericRule(GenericRule rule)
        {
            return this.UpdateRule(rule as T) as Vanrise.Entities.UpdateOperationOutput<GenericRuleDetail>;
        }

        public DeleteOperationOutput<GenericRuleDetail> DeleteGenericRule(int ruleId)
        {
            return this.DeleteRule(ruleId) as Vanrise.Entities.DeleteOperationOutput<GenericRuleDetail>;
        }

        #endregion

        #region Static Methods

        public static Q GetMatchRule<Q>(RuleTree ruleTree, List<CriteriaEvaluationInfo> criteriaEvaluationInfos, GenericRuleTarget target) where Q : class, IVRRule, IGenericRule
        {
            if (target == null)
                throw new ArgumentNullException("target");
            FillTargetEvaluatedCriterias(target, criteriaEvaluationInfos);
            return ruleTree.GetMatchRule(target) as Q;
        }

        public static IEnumerable<Object> GetCriteriaFieldValues(IGenericRule rule, GenericRuleDefinitionCriteriaField genericRuleDefinitionCriteriaField)
        {
            if (rule == null)
                throw new ArgumentNullException("rule");
            if (rule.Criteria == null || rule.Criteria.FieldsValues == null)
                return null;
            //if (rule.Criteria == null)
            //    throw new ArgumentNullException("rule.Criteria");
            //if (rule.Criteria.FieldsValues == null)
            //    throw new ArgumentNullException("rule.Criteria.FieldsValues");
            GenericRuleCriteriaFieldValues genericRuleCriteriaFieldValues;
            if (rule.Criteria.FieldsValues.TryGetValue(genericRuleDefinitionCriteriaField.FieldName, out genericRuleCriteriaFieldValues))
            {
                var fieldType = genericRuleDefinitionCriteriaField.FieldType;// GetCritieriaFieldType(rule, fieldName);

                var fieldRuntimeType = fieldType.GetNonNullableRuntimeType();

                var values = genericRuleCriteriaFieldValues.GetValues();
                if (values != null)
                    return values.Select(itm => Convert.ChangeType(itm, fieldRuntimeType));
                else
                    return null;
            }
            else
                return null;
        }

        public static bool TryGetTargetFieldValue(GenericRuleTarget target, string fieldName, out Object value)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.TargetFieldValues == null)
                throw new ArgumentNullException("target.TargetFieldValues");
            return target.TargetFieldValues.TryGetValue(fieldName, out value);
        }

        public static RuleTree BuildRuleTree<Q>(GenericRuleDefinitionCriteria ruleDefinitionCriteria, IEnumerable<Q> rules) where Q : class, IVRRule, IGenericRule
        {
            List<BaseRuleStructureBehavior> ruleStructureBehaviors = new List<BaseRuleStructureBehavior>();
            foreach (var ruleDefinitionCriteriaField in ruleDefinitionCriteria.Fields.OrderBy(itm => itm.Priority))
            {
                BaseRuleStructureBehavior ruleStructureBehavior = CreateRuleStructureBehavior(ruleDefinitionCriteriaField);
                ruleStructureBehaviors.Add(ruleStructureBehavior);
            }
            return new RuleTree(rules, ruleStructureBehaviors);
        }

        public static List<CriteriaEvaluationInfo> BuildCriteriaEvaluationInfos(GenericRuleDefinitionCriteria criteriaDefinition, VRObjectVariableCollection objects)
        {
            List<CriteriaEvaluationInfo> ruleCriteriaEvaluationInfos = new List<CriteriaEvaluationInfo>();

            foreach (var criteriaField in criteriaDefinition.Fields)
            {
                if (criteriaField.ValueObjectName != null || criteriaField.ValueEvaluator != null)
                {
                    if (criteriaField.ValueObjectName == null)
                        throw new NullReferenceException("criteriaField.ValueObjectName");
                    if (criteriaField.ValueEvaluator == null)
                        throw new NullReferenceException("criteriaField.ValueEvaluator");
                    if (objects == null)
                        throw new NullReferenceException("objects");
                    VRObjectVariable objectVariable;
                    if (!objects.TryGetValue(criteriaField.ValueObjectName, out objectVariable))
                        throw new NullReferenceException(String.Format("objectVariable '{0}'", criteriaField.ValueObjectName));
                    if (objectVariable.ObjectType == null)
                        throw new NullReferenceException(String.Format("objectVariable.ObjectType '{0}'", criteriaField.ValueObjectName));
                    ruleCriteriaEvaluationInfos.Add(new CriteriaEvaluationInfo
                    {
                        CriteriaName = criteriaField.FieldName,
                        ObjectName = criteriaField.ValueObjectName,
                        ObjectType = objectVariable.ObjectType,
                        PropertyEvaluator = criteriaField.ValueEvaluator
                    });
                }
            }
            return ruleCriteriaEvaluationInfos;
        }

        #endregion
        
        #region Private Methods

        bool RuleCriteriaFilter(GenericRule rule, GenericRuleDefinition ruleDefinition, Dictionary<string, object> filterValues)
        {
            if (rule.Criteria == null) return false;

            foreach (KeyValuePair<string, object> filter in filterValues)
            {
                if (filter.Value == null)
                    continue;

                DataRecordFieldType criteriaFieldType = ruleDefinition.CriteriaDefinition.Fields.MapRecord(itm => itm.FieldType, itm => itm.FieldName == filter.Key);
                if (criteriaFieldType == null)
                    throw new NullReferenceException("criteriaFieldType");

                GenericRuleCriteriaFieldValues criteriaFieldValue;
                rule.Criteria.FieldsValues.TryGetValue(filter.Key, out criteriaFieldValue);

                if (criteriaFieldValue != null)
                {
                    IEnumerable<object> values = criteriaFieldValue.GetValues();
                    if (values == null)
                        return false;

                    if (!criteriaFieldType.IsMatched(values, filter.Value))
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        bool RuleSettingsFilter(GenericRule rule, GenericRuleDefinition ruleDefinition, object settingsFilterValue)
        {
            return rule.AreSettingsMatched(ruleDefinition.SettingsDefinition, settingsFilterValue);
        }

        private struct GetRuleTreeCacheName
        {
            public Guid RuleDefinitionId { get; set; }
        }
        private RuleTree GetRuleTree(Guid ruleDefinitionId)
        {
            var cacheName = new GetRuleTreeCacheName { RuleDefinitionId = ruleDefinitionId };// String.Concat("GenericRuleManager<T>_GetRuleTree_", ruleDefinitionId);
            return GetCachedOrCreate(cacheName, () =>
            {
                GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
                GenericRuleDefinition ruleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);
                IEnumerable<GenericRule> rules = this.GetGenericRulesByDefinitionId(ruleDefinitionId);
                return BuildRuleTree(ruleDefinition.CriteriaDefinition, rules);
            });

        }

        private static BaseRuleStructureBehavior CreateRuleStructureBehavior(GenericRuleDefinitionCriteriaField ruleDefinitionCriteriaField)
        {
            GenericRules.RuleStructureBehaviors.IGenericRuleStructureBehavior behavior = null;
            switch (ruleDefinitionCriteriaField.RuleStructureBehaviorType)
            {
                case MappingRuleStructureBehaviorType.ByKey: behavior = new GenericRules.RuleStructureBehaviors.GenericRuleStructureBehaviorByKey(); break;
                case MappingRuleStructureBehaviorType.ByPrefix: behavior = new GenericRules.RuleStructureBehaviors.GenericRuleStructureBehaviorByPrefix(); break;
            }
            behavior.GenericRuleDefinitionCriteriaField = ruleDefinitionCriteriaField;
            return behavior as BaseRuleStructureBehavior;
        }

        private static void FillTargetEvaluatedCriterias(GenericRuleTarget target, List<CriteriaEvaluationInfo> ruleCriteriaEvaluationInfos)
        {
            if (target.TargetFieldValues == null)
                target.TargetFieldValues = new Dictionary<string, object>();
            if (ruleCriteriaEvaluationInfos != null)
            {
                foreach (var criteriaEvaluationInfo in ruleCriteriaEvaluationInfos)
                {
                    if (target.TargetFieldValues.ContainsKey(criteriaEvaluationInfo.CriteriaName))
                        continue;
                    dynamic obj;
                    if (!target.Objects.TryGetValue(criteriaEvaluationInfo.ObjectName, out obj))
                        throw new NullReferenceException(String.Format("obj. ObjectName '{0}'", criteriaEvaluationInfo.ObjectName));
                    var propertyEvaluatorContext = new VRObjectPropertyEvaluatorContext
                    {
                        Object = obj,
                        ObjectType = criteriaEvaluationInfo.ObjectType
                    };
                    target.TargetFieldValues.Add(criteriaEvaluationInfo.CriteriaName, criteriaEvaluationInfo.PropertyEvaluator.GetPropertyValue(propertyEvaluatorContext));
                }
            }
        }

        private struct CriteriaEvaluationInfosCacheName
        {
            public Guid RuleDefinitionId { get; set; }
        }
        private List<CriteriaEvaluationInfo> GetCachedCriteriaEvaluationInfos(Guid ruleDefinitionId)
        {
            var cacheName = new CriteriaEvaluationInfosCacheName { RuleDefinitionId = ruleDefinitionId };// String.Concat("GetCachedCriteriaEvaluationInfos_", ruleDefinitionId);
            return GetCachedOrCreate(cacheName, () =>
            {
                var ruleDefinition = GetRuleDefinition(ruleDefinitionId);
                if (ruleDefinition.CriteriaDefinition != null && ruleDefinition.CriteriaDefinition.Fields != null)
                {
                    return BuildCriteriaEvaluationInfos(ruleDefinition.CriteriaDefinition, ruleDefinition.Objects);
                }
                else
                {
                    return null;
                }
            });
        }
        
        #endregion
        
        #region Protected Methods

        public override GenericRuleDetail MapToDetails(T rule)
        {
            GenericRuleDefinition ruleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(rule.DefinitionId);

            List<string> descriptions = new List<string>();
            if (rule.Criteria != null && rule.Criteria.FieldsValues != null)
            {
                foreach (var criteriaField in ruleDefinition.CriteriaDefinition.Fields)
                {
                    GenericRuleCriteriaFieldValues fieldValues = null;
                    rule.Criteria.FieldsValues.TryGetValue(criteriaField.FieldName, out fieldValues);
                    descriptions.Add((fieldValues != null) ? criteriaField.FieldType.GetDescription(fieldValues) : null);
                }
            }

            return new GenericRuleDetail()
            {
                Entity = rule,
                FieldValueDescriptions = descriptions,
                SettingsDescription = rule.GetSettingsDescription(new GenericRuleSettingsDescriptionContext() { RuleDefinitionSettings = ruleDefinition.SettingsDefinition })
            };
        }

        #endregion
    }

    public class CriteriaEvaluationInfo
    {
        public string CriteriaName { get; set; }

        public string ObjectName { get; set; }

        public VRObjectType ObjectType { get; set; }

        public VRObjectPropertyEvaluator PropertyEvaluator { get; set; }
    }
}
