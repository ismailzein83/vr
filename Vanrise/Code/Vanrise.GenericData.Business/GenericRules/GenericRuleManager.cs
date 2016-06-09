﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;
using Vanrise.Common;
using Vanrise.Entities;
using System.Globalization;

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

        public T GetMatchRule(int ruleDefinitionId, GenericRuleTarget target)
        {
            var ruleTree = GetRuleTree(ruleDefinitionId);
            return ruleTree.GetMatchRule(target) as T;
        }

        public static IEnumerable<Object> GetCriteriaFieldValues(GenericRule rule, string fieldName)
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
            if (rule.Criteria.FieldsValues.TryGetValue(fieldName, out genericRuleCriteriaFieldValues))
            {
                var fieldType = GetCritieriaFieldType(rule, fieldName);

                var fieldRuntimeType = fieldType.GetRuntimeType();

                var values = genericRuleCriteriaFieldValues.GetValues();
                if (values != null)
                    return values.Select(itm => Convert.ChangeType(itm, fieldRuntimeType));
                else
                    return null;
            }
            else
                return null;
        }

        private static DataRecordFieldType GetCritieriaFieldType(GenericRule rule, string fieldName)
        {
            var ruleDefinition = GetRuleDefinition(rule.DefinitionId);

            if (ruleDefinition.CriteriaDefinition == null)
                throw new NullReferenceException(String.Format("ruleDefinition.CriteriaDefinition {0}", rule.DefinitionId));

            if (ruleDefinition.CriteriaDefinition.Fields == null)
                throw new NullReferenceException(String.Format("ruleDefinition.CriteriaDefinition.Fields {0}", rule.DefinitionId));

            var fieldDefinition = ruleDefinition.CriteriaDefinition.Fields.FirstOrDefault(itm => itm.FieldName == fieldName);
            if (fieldDefinition == null)
                throw new NullReferenceException(String.Format("fieldDefinition . Rule Definition Id {0}. Field Name {1}", rule.DefinitionId, fieldName));

            if (fieldDefinition.FieldType == null)
                throw new NullReferenceException(String.Format("fieldDefinition.FieldType . Rule Definition Id {0}. Field Name {1}", rule.DefinitionId, fieldName));
            return fieldDefinition.FieldType;
        }

        private static GenericRuleDefinition GetRuleDefinition(int ruleDefinitionId)
        {
            var ruleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(ruleDefinitionId);

            if (ruleDefinition == null)
                throw new NullReferenceException(String.Format("ruleDefinition {0}", ruleDefinitionId));
            return ruleDefinition;
        }

        public static bool TryGetTargetFieldValue(GenericRuleTarget target, string fieldName, out Object value)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.TargetFieldValues == null)
                throw new ArgumentNullException("target.TargetFieldValues");
            return target.TargetFieldValues.TryGetValue(fieldName, out value);
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

        private RuleTree GetRuleTree(int ruleDefinitionId)
        {
            String cacheName = String.Format("GenericRuleManager<T>_GetRuleTree_{0}", ruleDefinitionId);
            return GetCachedOrCreate(cacheName, () =>
            {
                GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
                GenericRuleDefinition ruleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);
                IEnumerable<T> rules = GetAllRules().FindAllRecords(itm => itm.DefinitionId == ruleDefinitionId);
                return BuildRuleTree(ruleDefinition.CriteriaDefinition, rules);
            });

        }

        public RuleTree BuildRuleTree(GenericRuleDefinitionCriteria ruleDefinitionCriteria, IEnumerable<T> rules)
        {
            List<BaseRuleStructureBehavior> ruleStructureBehaviors = new List<BaseRuleStructureBehavior>();
            foreach (var ruleDefinitionCriteriaField in ruleDefinitionCriteria.Fields.OrderBy(itm => itm.Priority))
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
            behavior.FieldName = ruleDefinitionCriteriaField.FieldName;
            return behavior as BaseRuleStructureBehavior;
        }

        #endregion

        #region Protected Methods

        protected override GenericRuleDetail MapToDetails(T rule)
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
}
