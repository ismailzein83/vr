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
using System.ComponentModel;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleManager<T> : Vanrise.Rules.RuleManager<T, GenericRuleDetail>, IGenericRuleManager where T : GenericRule
    {
        static GenericRuleManager()
        {
            GenericRuleManager<T> instance = new GenericRuleManager<T>();
            instance.AddRuleCachingExpirationChecker(new GenericRuleCachingExpirationChecker());
        }

        #region Public Methods

        public IDataRetrievalResult<GenericRuleDetail> GetFilteredRules(DataRetrievalInput<GenericRuleQuery> input)
        {
            List<T> filteredRules = GetApplicableFilteredRules(input.Query.RuleDefinitionId, input.Query);
            ResultProcessingHandler<GenericRuleDetail> handler = new ResultProcessingHandler<GenericRuleDetail>()
            {
                ExportExcelHandler = new GenericRuleExcelExportHandler(input.Query)
            };
            VRActionLogger.Current.LogGetFilteredAction(new GenericRuleLoggableEntity(input.Query.RuleDefinitionId), input);
            return DataRetrievalManager.Instance.ProcessResult(input, filteredRules.ToBigResult(input, null, (rule) => MapToDetails(rule)), handler);
        }

        public List<T> GetApplicableFilteredRules(Guid ruleDefinitionId, GenericRuleQuery query, bool withOverridenRules = false)
        {
            var ruleDefinition = GetRuleDefinition(ruleDefinitionId);

            Func<T, bool> filterExpression = (rule) => rule.DefinitionId == query.RuleDefinitionId
                && (string.IsNullOrEmpty(query.Description) || (!string.IsNullOrEmpty(rule.Description) && rule.Description.IndexOf(query.Description, StringComparison.OrdinalIgnoreCase) >= 0))
                && (!query.EffectiveDate.HasValue || (rule.BeginEffectiveTime <= query.EffectiveDate.Value && (!rule.EndEffectiveTime.HasValue || query.EffectiveDate.Value < rule.EndEffectiveTime)))
                && (query.CriteriaFieldValues == null || RuleCriteriaFilter(rule, ruleDefinition, query.CriteriaFieldValues))
                && (query.SettingsFilterValue == null || RuleSettingsFilter(rule, ruleDefinition, query.SettingsFilterValue));

            List<T> filteredRules = GetAllRules().Values.FindAllRecords(filterExpression).ToList();

            if (withOverridenRules)
                return filteredRules;
            else
                return FilterOverridenRules(filteredRules, ruleDefinition, query.CriteriaFieldValues);
        }

        List<T> FilterOverridenRules(List<T> rules, GenericRuleDefinition ruleDefinition, Dictionary<string, object> criterias)
        {
            if (criterias == null || criterias.Count == 0)
                return rules;

            List<T> result = new List<T>();

            List<T> overridenRules = new List<T>();

            criterias = criterias.OrderBy(itm => ruleDefinition.CriteriaDefinition.Fields.First(item => item.FieldName == itm.Key).Priority).ToDictionary(itm => itm.Key, itm => itm.Value);
            string lastCriteriaName = criterias.Last().Key;

            foreach (T rule in rules)
            {
                bool isRuleOverriden = false;

                foreach (T targetRule in rules)
                {
                    if (rule == targetRule || overridenRules.Contains(targetRule))
                        continue;

                    for (var index = 0; index < ruleDefinition.CriteriaDefinition.Fields.Count; index++)
                    {
                        var genericRuleDefinitionCriteriaField = ruleDefinition.CriteriaDefinition.Fields[index];
                        var criteriaFieldName = genericRuleDefinitionCriteriaField.FieldName;

                        var ruleFieldValues = rule.Criteria != null ? rule.Criteria.FieldsValues : null;
                        var ruleFieldValue = ruleFieldValues != null ? ruleFieldValues.GetRecord(criteriaFieldName) : null;

                        var targetRuleFieldValues = targetRule.Criteria != null ? targetRule.Criteria.FieldsValues : null;
                        if (targetRuleFieldValues == null) //target rule cannot override current rule
                            break;

                        var targetRuleFieldValue = targetRuleFieldValues.GetRecord(criteriaFieldName);

                        if (!criterias.ContainsKey(criteriaFieldName))
                        {
                            if ((ruleFieldValue == null && targetRuleFieldValue == null)
                                || (ruleFieldValue != null && targetRuleFieldValue != null && SameCriteriaValues(ruleFieldValue.GetValues(), targetRuleFieldValue.GetValues())))
                                continue;
                            else
                                break;
                        }

                        if (ruleFieldValue != null && targetRuleFieldValue == null)
                            break;

                        if (ruleFieldValue == null && targetRuleFieldValue != null)
                        {
                            string tempLastCriteriaName = ruleDefinition.CriteriaDefinition.Fields.Last().FieldName;
                            if (criteriaFieldName == tempLastCriteriaName)
                            {
                                isRuleOverriden = true;
                                break;
                            }
                            bool isOverriden = true;
                            for (var tempIndex = index + 1; tempIndex < ruleDefinition.CriteriaDefinition.Fields.Count; tempIndex++)
                            {
                                var tempGenericRuleDefinitionCriteriaField = ruleDefinition.CriteriaDefinition.Fields[tempIndex];
                                var tempCriteriaFieldName = tempGenericRuleDefinitionCriteriaField.FieldName;
                                if (criterias.ContainsKey(tempCriteriaFieldName))
                                    continue;

                                var tempCurrentRuleFieldValues = rule.Criteria != null ? rule.Criteria.FieldsValues.GetRecord(tempCriteriaFieldName) : null;
                                var tempTargetRuleFieldValues = targetRule.Criteria != null ? targetRule.Criteria.FieldsValues.GetRecord(tempCriteriaFieldName) : null;

                                if ((tempCurrentRuleFieldValues == null && tempTargetRuleFieldValues == null)
                                || (tempCurrentRuleFieldValues != null && tempTargetRuleFieldValues != null && SameCriteriaValues(tempCurrentRuleFieldValues.GetValues(), tempTargetRuleFieldValues.GetValues())))
                                {
                                    continue;
                                }
                                else
                                {
                                    isOverriden = false;
                                    break;
                                }
                            }

                            if (isOverriden)
                            {
                                isRuleOverriden = true;
                                break;
                            }
                        }

                        if (criteriaFieldName == lastCriteriaName)
                            break;
                    }

                    if (isRuleOverriden)
                        break;
                }
                if (!isRuleOverriden)
                    result.Add(rule);
                else
                    overridenRules.Add(rule);
            }

            return result;
        }

        private bool SameCriteriaValues(IEnumerable<object> firstRuleCriteriaValue, IEnumerable<object> secondRuleCriteriaValue)
        {
            if (firstRuleCriteriaValue == null && secondRuleCriteriaValue == null)
                return true;

            if (firstRuleCriteriaValue == null && secondRuleCriteriaValue != null)
                return false;

            if (firstRuleCriteriaValue != null && secondRuleCriteriaValue == null)
                return false;

            return firstRuleCriteriaValue.All(secondRuleCriteriaValue.Contains) && firstRuleCriteriaValue.Count() == secondRuleCriteriaValue.Count();
        }

        public GenericRule GetGenericRule(int ruleId, bool isViewedFromUI)
        {
            var rule = GetAllRules().GetRecord(ruleId);
            if (rule != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(new GenericRuleLoggableEntity(rule.DefinitionId), rule);
            return rule;
        }

        public GenericRule GetGenericRule(int ruleId)
        {
            return GetGenericRule(ruleId, false);
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

        public bool TryAddGenericRule(GenericRule rule)
        {
            return this.TryAdd(rule as T);
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericRuleDetail> UpdateGenericRule(GenericRule rule)
        {
            return this.UpdateRule(rule as T) as Vanrise.Entities.UpdateOperationOutput<GenericRuleDetail>;
        }

        public bool TryUpdateGenericRule(GenericRule rule)
        {
            return this.TryUpdateRule(rule as T);
        }

        public DeleteOperationOutput<GenericRuleDetail> DeleteGenericRule(int ruleId)
        {
            return this.SetRuleDeleted(ruleId) as Vanrise.Entities.DeleteOperationOutput<GenericRuleDetail>;
        }

        public override RuleLoggableEntity GetLoggableEntity(T rule)
        {
            return new GenericRuleLoggableEntity(rule.DefinitionId);
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
                {
                    switch (fieldRuntimeType.FullName)
                    {
                        case "System.Guid":
                            {
                                GuidConverter guidConverter = new GuidConverter();
                                return values.Select(itm => guidConverter.ConvertFrom(itm));
                            }

                        default:
                            return values.Select(itm => Convert.ChangeType(itm, fieldRuntimeType));
                    }
                }
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
            if (ruleDefinitionCriteria != null)
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
                if (criteriaField.ValueObjectName != null || criteriaField.ValuePropertyName != null)
                {
                    if (criteriaField.ValueObjectName == null)
                        throw new NullReferenceException("criteriaField.ValueObjectName");
                    if (criteriaField.ValuePropertyName == null)
                        throw new NullReferenceException("criteriaField.ValuePropertyName");
                    if (objects == null)
                        throw new NullReferenceException("VRObjectVariableCollection objects");

                    VRObjectVariable objectVariable;
                    if (!objects.TryGetValue(criteriaField.ValueObjectName, out objectVariable))
                        throw new NullReferenceException(String.Format("objectVariable '{0}'", criteriaField.ValueObjectName));

                    VRObjectTypeDefinitionManager vrObjectTypeDefinitionManager = new VRObjectTypeDefinitionManager();
                    VRObjectTypeDefinition vrObjectTypeDefinition = vrObjectTypeDefinitionManager.GetVRObjectTypeDefinition(objectVariable.VRObjectTypeDefinitionId);
                    if (vrObjectTypeDefinition.Settings == null)
                        throw new NullReferenceException(String.Format("vrObjectTypeDefinition.Settings: '{0}'", criteriaField.ValueObjectName));

                    if (vrObjectTypeDefinition.Settings.ObjectType == null)
                        throw new NullReferenceException(String.Format("vrObjectTypeDefinition.Settings.ObjectType: '{0}'", criteriaField.ValueObjectName));

                    VRObjectTypePropertyDefinition vrObjectTypePropertyDefinition;
                    if (!vrObjectTypeDefinition.Settings.Properties.TryGetValue(criteriaField.ValuePropertyName, out vrObjectTypePropertyDefinition))
                        throw new NullReferenceException(String.Format("vrObjectTypeDefinition.Settings.Properties: '{0}'", criteriaField.ValuePropertyName));

                    ruleCriteriaEvaluationInfos.Add(new CriteriaEvaluationInfo
                    {
                        CriteriaName = criteriaField.FieldName,
                        ObjectName = criteriaField.ValueObjectName,
                        ObjectType = vrObjectTypeDefinition.Settings.ObjectType,
                        PropertyEvaluator = vrObjectTypePropertyDefinition.PropertyEvaluator
                    });
                }
            }
            return ruleCriteriaEvaluationInfos;
        }

        #endregion

        #region Private Methods

        bool RuleCriteriaFilter(GenericRule rule, GenericRuleDefinition ruleDefinition, Dictionary<string, object> filterValues)
        {
            if (rule.Criteria == null) return true;

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
                    if (values != null && !criteriaFieldType.IsMatched(values, filter.Value))
                        return false;
                }
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

        #region Private Classes

        private class GenericRuleExcelExportHandler : ExcelExportHandler<GenericRuleDetail>
        {
            private GenericRuleQuery _query;
            public GenericRuleExcelExportHandler(GenericRuleQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<GenericRuleDetail> context)
            {
                GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
                var genericRuleDefinition = genericRuleDefinitionManager.GetGenericRuleDefinition(_query.RuleDefinitionId);

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description" });

                if (genericRuleDefinition.CriteriaDefinition != null)
                {
                    foreach (var field in genericRuleDefinition.CriteriaDefinition.Fields)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = field.Title });
                    }
                }

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Settings" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Description });

                            if (record.FieldValueDescriptions == null || record.FieldValueDescriptions.Count == 0)
                            {
                                if (genericRuleDefinition.CriteriaDefinition != null)
                                {
                                    foreach (var field in genericRuleDefinition.CriteriaDefinition.Fields)
                                    {
                                        row.Cells.Add(new ExportExcelCell { Value = null });
                                    }

                                }
                            }
                            else
                            {
                                foreach (var field in record.FieldValueDescriptions)
                                {
                                    row.Cells.Add(new ExportExcelCell { Value = field });
                                }
                            }

                            row.Cells.Add(new ExportExcelCell { Value = record.SettingsDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BeginEffectiveTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EndEffectiveTime });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        internal class GenericRuleLoggableEntity : RuleLoggableEntity
        {
            Guid _ruleDefinitionId;
            static GenericRuleDefinitionManager s_ruleDefinitionManager = new GenericRuleDefinitionManager();

            public GenericRuleLoggableEntity(Guid ruleDefinitionId)
            {
                _ruleDefinitionId = ruleDefinitionId;
            }

            public override string EntityUniqueName
            {
                get { return String.Format("VR_GenericData_GenericRule_{0}", _ruleDefinitionId); }
            }

            public override string EntityDisplayName
            {
                get { return s_ruleDefinitionManager.GetGenericRuleDefinitionName(_ruleDefinitionId); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_GenericRule_ViewHistoryItem"; }
            }
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

    public class GenericRuleCachingExpirationChecker : RuleCachingExpirationChecker
    {
        DateTime? _genericRuleDefinitionCacheLastCheck;

        public override bool IsRuleDependenciesCacheExpired()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<GenericRuleDefinitionManager.CacheManager>().IsCacheExpired(ref _genericRuleDefinitionCacheLastCheck);
        }
    }
}