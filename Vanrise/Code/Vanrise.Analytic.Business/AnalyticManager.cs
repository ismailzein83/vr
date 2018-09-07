using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Analytic.Business
{
    public class AnalyticManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            //CheckAnalyticRequiredPermission(input);
            if (input.Query.FromTime == input.Query.ToTime)
                return null;

            AnalyticTable analyticTable = new AnalyticTableManager().GetAnalyticTableById(input.Query.TableId);
            analyticTable.ThrowIfNull("analyticTable", input.Query.TableId);
            analyticTable.Settings.ThrowIfNull("analyticTable.Settings", input.Query.TableId);
            analyticTable.Settings.DataProvider.ThrowIfNull("analyticTable.Settings.DataProvider", input.Query.TableId);

            var analyticDataProvider = analyticTable.Settings.DataProvider.GetRemoteAnalyticDataProvider(new GetRemoteAnalyticDataProviderContext() { AnalyticTableId = input.Query.TableId });
            if (analyticDataProvider != null)
            {
                return analyticDataProvider.GetFilteredRecords(input);
            }
            else
            {
                SetQueryToTimeIfNull(input.Query);
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
                if (input.Query.AdvancedOrderOptions != null)
                {
                    input.Query.MeasureFields.ThrowIfNull("input.Query.MeasureFields");
                    List<string> additionalMeasureNames = input.Query.AdvancedOrderOptions.GetAdditionalMeasureNames();
                    foreach (var m in additionalMeasureNames)
                    {
                        if (!input.Query.MeasureFields.Contains(m))
                            input.Query.MeasureFields.Add(m);
                    }
                }
                return BigDataManager.Instance.RetrieveData(input, new AnalyticRecordRequestHandler());
            }
        }

        public List<AnalyticRecord> GetAllFilteredRecords(AnalyticQuery query)
        {
            AnalyticRecord summaryRecord;
            return GetAllFilteredRecords(query, out summaryRecord);
        }

        public List<AnalyticRecord> GetAllFilteredRecords(AnalyticQuery query, out AnalyticRecord summaryRecord)
        {
            return GetAllFilteredRecords(query, false, out summaryRecord);
        }

        public List<AnalyticRecord> GetAllFilteredRecords(AnalyticQuery query, bool dontApplyOrdering, out AnalyticRecord summaryRecord)
        {
            List<AnalyticResultSubTable> resultSubTables;
            return GetAllFilteredRecords(query, dontApplyOrdering, out summaryRecord, out resultSubTables);
        }

        public List<AnalyticRecord> GetAllFilteredRecords(AnalyticQuery query, out AnalyticRecord summaryRecord, out List<AnalyticResultSubTable> resultSubTables)
        {
            return GetAllFilteredRecords(query, false, out summaryRecord, out resultSubTables);
        }

        public List<AnalyticRecord> GetAllFilteredRecords(AnalyticQuery query, bool dontApplyOrdering, out AnalyticRecord summaryRecord, out List<AnalyticResultSubTable> resultSubTables)
        {
            SetQueryToTimeIfNull(query);
            if (query.LastHours.HasValue)
            {
                query.FromTime = DateTime.Now.AddHours(-query.LastHours.Value);
            }
            var queryContext = new AnalyticTableQueryContext(query);
            var dataProvider = queryContext.GetTable().Settings.DataProvider;
            if (dataProvider == null)
                dataProvider = Activator.CreateInstance(Type.GetType("Vanrise.Analytic.Data.SQL.SQLAnalyticDataProvider, Vanrise.Analytic.Data.SQL")) as AnalyticDataProvider;
            var dataManager = dataProvider.CreateDataManager(queryContext);
            HashSet<string> includeDBDimensions = null;

            var queryForDataManager = BuildQueryForDataManager(query);
            IEnumerable<DBAnalyticRecord> dbRecords = dataManager.GetAnalyticRecords(queryForDataManager, out includeDBDimensions);

            List<string> allDimensionNamesList = new List<string>();

            if (query.DimensionFields != null)
                allDimensionNamesList.AddRange(query.DimensionFields);
            if (query.ParentDimensions != null)
                allDimensionNamesList.AddRange(query.ParentDimensions);
            HashSet<string> allDimensionNames = new HashSet<string>(allDimensionNamesList);
            if (dbRecords != null)
            {
                Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary = BuildMeasureStyleRulesDictionary(query.MeasureStyleRules);
                FillCalculatedDimensions(queryContext, queryForDataManager.DimensionFields, dbRecords, includeDBDimensions, query.Filters, query.FilterGroup);
                var records = ApplyFinalGroupingAndFiltering(queryContext, dbRecords, query.DimensionFields, allDimensionNames,
                    query.MeasureFields, measureStyleRulesDictionary, query.Filters, query.SubTables, query.FilterGroup, query.WithSummary, out summaryRecord, out resultSubTables);
                if(dontApplyOrdering || !query.OrderType.HasValue)
                {
                    return records;
                }
                else
                {
                    if (records == null)
                        return null;
                    else
                        return GetOrderedAnalyticRecords(queryContext, query.OrderType.Value, query.DimensionFields, query.MeasureFields, query.AdvancedOrderOptions, records, record => record).ToList();
                }
            }
            else
            {
                resultSubTables = null;
                summaryRecord = null;
                return null;
            }
        }

        private AnalyticQuery BuildQueryForDataManager(AnalyticQuery query)
        {
            var queryForDataManager = query.VRDeepCopy();
            List<string> queryDimensions = queryForDataManager.DimensionFields != null ? new List<string>(queryForDataManager.DimensionFields) : null;
            List<string> queryMeasures = queryForDataManager.MeasureFields != null ? new List<string>(queryForDataManager.MeasureFields) : null;
            if (query.SubTables != null)
            {
                foreach (var subTable in query.SubTables)
                {
                    if (subTable.Dimensions != null)
                    {
                        if (queryDimensions == null)
                            queryDimensions = new List<string>();
                        queryDimensions.AddRange(subTable.Dimensions);
                    }
                    if (subTable.Measures != null)
                    {
                        if (queryMeasures == null)
                            queryMeasures = new List<string>();
                        queryMeasures.AddRange(subTable.Measures);
                    }
                }
            }
            if (queryDimensions != null && queryDimensions.Count > 0)
                queryForDataManager.DimensionFields = queryDimensions.Distinct().ToList();
            if (queryMeasures != null && queryMeasures.Count > 0)
                queryForDataManager.MeasureFields = queryMeasures.Distinct().ToList();
            return queryForDataManager;
        }

        public RecordFilterGroup BuildRecordSearchFilterGroup(RecordSearchFilterGroupInput input)
        {
            Guid dataRecordTypeId = GetDataRecordTypeForReportBySourceName(input.ReportId, input.SourceName);
            DataRecordTypeManager dataRecordTypeManager = new GenericData.Business.DataRecordTypeManager();
            var recordType = dataRecordTypeManager.GetDataRecordType(dataRecordTypeId);

            AnalyticItemConfigManager analyticItemConfigManager = new Business.AnalyticItemConfigManager();
            var analyticDimensions = analyticItemConfigManager.GetDimensions(input.TableId);

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;
            recordFilterGroup.Filters = new List<RecordFilter>();
            if (input.DimensionFilters != null)
            {
                foreach (var dimensionFilter in input.DimensionFilters)
                {
                    AnalyticDimension dimension;
                    if (analyticDimensions.TryGetValue(dimensionFilter.Dimension, out dimension))
                    {
                        var dimensionFieldMapping = dimension.Config.DimensionFieldMappings.Find(x => x.DataRecordTypeId == dataRecordTypeId);
                        if (dimensionFieldMapping == null)
                            throw new ArgumentNullException(string.Format("Dimension {0} is not mapped to record type {1}.", dimension.Name, recordType.Name));

                        var record = recordType.Fields.FindRecord(x => x.Name == dimensionFieldMapping.FieldName);
                        if (record == null)
                            throw new ArgumentNullException(string.Format("Record field mapping for dimension {0} not found.", dimension.Name));

                        List<object> notNullFilterValues = dimensionFilter.FilterValues.Where(itm => itm != null).ToList();
                        RecordFilter notNullValuesRecordFilter = notNullFilterValues.Count > 0 ? record.Type.ConvertToRecordFilter(record.Name, notNullFilterValues) : null;
                        EmptyRecordFilter emptyRecordFilter = notNullFilterValues.Count != dimensionFilter.FilterValues.Count ? new EmptyRecordFilter { FieldName = record.Name } : null;

                        if (notNullValuesRecordFilter != null && emptyRecordFilter != null)
                        {
                            RecordFilterGroup dimensionsRecordFilterGroup = new RecordFilterGroup();
                            dimensionsRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                            dimensionsRecordFilterGroup.Filters = new List<RecordFilter>();
                            dimensionsRecordFilterGroup.Filters.Add(emptyRecordFilter);
                            dimensionsRecordFilterGroup.Filters.Add(notNullValuesRecordFilter);

                            recordFilterGroup.Filters.Add(dimensionsRecordFilterGroup);
                        }
                        else if (notNullValuesRecordFilter != null)
                        {
                            recordFilterGroup.Filters.Add(notNullValuesRecordFilter);
                        }
                        else if (emptyRecordFilter != null)
                        {
                            recordFilterGroup.Filters.Add(emptyRecordFilter);
                        }
                    }
                }
            }


            if (input.FilterGroup != null)
            {
                var filterGroup = ConvertRecordFilterGroup(input.FilterGroup, recordType, analyticDimensions);

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

        public RecordFilterGroup BuildRecordSearchFieldFilter(RecordSearchFieldFilterInput input)
        {
            Guid dataRecordTypeId = GetDataRecordTypeForReportBySourceName(input.ReportId, input.SourceName);
            return new RecordFilterManager().BuildRecordFilterGroup(dataRecordTypeId, input.FieldFilters, null);
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

        const string DIMENSION_GROUPING_SEPARATOR = "^*^";

        public static string GetRecordDimensionsGroupingKey(List<string> requestedDimensionNames, DateTime? recordTime, Func<string, object> getDimensionValue)
        {
            StringBuilder builder = new StringBuilder();
            if (recordTime.HasValue)
                builder.Append(recordTime.Value.ToString());
            if (requestedDimensionNames != null)
            {
                foreach (var dimensionName in requestedDimensionNames)
                {
                    Object dimensionValue = getDimensionValue(dimensionName);
                    builder.Append(DIMENSION_GROUPING_SEPARATOR);
                    if (dimensionValue != null)
                        builder.Append(dimensionValue);
                }
            }
            return builder.ToString();
        }

        public static string GetDimensionValuesGroupingKey(DateTime? recordTime, DimensionValue[] dimensionValues)
        {
            StringBuilder builder = new StringBuilder();
            if (recordTime.HasValue)
                builder.Append(recordTime.Value.ToString());
            if (dimensionValues != null)
            {
                foreach (var dimensionValue in dimensionValues)
                {
                    builder.Append(DIMENSION_GROUPING_SEPARATOR);
                    if (dimensionValue != null && dimensionValue.Value != null)
                        builder.Append(dimensionValue.Value);
                }
            }
            return builder.ToString();
        }

        #endregion

        #region Private Methods

        private void SetQueryToTimeIfNull(AnalyticQuery query)
        {
            if (!query.ToTime.HasValue)
                query.ToTime = GenerateQueryToTime(query.FromTime);
        }

        internal static DateTime GenerateQueryToTime(DateTime fromTime)
        {
            if (fromTime < DateTime.Now)
                return DateTime.Today.AddDays(1);
            else
                return fromTime.AddDays(1);
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

        private void FillCalculatedDimensions(IAnalyticTableQueryContext analyticTableQueryContext, IEnumerable<string> allGroupingDimensionNames, IEnumerable<DBAnalyticRecord> sqlRecords, HashSet<string> availableDimensions, List<DimensionFilter> dimensionFilters,
           RecordFilterGroup filterGroup)
        {
            List<string> dimensionNamesToCalculate = new List<string>();
            if (allGroupingDimensionNames != null)
                dimensionNamesToCalculate.AddRange(allGroupingDimensionNames);
            var filterDimensions = analyticTableQueryContext.GetDimensionNamesFromQueryFilters();
            if (filterDimensions != null && filterDimensions.Count > 0)
                dimensionNamesToCalculate.AddRange(filterDimensions);

            dimensionNamesToCalculate = dimensionNamesToCalculate.Where(dimName => !availableDimensions.Contains(dimName)).ToList();

            if (dimensionNamesToCalculate.Count > 0)
            {
                dimensionNamesToCalculate = ResolveDimensionDependencies(dimensionNamesToCalculate, analyticTableQueryContext);
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

        private List<string> ResolveDimensionDependencies(List<string> dimensionNamesToCalculate, IAnalyticTableQueryContext analyticTableQueryContext)
        {
            List<string> dimOrderedByDependencies = new List<string>();
            foreach (var dimName in dimensionNamesToCalculate)
            {
                AddDimensionToOrderedByDependencies(analyticTableQueryContext.GetDimensionConfig(dimName), dimOrderedByDependencies, analyticTableQueryContext);
            }
            return dimOrderedByDependencies;
        }

        private void AddDimensionToOrderedByDependencies(AnalyticDimension dimConfig, List<string> dimOrderedByDependencies, IAnalyticTableQueryContext analyticTableQueryContext)
        {
            if (dimConfig.Config.DependentDimensions != null && dimConfig.Config.DependentDimensions.Count > 0)
            {
                foreach (var dependentDimName in dimConfig.Config.DependentDimensions)
                {
                    AddDimensionToOrderedByDependencies(analyticTableQueryContext.GetDimensionConfig(dependentDimName), dimOrderedByDependencies, analyticTableQueryContext);
                }
            }
            if (!dimOrderedByDependencies.Contains(dimConfig.Name) && dimConfig.Evaluator != null)
                dimOrderedByDependencies.Add(dimConfig.Name);
        }

        private List<AnalyticRecord> ApplyFinalGroupingAndFiltering(IAnalyticTableQueryContext analyticTableQueryContext, IEnumerable<DBAnalyticRecord> dbRecords,
            List<string> requestedDimensionNames, HashSet<string> allDimensionNames, List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary,
            List<DimensionFilter> dimensionFilters, List<AnalyticQuerySubTable> querySubTables, RecordFilterGroup filterGroup, bool withSummary, out AnalyticRecord summaryRecord, out List<AnalyticResultSubTable> resultSubTables)
        {
            Dictionary<string, DBAnalyticRecord> groupedRecordsByDimensionsKey = new Dictionary<string, DBAnalyticRecord>();
            DBAnalyticRecord summaryDBRecord = null;
            bool buildSummaryRecord = withSummary || (querySubTables != null && querySubTables.Count > 0);//building summary record is also needed to order the records of the sub table(s)
            List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesDBRecords = null;
            List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesSummaryDBRecords = null;
            if (querySubTables != null)
                InitializeSubTablesInProcessObjects(querySubTables, buildSummaryRecord, ref subTablesDBRecords, ref subTablesSummaryDBRecords);

            foreach (var dbRecord in dbRecords)
            {
                if (!ApplyFilters(analyticTableQueryContext, dbRecord, dimensionFilters, filterGroup))
                    continue;
                string recordGroupingKey = GetDimensionGroupingKey(requestedDimensionNames, dbRecord);
                DBAnalyticRecord matchRecord;
                if (!groupedRecordsByDimensionsKey.TryGetValue(recordGroupingKey, out matchRecord))
                {
                    matchRecord = dbRecord;
                    groupedRecordsByDimensionsKey.Add(recordGroupingKey, matchRecord);
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
                if (buildSummaryRecord)
                {
                    if (summaryDBRecord == null)
                    {
                        summaryDBRecord = CloneAnalyticDBRecord(dbRecord);
                    }
                    else
                    {
                        UpdateAggregateValues(analyticTableQueryContext, summaryDBRecord, dbRecord);
                    }

                    foreach (var groupingValue in summaryDBRecord.GroupingValuesByDimensionName)
                    {
                        if (groupingValue.Value.AllValues == null)
                            groupingValue.Value.AllValues = new List<dynamic>();
                        groupingValue.Value.AllValues.Add(dbRecord.GroupingValuesByDimensionName[groupingValue.Key].Value);
                    }
                }
                if (querySubTables != null)
                    ReflectDBRecordToSubTablesDBRecords(dbRecord, analyticTableQueryContext, recordGroupingKey, querySubTables, subTablesDBRecords, subTablesSummaryDBRecords, buildSummaryRecord);
            }
            List<AnalyticRecord> analyticRecords = new List<AnalyticRecord>();
            HashSet<DateTime> timeForMissingData = null;
            if (analyticTableQueryContext.TimeGroupingUnit.HasValue)
            {
                timeForMissingData = new HashSet<DateTime>();
                DateTime fromTime = GetStartDateTime(analyticTableQueryContext.FromTime, analyticTableQueryContext.TimeGroupingUnit.Value);
                var toTime = analyticTableQueryContext.ToTime;
                timeForMissingData.Add(fromTime);
                fromTime = GetNextDateTime(fromTime, analyticTableQueryContext.TimeGroupingUnit.Value);
                while (fromTime <= toTime)
                {
                    timeForMissingData.Add(fromTime);
                    fromTime = GetNextDateTime(fromTime, analyticTableQueryContext.TimeGroupingUnit.Value);
                }
            }
            List<SubTableAnalyticRecordInProcessByParentGroupingKey> subTablesRecordsByParentGroupingKey;
            List<SubTableAnalyticRecordInProcessByGroupingKey> subTablesSummaryRecordsByGroupingKey;
            List<List<string>> subTablesOrderedDimensionKeys;
            BuildSubTablesRecordsFromDBRecords(subTablesDBRecords, subTablesSummaryDBRecords, analyticTableQueryContext, allDimensionNames, measureStyleRulesDictionary, 
                querySubTables, buildSummaryRecord, out resultSubTables, out subTablesRecordsByParentGroupingKey, out subTablesSummaryRecordsByGroupingKey, out subTablesOrderedDimensionKeys);
            foreach (var dbRecordEntry in groupedRecordsByDimensionsKey)
            {
                string recordGroupingKey = dbRecordEntry.Key;
                bool allMeasuresAreNull = true;
                AnalyticRecord analyticRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, dbRecordEntry.Value, requestedDimensionNames,
                    allDimensionNames, measureNames, measureStyleRulesDictionary, false, null, recordGroupingKey, null, out allMeasuresAreNull);
                if (querySubTables != null)
                    AddSubTablesMeasuresToRecord(analyticRecord, recordGroupingKey, analyticTableQueryContext, measureStyleRulesDictionary, querySubTables, subTablesRecordsByParentGroupingKey, subTablesOrderedDimensionKeys);

                if (analyticTableQueryContext.TimeGroupingUnit.HasValue)
                {
                    timeForMissingData.Remove(analyticRecord.Time.Value);
                }
                if (analyticTableQueryContext.TopRecords.HasValue)
                {
                    if (!allMeasuresAreNull)
                        analyticRecords.Add(analyticRecord);
                }
                else
                {
                    analyticRecords.Add(analyticRecord);
                }
            }
            if (timeForMissingData != null)
            {
                foreach (var dateTime in timeForMissingData)
                {
                    AnalyticRecord analyticRecord = CreateAnalyticRecordFilledWithDefaultValues(analyticTableQueryContext, measureNames, measureStyleRulesDictionary, dateTime);
                    analyticRecords.Add(analyticRecord);
                }
            }

            if (withSummary && summaryDBRecord != null)
            {
                bool allSummaryMeasuresAreNull;
                summaryRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, summaryDBRecord, null, allDimensionNames, measureNames, measureStyleRulesDictionary, 
                    true, null, null, null, out allSummaryMeasuresAreNull);
                if (querySubTables != null)
                    AddSubTablesMeasuresToSummaryRecord(summaryRecord, analyticTableQueryContext, measureStyleRulesDictionary, querySubTables, subTablesSummaryRecordsByGroupingKey, subTablesOrderedDimensionKeys);
            }

            else
            {
                summaryRecord = null;
            }
            return analyticRecords;
        }

        private void InitializeSubTablesInProcessObjects(List<AnalyticQuerySubTable> querySubTables, bool buildSummaryRecord, ref List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesDBRecords, ref List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesSummaryDBRecords)
        {
            subTablesDBRecords = new List<Dictionary<string, SubTableDBAnalyticRecordInProcess>>();
            if (buildSummaryRecord)
                subTablesSummaryDBRecords = new List<Dictionary<string, SubTableDBAnalyticRecordInProcess>>();
            foreach (var querySubTable in querySubTables)
            {
                subTablesDBRecords.Add(new Dictionary<string, SubTableDBAnalyticRecordInProcess>());
                if (buildSummaryRecord)
                    subTablesSummaryDBRecords.Add(new Dictionary<string, SubTableDBAnalyticRecordInProcess>());
            }
        }

        private void ReflectDBRecordToSubTablesDBRecords(DBAnalyticRecord dbRecord, IAnalyticTableQueryContext analyticTableQueryContext, string recordGroupingKey, List<AnalyticQuerySubTable> querySubTables, 
            List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesDBRecords, List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesSummaryDBRecords,
            bool buildSummaryRecord)
        {
            for (int subTableIndex = 0; subTableIndex < querySubTables.Count; subTableIndex++)
            {
                var querySubTable = querySubTables[subTableIndex];
                var subTableRecordsByDimensionsKey = subTablesDBRecords[subTableIndex];
                string subTableGroupingKey = GetDimensionGroupingKey(querySubTable.Dimensions, dbRecord);
                string fullSubTableGroupingKey = String.Concat(recordGroupingKey, "^*^", subTableGroupingKey);
                SubTableDBAnalyticRecordInProcess matchSubTableRecord;

                if (!subTableRecordsByDimensionsKey.TryGetValue(fullSubTableGroupingKey, out matchSubTableRecord))
                {
                    matchSubTableRecord = new SubTableDBAnalyticRecordInProcess
                    {
                        GroupingKey = subTableGroupingKey,
                        ParentRecordGroupingKey = recordGroupingKey,
                        Record = CloneAnalyticDBRecord(dbRecord)
                    };
                    subTableRecordsByDimensionsKey.Add(fullSubTableGroupingKey, matchSubTableRecord);
                }
                else
                {
                    UpdateAggregateValues(analyticTableQueryContext, matchSubTableRecord.Record, dbRecord);
                }
                foreach (var groupingValue in matchSubTableRecord.Record.GroupingValuesByDimensionName)
                {
                    if (groupingValue.Value.AllValues == null)
                        groupingValue.Value.AllValues = new List<dynamic>();
                    groupingValue.Value.AllValues.Add(dbRecord.GroupingValuesByDimensionName[groupingValue.Key].Value);
                }
                if (buildSummaryRecord)
                {
                    Dictionary<string, SubTableDBAnalyticRecordInProcess> subTableSummaryDBRecordsByDimensionKeys = subTablesSummaryDBRecords[subTableIndex];
                    SubTableDBAnalyticRecordInProcess matchSubTableSummaryRecord;

                    if (!subTableSummaryDBRecordsByDimensionKeys.TryGetValue(subTableGroupingKey, out matchSubTableSummaryRecord))
                    {
                        matchSubTableSummaryRecord = new SubTableDBAnalyticRecordInProcess
                        {
                            GroupingKey = subTableGroupingKey,
                            Record = CloneAnalyticDBRecord(dbRecord)
                        };
                        subTableSummaryDBRecordsByDimensionKeys.Add(subTableGroupingKey, matchSubTableSummaryRecord);
                    }
                    else
                    {
                        UpdateAggregateValues(analyticTableQueryContext, matchSubTableSummaryRecord.Record, dbRecord);
                    }
                    foreach (var groupingValue in matchSubTableSummaryRecord.Record.GroupingValuesByDimensionName)
                    {
                        if (groupingValue.Value.AllValues == null)
                            groupingValue.Value.AllValues = new List<dynamic>();
                        groupingValue.Value.AllValues.Add(dbRecord.GroupingValuesByDimensionName[groupingValue.Key].Value);
                    }
                }
            }
        }

        private void BuildSubTablesRecordsFromDBRecords(List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesDBRecords, List<Dictionary<string, SubTableDBAnalyticRecordInProcess>> subTablesSummaryDBRecords,
            IAnalyticTableQueryContext analyticTableQueryContext, HashSet<string> allDimensionNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary,
            List<AnalyticQuerySubTable> querySubTables, bool buildSummaryRecord, out List<AnalyticResultSubTable> resultSubTables,
            out List<SubTableAnalyticRecordInProcessByParentGroupingKey> subTablesRecordsByParentGroupingKey, out List<SubTableAnalyticRecordInProcessByGroupingKey> subTablesSummaryRecordsByGroupingKey, out List<List<string>> subTablesOrderedDimensionKeys)
        {
            resultSubTables = null;
            subTablesRecordsByParentGroupingKey = null;
            subTablesSummaryRecordsByGroupingKey = null;
            subTablesOrderedDimensionKeys = null;
            if (querySubTables != null)
            {
                resultSubTables = new List<AnalyticResultSubTable>();
                subTablesRecordsByParentGroupingKey = new List<SubTableAnalyticRecordInProcessByParentGroupingKey>();
                if (buildSummaryRecord)
                    subTablesSummaryRecordsByGroupingKey = new List<SubTableAnalyticRecordInProcessByGroupingKey>();
                subTablesOrderedDimensionKeys = new List<List<string>>();
                for (int subTableIndex = 0; subTableIndex < querySubTables.Count; subTableIndex++)
                {
                    var querySubTable = querySubTables[subTableIndex];
                    List<string> allQueryAndSubTableDimensionsList = new List<string>(allDimensionNames);
                    if (querySubTable.Dimensions != null)
                        allQueryAndSubTableDimensionsList.AddRange(querySubTable.Dimensions);
                    HashSet<string> allQueryAndSubTableDimensions = allQueryAndSubTableDimensionsList.ToHashSet();
                    var subTableDBRecords = subTablesDBRecords[subTableIndex];
                    SubTableAnalyticRecordInProcessByParentGroupingKey subTableRecordsByParentGroupingKey = new SubTableAnalyticRecordInProcessByParentGroupingKey();
                    subTablesRecordsByParentGroupingKey.Add(subTableRecordsByParentGroupingKey);
                    foreach (var subtableDBRecord in subTableDBRecords.Values)
                    {
                        bool subTableAllMeasuresAreNull;
                        AnalyticRecord subTableAnalyticRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, subtableDBRecord.Record, null,
                    allQueryAndSubTableDimensions, querySubTable.Measures, measureStyleRulesDictionary, false, subTableIndex, subtableDBRecord.ParentRecordGroupingKey, subtableDBRecord.GroupingKey, 
                    out subTableAllMeasuresAreNull);//no need to fill dimensions values for sub table records 
                        var subTableAnalyticRecordInProcess = new SubTableAnalyticRecordInProcess
                        {
                            GroupingKey = subtableDBRecord.GroupingKey,
                            ParentRecordGroupingKey = subtableDBRecord.ParentRecordGroupingKey,
                            Record = subTableAnalyticRecord
                        };
                        subTableRecordsByParentGroupingKey.GetOrCreateItem(subtableDBRecord.ParentRecordGroupingKey).Add(subTableAnalyticRecordInProcess.GroupingKey, subTableAnalyticRecordInProcess);
                    }

                    var subTableSummaryDBRecords = subTablesSummaryDBRecords[subTableIndex];
                    SubTableAnalyticRecordInProcessByGroupingKey subTableSummaryRecordsByGroupingKey = new SubTableAnalyticRecordInProcessByGroupingKey();
                    subTablesSummaryRecordsByGroupingKey.Add(subTableSummaryRecordsByGroupingKey);
                    var subTableSummaryRecords = new List<SubTableAnalyticRecordInProcess>();
                    foreach (var subTableSummaryDBRecord in subTableSummaryDBRecords.Values)
                    {
                        bool subTableSummaryAllMeasuresAreNull;
                        AnalyticRecord subTableSummaryAnalyticRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, subTableSummaryDBRecord.Record, querySubTable.Dimensions,
                    allQueryAndSubTableDimensions, querySubTable.Measures, measureStyleRulesDictionary, true, subTableIndex, null, subTableSummaryDBRecord.GroupingKey, 
                    out subTableSummaryAllMeasuresAreNull);//dimensions values are needed for summary sub table records to sort sub table records at the final stages
                        var subTableSummaryAnalyticRecordInProcess = new SubTableAnalyticRecordInProcess
                        {
                            GroupingKey = subTableSummaryDBRecord.GroupingKey,
                            Record = subTableSummaryAnalyticRecord
                        };
                        subTableSummaryRecordsByGroupingKey.Add(subTableSummaryAnalyticRecordInProcess.GroupingKey, subTableSummaryAnalyticRecordInProcess);
                        subTableSummaryRecords.Add(subTableSummaryAnalyticRecordInProcess);
                    }
                    if (querySubTable.OrderType.HasValue)
                    {
                        subTableSummaryRecords = GetOrderedAnalyticRecords(analyticTableQueryContext, querySubTable.OrderType.Value, querySubTable.Dimensions, querySubTable.Measures, querySubTable.AdvancedOrderOptions, subTableSummaryRecords, recordInProcexcc => recordInProcexcc.Record).ToList();
                    }
                    var resultSubTable = new AnalyticResultSubTable { DimensionValues = new List<DimensionValue[]>() };
                    resultSubTables.Add(resultSubTable);
                    List<string> subTableOrderedDimensionKeys = new List<string>();
                    subTablesOrderedDimensionKeys.Add(subTableOrderedDimensionKeys);
                    foreach (var subTableSummaryRecordInProcess in subTableSummaryRecords)
                    {
                        resultSubTable.DimensionValues.Add(subTableSummaryRecordInProcess.Record.DimensionValues);
                        subTableOrderedDimensionKeys.Add(subTableSummaryRecordInProcess.GroupingKey);
                    }
                }
            }
        }

        private void AddSubTablesMeasuresToRecord(AnalyticRecord analyticRecord, string recordGroupingKey, IAnalyticTableQueryContext analyticTableQueryContext, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary, 
            List<AnalyticQuerySubTable> querySubTables, List<SubTableAnalyticRecordInProcessByParentGroupingKey> subTablesRecordsByParentGroupingKey, List<List<string>> subTablesOrderedDimensionKeys)
        {
            analyticRecord.SubTables = new List<AnalyticRecordSubTable>();
            for (int subTableIndex = 0; subTableIndex < querySubTables.Count; subTableIndex++)
            {
                AnalyticRecordSubTable recordSubTable = new AnalyticRecordSubTable { MeasureValues = new List<MeasureValues>() };
                analyticRecord.SubTables.Add(recordSubTable);
                var querySubTable = querySubTables[subTableIndex];
                var subTableOrderedDimensionKeys = subTablesOrderedDimensionKeys[subTableIndex];
                var subTableRecordsByParentGroupingKey = subTablesRecordsByParentGroupingKey[subTableIndex];
                var subTableRecordsByGroupingKey = subTableRecordsByParentGroupingKey.GetRecord(recordGroupingKey);
                foreach (var dimensionKey in subTableOrderedDimensionKeys)
                {
                    SubTableAnalyticRecordInProcess matchSubTableRecord = subTableRecordsByGroupingKey.GetRecord(dimensionKey);
                    if (matchSubTableRecord != null)
                        recordSubTable.MeasureValues.Add(matchSubTableRecord.Record.MeasureValues);
                    else
                        recordSubTable.MeasureValues.Add(CreateMeasuresFilledWithDefaultValues(analyticTableQueryContext, querySubTable.Measures, measureStyleRulesDictionary));
                }
            }
        }

        private void AddSubTablesMeasuresToSummaryRecord(AnalyticRecord summaryRecord, IAnalyticTableQueryContext analyticTableQueryContext, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary,
            List<AnalyticQuerySubTable> querySubTables, List<SubTableAnalyticRecordInProcessByGroupingKey> subTablesSummaryRecordsByGroupingKey, List<List<string>> subTablesOrderedDimensionKeys)
        {
            summaryRecord.SubTables = new List<AnalyticRecordSubTable>();
            for (int subTableIndex = 0; subTableIndex < querySubTables.Count; subTableIndex++)
            {
                AnalyticRecordSubTable summaryRecordSubTable = new AnalyticRecordSubTable { MeasureValues = new List<MeasureValues>() };
                summaryRecord.SubTables.Add(summaryRecordSubTable);
                var querySubTable = querySubTables[subTableIndex];
                var subTableOrderedDimensionKeys = subTablesOrderedDimensionKeys[subTableIndex];
                var subTableSummaryRecordsByGroupingKey = subTablesSummaryRecordsByGroupingKey[subTableIndex];
                foreach (var dimensionKey in subTableOrderedDimensionKeys)
                {
                    SubTableAnalyticRecordInProcess matchSubTableSummaryRecord = subTableSummaryRecordsByGroupingKey.GetRecord(dimensionKey);
                    if (matchSubTableSummaryRecord != null)
                        summaryRecordSubTable.MeasureValues.Add(matchSubTableSummaryRecord.Record.MeasureValues);
                    else
                        summaryRecordSubTable.MeasureValues.Add(CreateMeasuresFilledWithDefaultValues(analyticTableQueryContext, querySubTable.Measures, measureStyleRulesDictionary));
                }
            }
        }

        private static DBAnalyticRecord CloneAnalyticDBRecord(DBAnalyticRecord dbRecord)
        {
            var summarySQLRecord = new DBAnalyticRecord { GroupingValuesByDimensionName = new Dictionary<string, DBAnalyticRecordGroupingValue>(), AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
            foreach (var groupingValueEntry in dbRecord.GroupingValuesByDimensionName)
            {
                summarySQLRecord.GroupingValuesByDimensionName.Add(groupingValueEntry.Key, groupingValueEntry.Value.Clone() as DBAnalyticRecordGroupingValue);
            }
            foreach (var aggValueEntry in dbRecord.AggValuesByAggName)
            {
                summarySQLRecord.AggValuesByAggName.Add(aggValueEntry.Key, aggValueEntry.Value.Clone() as DBAnalyticRecordAggValue);
            }
            return summarySQLRecord;
        }

        private AnalyticRecord CreateAnalyticRecordFilledWithDefaultValues(IAnalyticTableQueryContext analyticTableQueryContext, List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary, DateTime dateTime)
        {
            AnalyticRecord analyticRecord = new AnalyticRecord() { Time = dateTime, MeasureValues = CreateMeasuresFilledWithDefaultValues(analyticTableQueryContext, measureNames, measureStyleRulesDictionary) };            
            return analyticRecord;
        }

        private MeasureValues CreateMeasuresFilledWithDefaultValues(IAnalyticTableQueryContext analyticTableQueryContext, List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary)
        {
            var measureValues = new MeasureValues();
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
                measureValues.Add(measureName, new MeasureValue { Value = measureValue, StyleCode = styleCode });
            }
            return measureValues;
        }

        private DateTime GetNextDateTime(DateTime time, TimeGroupingUnit timeGroupingUnit)
        {
            switch (timeGroupingUnit)
            {
                case TimeGroupingUnit.Day:
                    return time.AddDays(1);
                case TimeGroupingUnit.Hour:
                    return time.AddHours(1);
                default:
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
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
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
                    {
                        if (!dimFilter.FilterValues.Contains(null))
                            return false;
                    }
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
            Func<string, object> getDimensionValue = (dimensionName) =>
            {
                DBAnalyticRecordGroupingValue groupingValue;
                if (record.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                {
                    return groupingValue.Value;
                }
                else
                    throw new NullReferenceException(String.Format("groupingValue. dimName '{0}'", dimensionName));
            };
            return GetRecordDimensionsGroupingKey(requestedDimensionNames, record.Time, getDimensionValue);
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

        private AnalyticRecord BuildAnalyticRecordFromSQLRecord(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord, List<string> dimensionNames,
            HashSet<string> allDimensionNames, List<string> measureNames, Dictionary<string, MeasureStyleRule> measureStyleRulesDictionary, 
            bool isSummaryRecord, int? subTableIndex, string recordGroupingKey, string subTableRecordGroupingKey, out bool allMeasuresAreNull)
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

            var getMeasureValueContext = new GetMeasureValueContext(analyticTableQueryContext, dbRecord, allDimensionNames, isSummaryRecord, subTableIndex, recordGroupingKey, subTableRecordGroupingKey);
            RecordFilterManager filterManager = new RecordFilterManager();
            allMeasuresAreNull = true;

            foreach (var measureName in measureNames)
            {
                var measureConfig = analyticTableQueryContext.GetMeasureConfig(measureName);
                var measureValue = measureConfig.Evaluator.GetMeasureValue(getMeasureValueContext);

                dynamic modifiedMeasureValue;
                DataRecordFieldType measureType = analyticTableQueryContext.GetMeasureConfig(measureName).Config.FieldType;
                if (measureType.CanRoundValue)
                    modifiedMeasureValue = measureType.GetRoundedValue(measureValue);
                else
                    modifiedMeasureValue = measureValue;

                string styleCode = null;
                if (measureStyleRulesDictionary != null)
                {
                    MeasureStyleRule measureStyleRule = null;
                    if (measureStyleRulesDictionary.TryGetValue(measureName, out measureStyleRule))
                    {
                        foreach (var rule in measureStyleRule.Rules)
                        {
                            if (rule.RecordFilter != null && filterManager.IsSingleFieldFilterMatch(rule.RecordFilter, modifiedMeasureValue, measureConfig.Config.FieldType))
                            {
                                styleCode = rule.StyleCode;
                                break;
                            }
                        }
                    }
                }

                if (measureValue != null)
                    allMeasuresAreNull = false;

                analyticRecord.MeasureValues.Add(measureName, new MeasureValue { Value = measureValue, StyleCode = styleCode });
            }

            return analyticRecord;
        }

        private static IEnumerable<T> GetOrderedAnalyticRecords<T>(IAnalyticTableQueryContext queryContext, AnalyticQueryOrderType orderType, List<string> dimensions, List<string> measures,
            AnalyticQueryAdvancedOrderOptionsBase advancedOrderOptions, IEnumerable<T> allRecords, Func<T, AnalyticRecord> getAnalyticRecord)
        {
            IEnumerable<T> orderedRecords;
            switch (orderType)
            {
                case AnalyticQueryOrderType.ByAllDimensions: orderedRecords = GetOrderedByAllDimensions(queryContext, dimensions, allRecords, getAnalyticRecord); break;
                case AnalyticQueryOrderType.ByAllMeasures: orderedRecords = GetOrderedByAllMeasures(measures, allRecords, getAnalyticRecord); break;
                case AnalyticQueryOrderType.AdvancedMeasureOrder: orderedRecords = GetOrderedByAllAdvancedMeasureOrder(measures, advancedOrderOptions, allRecords, getAnalyticRecord); break;
                default: orderedRecords = null; break;
            }
            return orderedRecords;
        }

        private static IEnumerable<T> GetOrderedByAllDimensions<T>(IAnalyticTableQueryContext queryContext, List<string> dimensions, IEnumerable<T> allRecords, Func<T, AnalyticRecord> getAnalyticRecord)
        {
            List<string> orderByDimensions = dimensions;
            if (orderByDimensions == null || orderByDimensions.Count == 0)
                throw new NullReferenceException("orderByDimensions");
            IOrderedEnumerable<T> orderedRecords;
            var firstDimensionConfig = queryContext.GetDimensionConfig(orderByDimensions[0]);
            if (firstDimensionConfig.Config.FieldType.OrderType == DataRecordFieldOrderType.ByFieldValue)
                orderedRecords = allRecords.OrderBy(record => getAnalyticRecord(record).DimensionValues[0].Value);
            else
                orderedRecords = allRecords.OrderBy(record => getAnalyticRecord(record).DimensionValues[0].Name);
            if (orderByDimensions.Count > 1)
            {
                for (int i = 1; i < orderByDimensions.Count; i++)
                {
                    var dimensionIndex = i;
                    var dimensionConfig = queryContext.GetDimensionConfig(orderByDimensions[dimensionIndex]);
                    if (dimensionConfig.Config.FieldType.OrderType == DataRecordFieldOrderType.ByFieldValue)
                        orderedRecords = orderedRecords.ThenBy(record => getAnalyticRecord(record).DimensionValues[dimensionIndex].Value);
                    else
                        orderedRecords = orderedRecords.ThenBy(record => getAnalyticRecord(record).DimensionValues[dimensionIndex].Name);
                }
            }
            return orderedRecords;
        }

        private static IEnumerable<T> GetOrderedByAllMeasures<T>(List<string> measures, IEnumerable<T> allRecords, Func<T, AnalyticRecord> getAnalyticRecord)
        {
            List<string> orderByMeasures = measures;
            if (orderByMeasures == null || orderByMeasures.Count() == 0)
                throw new NullReferenceException("orderByMeasures");
            string firstMeasureName = orderByMeasures[0];
            IOrderedEnumerable<T> orderedRecords = allRecords.OrderByDescending(record => getAnalyticRecord(record).MeasureValues[firstMeasureName].Value);
            if (orderByMeasures.Count > 1)
            {
                for (int i = 1; i < orderByMeasures.Count; i++)
                {
                    string measureName = orderByMeasures[i];
                    orderedRecords = orderedRecords.ThenByDescending(record => getAnalyticRecord(record).MeasureValues[measureName].Value);
                }
            }
            return orderedRecords;
        }

        private static IEnumerable<T> GetOrderedByAllAdvancedMeasureOrder<T>(List<string> measures, AnalyticQueryAdvancedOrderOptionsBase advancedOrderOptions, IEnumerable<T> allRecords, Func<T, AnalyticRecord> getAnalyticRecord)
        {
            if (measures == null)
                throw new NullReferenceException("measures");
            AnalyticQueryAdvancedMeasureOrderOptions advancedMeasureOrderOptions = advancedOrderOptions.CastWithValidate<AnalyticQueryAdvancedMeasureOrderOptions>("advancedOrderOptions");

            if (advancedMeasureOrderOptions.MeasureOrders == null || advancedMeasureOrderOptions.MeasureOrders.Count == 0)
                throw new NullReferenceException("advancedOrderOptions.MeasureOrders");
            var measureOrders = advancedMeasureOrderOptions.MeasureOrders;
            var firstMeasureOrder = measureOrders[0];
            if (!measures.Contains(firstMeasureOrder.MeasureName))
                throw new Exception(String.Format("Measure Order '{0}' is not available in the query measures", firstMeasureOrder.MeasureName));
            IOrderedEnumerable<T> orderedRecords = firstMeasureOrder.OrderDirection == OrderDirection.Ascending ?
                allRecords.OrderBy(record => getAnalyticRecord(record).MeasureValues[firstMeasureOrder.MeasureName].Value) :
                allRecords.OrderByDescending(record => getAnalyticRecord(record).MeasureValues[firstMeasureOrder.MeasureName].Value);
            if (measureOrders.Count > 1)
            {
                for (int i = 1; i < measureOrders.Count; i++)
                {
                    var measureOrder = measureOrders[i];
                    if (!measures.Contains(measureOrder.MeasureName))
                        throw new Exception(String.Format("Measure Order '{0}' is not available in the query measures", measureOrder.MeasureName));
                    orderedRecords = measureOrder.OrderDirection == OrderDirection.Ascending ?
                        orderedRecords.ThenBy(record => getAnalyticRecord(record).MeasureValues[measureOrder.MeasureName].Value) :
                        orderedRecords.ThenByDescending(record => getAnalyticRecord(record).MeasureValues[measureOrder.MeasureName].Value);
                }
            }
            return orderedRecords;
        }


        #endregion

        #region Private Classes

        private class AnalyticRecordRequestHandler : BigDataRequestHandler<AnalyticQuery, AnalyticRecord, AnalyticRecord>
        {
            AnalyticRecord _summaryRecord;

            public AnalyticRecordRequestHandler()
            {
            }

            public override AnalyticRecord EntityDetailMapper(AnalyticRecord entity)
            {
                return entity;
            }

            public override IEnumerable<AnalyticRecord> RetrieveAllData(DataRetrievalInput<AnalyticQuery> input)
            {
                var analyticManager = new AnalyticManager();
                return analyticManager.GetAllFilteredRecords(input.Query, true, out _summaryRecord);
            }

            protected override BigResult<AnalyticRecord> AllRecordsToBigResult(DataRetrievalInput<AnalyticQuery> input, IEnumerable<AnalyticRecord> allRecords)
            {
                var query = input.Query;
                IEnumerable<AnalyticRecord> orderedRecords;
                if (query.OrderType.HasValue)
                {
                    orderedRecords = AnalyticManager.GetOrderedAnalyticRecords(new AnalyticTableQueryContext(query), query.OrderType.Value, query.DimensionFields, query.MeasureFields, query.AdvancedOrderOptions, allRecords, record => record);
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

            protected override ResultProcessingHandler<AnalyticRecord> GetResultProcessingHandler(DataRetrievalInput<AnalyticQuery> input, BigResult<AnalyticRecord> bigResult)
            {
                return new ResultProcessingHandler<AnalyticRecord>
                {
                    ExportExcelHandler = new AnalyticExcelExportHandler(input.Query)
                };
            }
        }

        private class AnalyticMeasureExternalSourceContext : IAnalyticMeasureExternalSourceContext
        {
            public AnalyticQuery AnalyticQuery { get; set; }
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
                ExportExcelSheet sheet = new ExportExcelSheet() { Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() } };

                AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
                var dimensions = analyticItemConfigManager.GetDimensions(_query.TableId);
                dimensions.ThrowIfNull("dimensions", _query.TableId);
                var measures = analyticItemConfigManager.GetMeasures(_query.TableId);
                measures.ThrowIfNull("measures", _query.TableId);

                if (_query.DimensionFields != null)
                {
                    foreach (var dimName in _query.DimensionFields)
                    {
                        var excelHeaderCell = new ExportExcelHeaderCell { Title = dimName };
                        var setTypeContext = new DataRecordFieldTypeSetExcelCellTypeContext { HeaderCell = excelHeaderCell };

                        var dimension = dimensions.GetRecord(dimName);
                        dimension.ThrowIfNull("dimension", dimName);
                        dimension.Config.ThrowIfNull("dimension.Config", dimName);
                        dimension.Config.FieldType.ThrowIfNull("dimension.Config.FieldType", dimName);
                        dimension.Config.FieldType.SetExcelCellType(setTypeContext);
                        sheet.Header.Cells.Add(excelHeaderCell);
                    }
                }

                if (_query.MeasureFields != null)
                {
                    foreach (var measureName in _query.MeasureFields)
                    {
                        var excelHeaderCell = new ExportExcelHeaderCell { Title = measureName };
                        var setTypeContext = new DataRecordFieldTypeSetExcelCellTypeContext { HeaderCell = excelHeaderCell };

                        var measure = measures.GetRecord(measureName);
                        measure.ThrowIfNull("measure", measureName);
                        measure.Config.ThrowIfNull("measure.Config", measureName);
                        measure.Config.FieldType.ThrowIfNull("measure.Config.FieldType", measureName);
                        measure.Config.FieldType.SetExcelCellType(setTypeContext);
                        sheet.Header.Cells.Add(excelHeaderCell);
                    }
                }

                sheet.Rows = new List<ExportExcelRow>();

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var summary = ((AnalyticSummaryBigResult<AnalyticRecord>)(context.BigResult)).Summary;
                    if (summary != null)
                    {
                        var totalrow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        for (int k = 0; k < _query.DimensionFields.Count(); k++)
                            totalrow.Cells.Add(new ExportExcelCell() { Value = "" });

                        if (_query.MeasureFields != null)
                        {
                            foreach (var measureName in _query.MeasureFields)
                            {
                                MeasureValue measureValue;
                                if (!summary.MeasureValues.TryGetValue(measureName, out measureValue))
                                    throw new NullReferenceException(String.Format("measureValue. measureName '{0}'", measureName));

                                totalrow.Cells.Add(new ExportExcelCell { Value = measureValue.Value });
                            }
                        }
                        sheet.Rows.Add(totalrow);
                    }

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);

                        if (record.DimensionValues != null)
                        {
                            foreach (var dimValue in record.DimensionValues)
                                row.Cells.Add(new ExportExcelCell { Value = dimValue.Name });
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
                }

                context.MainSheet = sheet;
            }

            private class DataRecordFieldTypeSetExcelCellTypeContext : IDataRecordFieldTypeSetExcelCellTypeContext
            {
                public ExportExcelHeaderCell HeaderCell
                {
                    get;
                    set;
                }
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

        private class SubTableDBAnalyticRecordInProcess
        {
            public string GroupingKey { get; set; }

            public string ParentRecordGroupingKey { get; set; }

            public DBAnalyticRecord Record { get; set; }
        }

        private class SubTableAnalyticRecordInProcess
        {
            public string GroupingKey { get; set; }

            public string ParentRecordGroupingKey { get; set; }

            public AnalyticRecord Record { get; set; }
        }

        private class SubTableAnalyticRecordInProcessByGroupingKey : Dictionary<string, SubTableAnalyticRecordInProcess>
        {
        }

        private class SubTableAnalyticRecordInProcessByParentGroupingKey : Dictionary<string, SubTableAnalyticRecordInProcessByGroupingKey>
        {
        }

        #endregion
    }
}
