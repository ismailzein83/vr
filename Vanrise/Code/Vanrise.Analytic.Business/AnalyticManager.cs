using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Security.Business;

namespace Vanrise.Analytic.Business
{ 
    public class AnalyticManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            // CheckAnalyticRequiredPermission(input);
            if (input.Query.FromTime == input.Query.ToTime)
                return null;
            if (!input.Query.CurrencyId.HasValue)
            {
                CurrencyManager currencyManager = new CurrencyManager();
                input.Query.CurrencyId = currencyManager.GetSystemCurrency().CurrencyId;
            }
            if (input.SortByColumnName != null && input.SortByColumnName.Contains("MeasureValues"))
            {
                string[] measureProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""].Value", measureProperty[0], measureProperty[1]);
            }
            if(input.Query.AdvancedOrderOptions != null)
            {
                input.Query.MeasureFields.ThrowIfNull("input.Query.MeasureFields");
                List<string> additionalMeasureNames = input.Query.AdvancedOrderOptions.GetAdditionalMeasureNames();
                foreach(var m in additionalMeasureNames)
                {
                    if (!input.Query.MeasureFields.Contains(m))
                        input.Query.MeasureFields.Add(m);
                }
            }
            return BigDataManager.Instance.RetrieveData(input, new AnalyticRecordRequestHandler());
        }
        public RecordFilterGroup BuildRecordSearchFilterGroup(RecordSearchFilterGroupInput input)
        {
            Guid dataRecordTypeId = GetDataRecordTypeForReportBySourceName(input.ReportId, input.SourceName);
            DataRecordTypeManager dataRecordTypeManager = new GenericData.Business.DataRecordTypeManager();
            var recordType = dataRecordTypeManager.GetDataRecordType(dataRecordTypeId);
            AnalyticItemConfigManager analyticItemConfigManager = new Business.AnalyticItemConfigManager();
            var analyticDimensions = analyticItemConfigManager.GetDimensions(input.TableId);
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            foreach (var dimensionFilter in input.DimensionFilters)
            {
                AnalyticDimension dimension;
                if (analyticDimensions.TryGetValue(dimensionFilter.Dimension, out dimension))
                {
                    var dimensionFieldMapping = dimension.Config.DimensionFieldMappings.Find(x => x.DataRecordTypeId == dataRecordTypeId);
                    if (dimensionFieldMapping == null)
                    {
                        throw new ArgumentNullException(string.Format("Dimension {0} is not mapped to record type {1}.", dimension.Name, recordType.Name));
                    }
                    var record = recordType.Fields.FindRecord(x => x.Name == dimensionFieldMapping.FieldName);
                    if (record == null)
                    {
                        throw new ArgumentNullException(string.Format("Record field mapping for dimension {0} not found.", dimension.Name));
                    }
                    var recordFilter = record.Type.ConvertToRecordFilter(record.Name, dimensionFilter.FilterValues);
                    // recordFilter.FieldName = record.Name;
                    if (recordFilterGroup.Filters == null)
                        recordFilterGroup.Filters = new List<RecordFilter>();
                    recordFilterGroup.Filters.Add(recordFilter);
                }
            }
            if (input.FilterGroup != null)
            {
                var filterGroup = ConvertRecordFilterGroup(input.FilterGroup, recordType, analyticDimensions);
                recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
                if (input.FilterGroup.LogicalOperator == RecordQueryLogicalOperator.And)
                {
                    foreach (var filter in filterGroup.Filters)
                    {
                        recordFilterGroup.Filters.Add(filter);
                    }
                }
                else
                {
                    recordFilterGroup.Filters.Add(filterGroup);
                }
            }
            return recordFilterGroup;
        }
        public Guid GetDataRecordTypeForReportBySourceName(Guid reportId, string sourceName)
        {
            AnalyticReportManager analyticReportManager = new Business.AnalyticReportManager();
            var analyticReport = analyticReportManager.GetAnalyticReportById(reportId);
            var analyticReportSettings = analyticReport.Settings as Entities.DataRecordSearchPageSettings;
            Guid dataRecordTypeId = Guid.Empty;
            foreach (var source in analyticReportSettings.Sources)
            {
                if (source.Name == sourceName)
                {
                    dataRecordTypeId = source.DataRecordTypeId;
                }
            }
            return dataRecordTypeId;
        }
        public RecordFilterGroup ConvertRecordFilterGroup(RecordFilterGroup filterGroup, DataRecordType recordType, Dictionary<string, AnalyticDimension> analyticDimensions)
        {
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            recordFilterGroup.LogicalOperator = filterGroup.LogicalOperator;
            foreach (var filter in filterGroup.Filters)
            {
                ConvertChildRecordFilterGroup(recordFilterGroup, filter, recordType, analyticDimensions);
            }
            return recordFilterGroup;
        }
        public void ConvertChildRecordFilterGroup(RecordFilterGroup recordFilterGroup, RecordFilter recordFilter, DataRecordType recordType, Dictionary<string, AnalyticDimension> analyticDimensions)
        {
            RecordFilterGroup childFilterGroup = recordFilter as RecordFilterGroup;
            if (recordFilterGroup.Filters == null)
                recordFilterGroup.Filters = new List<RecordFilter>();
            if (childFilterGroup != null)
            {
                RecordFilterGroup childRecordFilterGroup = new RecordFilterGroup();
                childRecordFilterGroup.LogicalOperator = childFilterGroup.LogicalOperator;
                recordFilterGroup.Filters.Add(childRecordFilterGroup);
                foreach (var filter in childFilterGroup.Filters)
                {
                    ConvertChildRecordFilterGroup(childRecordFilterGroup, filter, recordType, analyticDimensions);
                }
            }
            else if (recordFilter.FieldName != null)
            {
                AnalyticDimension dimension;
                if (analyticDimensions.TryGetValue(recordFilter.FieldName, out dimension))
                {
                    var dimensionFieldMapping = dimension.Config.DimensionFieldMappings.Find(x => x.DataRecordTypeId == recordType.DataRecordTypeId);
                    if (dimensionFieldMapping != null)
                    {
                        var record = recordType.Fields.FindRecord(x => x.Name == dimensionFieldMapping.FieldName);
                        if (record != null)
                        {
                            recordFilter.FieldName = record.Name;
                            recordFilterGroup.Filters.Add(recordFilter);
                        }
                    }
                }
            }
        }

        public bool CheckAnalyticRequiredPermission(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            AnalyticTableManager manager = new AnalyticTableManager();

            int userId = SecurityContext.Current.GetLoggedInUserId();

            if (!manager.DoesUserHaveAccess(userId, input.Query.TableId))
            {
                return false;
            }
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();

            if (!analyticItemConfigManager.DoesUserHaveAccess(userId, input.Query.TableId, input.Query.MeasureFields))
            {
                return false;
            }
            return true;
        }

        #region Private Methods

        internal List<AnalyticRecord> ProcessSQLRecords(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, List<string> parentDimensionNames, List<string> measureNames, List<MeasureStyleRule> measureStyleRules, List<DimensionFilter> dimensionFilters,
           RecordFilterGroup filterGroup, IEnumerable<DBAnalyticRecord> dbRecords, HashSet<string> availableDimensions, bool withSummary, out AnalyticRecord summaryRecord)
        {
            List<string> allDimensionNamesList = new List<string>();


            Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary = BuildMeasureStyleRulesDictionary(measureStyleRules);

            if (requestedDimensionNames != null)
                allDimensionNamesList.AddRange(requestedDimensionNames);
            if (parentDimensionNames != null)
                allDimensionNamesList.AddRange(parentDimensionNames);
            HashSet<string> allDimensionNames = new HashSet<string>(allDimensionNamesList);
            FillCalculatedDimensions(analyticTableQueryContext, requestedDimensionNames, dbRecords, availableDimensions, dimensionFilters, filterGroup);
            List<AnalyticRecord> records = ApplyFinalGroupingAndFiltering(analyticTableQueryContext, dbRecords, requestedDimensionNames, allDimensionNames, measureNames, measureStyleRulesDictionary, dimensionFilters, filterGroup, withSummary, out summaryRecord);
            return records;
        }
        private Dictionary<string, MeasureStyleRule> BuildMeasureStyleRulesDictionary(List<MeasureStyleRule> measureStyleRules)
        {
            Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary = null;
            if (measureStyleRules != null)
            {
                measureStyleRulesDictionary = new Dictionary<string, MeasureStyleRule>();
                foreach (var styleRule in measureStyleRules)
                {
                    MeasureStyleRule measureStyleRule = null;
                    if (!measureStyleRulesDictionary.TryGetValue(styleRule.MeasureName, out measureStyleRule))
                    {
                        measureStyleRulesDictionary.Add(styleRule.MeasureName, styleRule);
                    }
                }
            }
            return measureStyleRulesDictionary;

        }
        private void FillCalculatedDimensions(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, IEnumerable<DBAnalyticRecord> sqlRecords, HashSet<string> availableDimensions, List<DimensionFilter> dimensionFilters,
           RecordFilterGroup filterGroup)
        {
            List<string> dimensionNamesToCalculate = new List<string>();
            if (requestedDimensionNames != null)
                dimensionNamesToCalculate.AddRange(requestedDimensionNames);
            if (dimensionFilters != null)
                dimensionNamesToCalculate.AddRange(dimensionFilters.Select(dimFilter => dimFilter.Dimension));
            var filterGroupDimensions = analyticTableQueryContext.GetDimensionNames(filterGroup);
            if (filterGroupDimensions != null)
                dimensionNamesToCalculate.AddRange(filterGroupDimensions);

            dimensionNamesToCalculate = dimensionNamesToCalculate.Where(dimName => !availableDimensions.Contains(dimName)).ToList();

            if (dimensionNamesToCalculate.Count > 0)
            {
                IEnumerable<AnalyticDimension> dimensionsToCalculate = new HashSet<string>(dimensionNamesToCalculate).Select(dimName => analyticTableQueryContext.GetDimensionConfig(dimName));
                foreach (var sqlRecord in sqlRecords)
                {
                    foreach (var dimToCalculate in dimensionsToCalculate)
                    {
                        var getDimensionValueContext = new GetDimensionValueContext(analyticTableQueryContext, sqlRecord);
                        sqlRecord.GroupingValuesByDimensionName.Add(dimToCalculate.Name, new DBAnalyticRecordGroupingValue
                        {
                            Value = dimToCalculate.Evaluator.GetDimensionValue(getDimensionValueContext)
                        });
                    }
                }
            }
        }

        private List<AnalyticRecord> ApplyFinalGroupingAndFiltering(IAnalyticTableQueryContext analyticTableQueryContext, IEnumerable<DBAnalyticRecord> dbRecords, List<string> requestedDimensionNames, HashSet<string> allDimensionNames, List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary, List<DimensionFilter> dimensionFilters, RecordFilterGroup filterGroup, bool withSummary, out AnalyticRecord summaryRecord)
        {
            Dictionary<string, DBAnalyticRecord> groupedRecordsByDimensionsKey = new Dictionary<string, DBAnalyticRecord>();
            DBAnalyticRecord summarySQLRecord = null;
            foreach (var dbRecord in dbRecords)
            {
                if (!ApplyFilters(analyticTableQueryContext, dbRecord, dimensionFilters, filterGroup))
                    continue;
                string groupingKey = GetDimensionGroupingKey(requestedDimensionNames, dbRecord);
                DBAnalyticRecord matchRecord;
                if (!groupedRecordsByDimensionsKey.TryGetValue(groupingKey, out matchRecord))
                {
                    matchRecord = dbRecord;
                    groupedRecordsByDimensionsKey.Add(groupingKey, matchRecord);
                }
                else
                {
                    UpdateAggregateValues(analyticTableQueryContext, matchRecord, dbRecord);
                }
                foreach (var groupingValue in matchRecord.GroupingValuesByDimensionName)
                {
                    if (groupingValue.Value.AllValues == null)
                        groupingValue.Value.AllValues = new List<dynamic>();
                    groupingValue.Value.AllValues.Add(dbRecord.GroupingValuesByDimensionName[groupingValue.Key].Value);
                }
                if (withSummary)
                {
                    if (summarySQLRecord == null)
                    {
                        summarySQLRecord = new DBAnalyticRecord { GroupingValuesByDimensionName = new Dictionary<string, DBAnalyticRecordGroupingValue>(), AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
                        foreach (var groupingValueEntry in dbRecord.GroupingValuesByDimensionName)
                        {
                            summarySQLRecord.GroupingValuesByDimensionName.Add(groupingValueEntry.Key, groupingValueEntry.Value.Clone() as DBAnalyticRecordGroupingValue);
                        }
                        foreach (var aggValueEntry in dbRecord.AggValuesByAggName)
                        {
                            summarySQLRecord.AggValuesByAggName.Add(aggValueEntry.Key, aggValueEntry.Value.Clone() as DBAnalyticRecordAggValue);
                        }
                    }
                    else
                        UpdateAggregateValues(analyticTableQueryContext, summarySQLRecord, dbRecord);

                    foreach (var groupingValue in summarySQLRecord.GroupingValuesByDimensionName)
                    {
                        if (groupingValue.Value.AllValues == null)
                            groupingValue.Value.AllValues = new List<dynamic>();
                        groupingValue.Value.AllValues.Add(dbRecord.GroupingValuesByDimensionName[groupingValue.Key].Value);
                    }
                }
            }
            List<AnalyticRecord> analyticRecords = new List<AnalyticRecord>();
            HashSet<DateTime> timeForMissingData = null;
            if (analyticTableQueryContext.Query.TimeGroupingUnit.HasValue)
            {
                timeForMissingData = new HashSet<DateTime>();
                DateTime fromTime = GetStartDateTime(analyticTableQueryContext.Query.FromTime, analyticTableQueryContext.Query.TimeGroupingUnit.Value);
                var toTime = analyticTableQueryContext.Query.ToTime.HasValue ? analyticTableQueryContext.Query.ToTime.Value : DateTime.Now;
                timeForMissingData.Add(fromTime);
                fromTime = GetNextDateTime(fromTime, analyticTableQueryContext.Query.TimeGroupingUnit.Value);
                while (fromTime <= toTime)
                {
                    timeForMissingData.Add(fromTime);
                   fromTime = GetNextDateTime(fromTime, analyticTableQueryContext.Query.TimeGroupingUnit.Value);
                }
            }
            foreach (var dbRecord in groupedRecordsByDimensionsKey.Values)
            {
                AnalyticRecord analyticRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, dbRecord, requestedDimensionNames, allDimensionNames, measureNames, measureStyleRulesDictionary);
                if (analyticTableQueryContext.Query.TimeGroupingUnit.HasValue)
                {
                    timeForMissingData.Remove(analyticRecord.Time.Value);
                }
                analyticRecords.Add(analyticRecord);
            }
            if (timeForMissingData != null)
            {
                foreach (var dateTime in timeForMissingData)
                {
                    AnalyticRecord analyticRecord = CreateAnalyticRecordFilledWithDefaultValues(analyticTableQueryContext, measureNames, measureStyleRulesDictionary, dateTime);
                    analyticRecords.Add(analyticRecord);
                }
            }

            if (withSummary && summarySQLRecord != null)
                summaryRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, summarySQLRecord, null, allDimensionNames, measureNames, null);
            else
                summaryRecord = null;
            return analyticRecords;
        }

        private AnalyticRecord CreateAnalyticRecordFilledWithDefaultValues(IAnalyticTableQueryContext analyticTableQueryContext,List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary,DateTime dateTime)
        {
            AnalyticRecord analyticRecord = new AnalyticRecord() { Time = dateTime, MeasureValues = new MeasureValues() };
            foreach (var measureName in measureNames)
            {
                var measureConfig = analyticTableQueryContext.GetMeasureConfig(measureName);
                var measureRuntimeType = measureConfig.Config.FieldType.GetRuntimeType();
                var measureValue = Utilities.GetTypeDefault(measureRuntimeType);
                RecordFilterManager filterManager = new RecordFilterManager();
                string styleCode = null;
                if (measureStyleRulesDictionary != null)
                {
                    MeasureStyleRule measureStyleRule = null;
                    if (measureStyleRulesDictionary.TryGetValue(measureName, out measureStyleRule))
                    {
                        foreach (var rule in measureStyleRule.Rules)
                        {
                            StyleRuleConditionContext context = new StyleRuleConditionContext { Value = measureValue };
                            if (rule.RecordFilter != null && filterManager.IsSingleFieldFilterMatch(rule.RecordFilter, measureValue, measureConfig.Config.FieldType))
                            {
                                styleCode = rule.StyleCode;
                                break;
                            }
                        }
                    }

                }
                analyticRecord.MeasureValues.Add(measureName, new MeasureValue { Value = measureValue, StyleCode = styleCode });
            }
            return analyticRecord;
        }
       
        private DateTime GetNextDateTime(DateTime time, TimeGroupingUnit timeGroupingUnit)
        {
            switch (timeGroupingUnit)
            {
                case TimeGroupingUnit.Day:
                    return time.AddDays(1);
                case TimeGroupingUnit.Hour:
                    return time.AddHours(1);
                default :
                    return time;
            }
        }
        private DateTime GetStartDateTime(DateTime time, TimeGroupingUnit timeGroupingUnit)
        {
            switch (timeGroupingUnit)
            {
                case TimeGroupingUnit.Day:
                    return time.Date;
                case TimeGroupingUnit.Hour:
                    return new DateTime(time.Year,time.Month,time.Day,time.Hour,0,0);
                default:
                    return time;
            }
        }
        private bool ApplyFilters(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord, List<DimensionFilter> dimensionFilters, RecordFilterGroup filterGroup)
        {
            if (filterGroup != null)
            {
                var recordFilterManager = new RecordFilterManager();
                var recordFilterContext = new RecordFilterGenericFieldMatchContext(analyticTableQueryContext, dbRecord);
                if (!recordFilterManager.IsFilterGroupMatch(filterGroup, recordFilterContext))
                    return false;
            }
            if (dimensionFilters != null)
            {
                foreach (var dimFilter in dimensionFilters)
                {
                    DBAnalyticRecordGroupingValue dimensionValue;
                    if (!dbRecord.GroupingValuesByDimensionName.TryGetValue(dimFilter.Dimension, out dimensionValue))
                        throw new NullReferenceException(String.Format("dimensionValue. dimName '{0}'", dimFilter.Dimension));
                    if (dimensionValue.Value == null)
                        return dimFilter.FilterValues.Contains(null);
                    else
                    {
                        var nonNullFilterValues = dimFilter.FilterValues.Where(itm => itm != null).ToList();
                        if (nonNullFilterValues.Count > 0)
                        {
                            var filterValueType = nonNullFilterValues[0].GetType();
                            var convertedDimensionValue = filterValueType == typeof(string) ? dimensionValue.Value.ToString() : Convert.ChangeType(dimensionValue.Value, filterValueType);
                            if (!nonNullFilterValues.Contains(convertedDimensionValue))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        private string GetDimensionGroupingKey(List<string> requestedDimensionNames, DBAnalyticRecord record)
        {
            StringBuilder builder = new StringBuilder();
            if (record.Time.HasValue)
                builder.Append(record.Time.Value.ToString());
            if (requestedDimensionNames != null)
            {
                foreach (var dimensionName in requestedDimensionNames)
                {
                    DBAnalyticRecordGroupingValue groupingValue;
                    if (record.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                    {
                        builder.AppendFormat("^*^{0}", groupingValue.Value != null ? groupingValue.Value : "");
                    }
                    else
                        throw new NullReferenceException(String.Format("groupingValue. dimName '{0}'", dimensionName));
                }
            }
            return builder.ToString();
        }

        private void UpdateAggregateValues(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord existingRecord, DBAnalyticRecord record)
        {
            foreach (var aggEntry in record.AggValuesByAggName)
            {
                var existingAgg = existingRecord.AggValuesByAggName[aggEntry.Key];
                var agg = aggEntry.Value;
                if (existingAgg.Value == null)
                    existingAgg.Value = agg.Value;
                else if (agg.Value != null)
                {
                    switch (analyticTableQueryContext.GetAggregateConfig(aggEntry.Key).Config.AggregateType)
                    {
                        case AnalyticAggregateType.Count:
                        case AnalyticAggregateType.Sum:
                            existingAgg.Value = existingAgg.Value + agg.Value;
                            break;
                        case AnalyticAggregateType.Max:
                            if (existingAgg.Value < agg.Value)
                                existingAgg.Value = agg.Value;
                            break;
                        case AnalyticAggregateType.Min:
                            if (existingAgg.Value > agg.Value)
                                existingAgg.Value = agg.Value;
                            break;
                    }
                }
            }
        }

        private AnalyticRecord BuildAnalyticRecordFromSQLRecord(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord, List<string> dimensionNames, HashSet<string> allDimensionNames, List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary)
        {
            AnalyticRecord analyticRecord = new AnalyticRecord() { Time = dbRecord.Time, MeasureValues = new MeasureValues() };

            if (dimensionNames != null)
            {
                analyticRecord.DimensionValues = new DimensionValue[dimensionNames.Count];
                int dimIndex = 0;
                foreach (string dimName in dimensionNames)
                {
                    var dimensionValue = new DimensionValue();
                    dimensionValue.Value = dbRecord.GroupingValuesByDimensionName[dimName].Value;
                    if (dimensionValue.Value != null && dimensionValue.Value != DBNull.Value)
                        dimensionValue.Name = analyticTableQueryContext.GetDimensionConfig(dimName).Config.FieldType.GetDescription(dimensionValue.Value);
                    analyticRecord.DimensionValues[dimIndex] = dimensionValue;
                    dimIndex++;
                }
            }
            foreach (var measureName in measureNames)
            {
                var measureConfig = analyticTableQueryContext.GetMeasureConfig(measureName);
                var getMeasureValueContext = new GetMeasureValueContext(analyticTableQueryContext, dbRecord, allDimensionNames);
                var measureValue = measureConfig.Evaluator.GetMeasureValue(getMeasureValueContext);
                RecordFilterManager filterManager = new RecordFilterManager();
                string styleCode = null;
                if (measureStyleRulesDictionary != null)
                {
                    MeasureStyleRule measureStyleRule = null;
                    if (measureStyleRulesDictionary.TryGetValue(measureName, out measureStyleRule))
                    {
                        foreach (var rule in measureStyleRule.Rules)
                        {
                            StyleRuleConditionContext context = new StyleRuleConditionContext { Value = measureValue };
                            if (rule.RecordFilter != null && filterManager.IsSingleFieldFilterMatch(rule.RecordFilter, measureValue, measureConfig.Config.FieldType))
                            {
                                styleCode = rule.StyleCode;
                                break;
                            }
                        }
                    }

                }
                analyticRecord.MeasureValues.Add(measureName, new MeasureValue { Value = measureValue, StyleCode = styleCode });
            }


            return analyticRecord;
        }

        #endregion

        #region Private Classes

        private class AnalyticRecordRequestHandler : BigDataRequestHandler<AnalyticQuery, AnalyticRecord, AnalyticRecord>
        {

            public AnalyticRecordRequestHandler()
            {
            }
            public override AnalyticRecord EntityDetailMapper(AnalyticRecord entity)
            {
                return entity;
            }

            AnalyticRecord _summaryRecord;
            public override IEnumerable<AnalyticRecord> RetrieveAllData(DataRetrievalInput<AnalyticQuery> input)
            {
                if(input.Query.LastHours.HasValue)
                {
                    input.Query.FromTime = DateTime.Now.AddHours(-input.Query.LastHours.Value);
                }
                var query = input.Query;
                var queryContext = new AnalyticTableQueryContext(query);
                var dataProvider = queryContext.GetTable().Settings.DataProvider;
                if (dataProvider == null)
                    dataProvider = Activator.CreateInstance(Type.GetType("Vanrise.Analytic.Data.SQL.SQLAnalyticDataProvider, Vanrise.Analytic.Data.SQL")) as AnalyticDataProvider;
                var dataManager = dataProvider.CreateDataManager(queryContext);
                HashSet<string> includeDBDimensions;
                var dbRecords = dataManager.GetAnalyticRecords(input, out includeDBDimensions);

                if (dbRecords != null)
                    return (new AnalyticManager()).ProcessSQLRecords(new AnalyticTableQueryContext(query), query.DimensionFields, query.ParentDimensions, query.MeasureFields, query.MeasureStyleRules, query.Filters, query.FilterGroup,
                        dbRecords, includeDBDimensions, query.WithSummary, out _summaryRecord);
                else
                    return null;
            }

            protected override BigResult<AnalyticRecord> AllRecordsToBigResult(DataRetrievalInput<AnalyticQuery> input, IEnumerable<AnalyticRecord> allRecords)
            {
                var query = input.Query;
                IEnumerable<AnalyticRecord> orderedRecords;
                if (query.OrderType.HasValue)
                {
                    switch (query.OrderType.Value)
                    {
                        case AnalyticQueryOrderType.ByAllDimensions: orderedRecords = GetOrderedByAllDimensions(query, allRecords); break;
                        case AnalyticQueryOrderType.ByAllMeasures: orderedRecords = GetOrderedByAllMeasures(query, allRecords); break;
                        case AnalyticQueryOrderType.AdvancedMeasureOrder: orderedRecords = GetOrderedByAllAdvancedMeasureOrder(query, allRecords); break;
                        default: orderedRecords = null; break;
                    }
                }
                else
                    orderedRecords = allRecords.VROrderList(input);

                IEnumerable<AnalyticRecord> pagedRecords;
                if (query.TopRecords.HasValue)
                    pagedRecords = orderedRecords.Take(query.TopRecords.Value);
                else
                    pagedRecords = orderedRecords.VRGetPage(input);
                var analyticBigResult = new AnalyticSummaryBigResult<AnalyticRecord>()
                {
                    ResultKey = input.ResultKey,
                    Data = pagedRecords.ToList(),
                    TotalCount = allRecords.Count()
                };
                if (input.Query.WithSummary)
                    analyticBigResult.Summary = _summaryRecord;
                return analyticBigResult;
            }

            private IEnumerable<AnalyticRecord> GetOrderedByAllDimensions(AnalyticQuery query, IEnumerable<AnalyticRecord> allRecords)
            {
                var queryContext = new AnalyticTableQueryContext(query);
                List<string> orderByDimensions = query.DimensionFields;
                if (orderByDimensions == null || orderByDimensions.Count == 0)
                    throw new NullReferenceException("orderByDimensions");
                IOrderedEnumerable<AnalyticRecord> orderedRecords;
                var firstDimensionConfig = queryContext.GetDimensionConfig(orderByDimensions[0]);
                if (firstDimensionConfig.Config.FieldType.OrderType == DataRecordFieldOrderType.ByFieldValue)
                    orderedRecords = allRecords.OrderBy(record => record.DimensionValues[0].Value);
                else
                    orderedRecords = allRecords.OrderBy(record => record.DimensionValues[0].Name);
                if (orderByDimensions.Count > 1)
                {
                    for (int i = 1; i < orderByDimensions.Count; i++)
                    {
                        var dimensionIndex = i;
                        var dimensionConfig = queryContext.GetDimensionConfig(orderByDimensions[dimensionIndex]);
                        if (dimensionConfig.Config.FieldType.OrderType == DataRecordFieldOrderType.ByFieldValue)
                            orderedRecords = orderedRecords.ThenBy(record => record.DimensionValues[dimensionIndex].Value);
                        else
                            orderedRecords = orderedRecords.ThenBy(record => record.DimensionValues[dimensionIndex].Name);
                    }
                }
                return orderedRecords;
            }

            private IEnumerable<AnalyticRecord> GetOrderedByAllMeasures(AnalyticQuery query, IEnumerable<AnalyticRecord> allRecords)
            {
                List<string> orderByMeasures = query.MeasureFields;
                if (orderByMeasures == null || orderByMeasures.Count() == 0)
                    throw new NullReferenceException("orderByMeasures");
                string firstMeasureName = orderByMeasures[0];
                IOrderedEnumerable<AnalyticRecord> orderedRecords = allRecords.OrderByDescending(record => record.MeasureValues[firstMeasureName].Value);
                if (orderByMeasures.Count > 1)
                {
                    for (int i = 1; i < orderByMeasures.Count; i++)
                    {
                        string measureName = orderByMeasures[i];
                        orderedRecords = orderedRecords.ThenByDescending(itm => itm.MeasureValues[measureName].Value);
                    }
                }
                return orderedRecords;
            }

            private IEnumerable<AnalyticRecord> GetOrderedByAllAdvancedMeasureOrder(AnalyticQuery query, IEnumerable<AnalyticRecord> allRecords)
            {
                if (query.MeasureFields == null)
                    throw new NullReferenceException("query.MeasureFields");
                AnalyticQueryAdvancedMeasureOrderOptions advancedMeasureOrderOptions = query.AdvancedOrderOptions.CastWithValidate<AnalyticQueryAdvancedMeasureOrderOptions>("query.AdvancedOrderOptions");

                if (advancedMeasureOrderOptions.MeasureOrders == null || advancedMeasureOrderOptions.MeasureOrders.Count == 0)
                    throw new NullReferenceException("query.AdvanceMeasureOrderOptions.MeasureOrders");
                var measureOrders = advancedMeasureOrderOptions.MeasureOrders;
                var firstMeasureOrder = measureOrders[0];
                if (!query.MeasureFields.Contains(firstMeasureOrder.MeasureName))
                    throw new Exception(String.Format("Measure Order '{0}' is not available in the query measures", firstMeasureOrder.MeasureName));
                IOrderedEnumerable<AnalyticRecord> orderedRecords = firstMeasureOrder.OrderDirection == OrderDirection.Ascending ?
                    allRecords.OrderBy(record => record.MeasureValues[firstMeasureOrder.MeasureName].Value) :
                    allRecords.OrderByDescending(record => record.MeasureValues[firstMeasureOrder.MeasureName].Value);
                if (measureOrders.Count > 1)
                {
                    for (int i = 1; i < measureOrders.Count; i++)
                    {
                        var measureOrder = measureOrders[i];
                        if (!query.MeasureFields.Contains(measureOrder.MeasureName))
                            throw new Exception(String.Format("Measure Order '{0}' is not available in the query measures", measureOrder.MeasureName));
                        orderedRecords = measureOrder.OrderDirection == OrderDirection.Ascending ?
                            orderedRecords.ThenBy(record => record.MeasureValues[measureOrder.MeasureName].Value) :
                            orderedRecords.ThenByDescending(record => record.MeasureValues[measureOrder.MeasureName].Value);
                    }
                }
                return orderedRecords;
            }

            protected override ResultProcessingHandler<AnalyticRecord> GetResultProcessingHandler(DataRetrievalInput<AnalyticQuery> input, BigResult<AnalyticRecord> bigResult)
            {
                return new ResultProcessingHandler<AnalyticRecord>
                {
                    ExportExcelHandler = new AnalyticExcelExportHandler(input.Query)
                };
            }
        }

        private class AnalyticExcelExportHandler : ExcelExportHandler<AnalyticRecord>
        {
            AnalyticQuery _query;
            public AnalyticExcelExportHandler(AnalyticQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AnalyticRecord> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                if (_query.DimensionFields != null)
                {
                    foreach (var dimName in _query.DimensionFields)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = dimName });
                    }
                }
                if (_query.MeasureFields != null)
                {
                    foreach (var measureName in _query.MeasureFields)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = measureName });
                    }
                }
                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    if (record.DimensionValues != null)
                    {
                        foreach (var dimValue in record.DimensionValues)
                        {
                            row.Cells.Add(new ExportExcelCell { Value = dimValue.Name });
                        }
                    }
                    if (_query.MeasureFields != null)
                    {
                        foreach (var measureName in _query.MeasureFields)
                        {
                            MeasureValue measureValue;
                            if (!record.MeasureValues.TryGetValue(measureName, out measureValue))
                                throw new NullReferenceException(String.Format("measureValue. measureName '{0}'", measureName));
                            row.Cells.Add(new ExportExcelCell { Value = measureValue.Value });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class RecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {
            IAnalyticTableQueryContext _analyticTableQueryContext;
            DBAnalyticRecord _dbRecord;

            public RecordFilterGenericFieldMatchContext(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord)
            {
                if (analyticTableQueryContext == null)
                    throw new ArgumentNullException("analyticTableQueryContext");
                if (dbRecord == null)
                    throw new ArgumentNullException("dbRecord");
                _analyticTableQueryContext = analyticTableQueryContext;
                _dbRecord = dbRecord;
            }
            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                //string[] fieldParts = fieldName.Split('_');
                //if(fieldParts[0] == "Dimension")
                //{
                string dimensionName = fieldName;// fieldParts[1];
                var dimensionConfig = _analyticTableQueryContext.GetDimensionConfig(dimensionName);
                fieldType = dimensionConfig.Config.FieldType;
                DBAnalyticRecordGroupingValue dimensionValue;
                if (!_dbRecord.GroupingValuesByDimensionName.TryGetValue(dimensionName, out dimensionValue))
                    throw new NullReferenceException(String.Format("dimensionValue. dimensionName '{0}'", dimensionName));
                return dimensionValue.Value;
                //}
                //else
                //{
                //    string measureName = fieldParts[1];
                //    var measureConfig = _analyticTableQueryContext.GetMeasureConfig(measureName);
                //    fieldType = measureConfig.Config.FieldType;
                //    Object measureValue;
                //    if (!_record.MeasureValues.TryGetValue(measureName, out measureValue))
                //        throw new NullReferenceException(String.Format("measureValue. measure name '{0}'", measureName));
                //    return measureValue;
                //}
            }
        }

        #endregion
    }
}
