using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Vanrise.Analytic.Business
{
    public class AnalyticManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();

            IDataRetrievalResult<AnalyticRecord> analyticRecords;
            if (input.SortByColumnName.Contains("MeasureValues"))
            {
                string[] measureProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""]", measureProperty[0], measureProperty[1]);
            }
            SetDataManagerData(input.Query.TableId, dataManager);
            analyticRecords = BigDataManager.Instance.RetrieveData(input, new AnalyticRecordRequestHandler(dataManager));

            if (analyticRecords != null)
            {
                BigResult<AnalyticRecord> bigResult = analyticRecords as BigResult<AnalyticRecord>;
                if (bigResult != null)
                {
                    var rslt = new AnalyticSummaryBigResult<AnalyticRecord>()
                    {
                        ResultKey = bigResult.ResultKey,
                        Data = bigResult.Data,
                        TotalCount = bigResult.TotalCount
                    };
                    if (input.Query.WithSummary)
                        rslt.Summary = dataManager.GetAnalyticSummary(input);
                    return rslt;
                }
                else
                    return analyticRecords;
            }
            else
                return null;
        }

        private static void SetDataManagerData(int tableId, IAnalyticDataManager dataManager)
        {
            dataManager.AnalyticTableQueryContext = new AnalyticTableQueryContext(tableId);

        }

        public Vanrise.Entities.IDataRetrievalResult<TimeVariationAnalyticRecord> GetTimeVariationAnalyticRecords(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();

            IDataRetrievalResult<TimeVariationAnalyticRecord> timeVariationAnalyticRecords;
            if (input.SortByColumnName.Contains("MeasureValues"))
            {
                string[] measureProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""]", measureProperty[0], measureProperty[1]);
            }
            SetDataManagerData(input.Query.TableId, dataManager);
            timeVariationAnalyticRecords = BigDataManager.Instance.RetrieveData(input, new TimeVariationAnalyticRecordRequestHandler(dataManager));

            if (timeVariationAnalyticRecords != null)
            {
                BigResult<TimeVariationAnalyticRecord> bigResult = timeVariationAnalyticRecords as BigResult<TimeVariationAnalyticRecord>;
                if (bigResult != null)
                {
                    var rslt = new TimeVariationAnalyticBigResult()
                    {
                        ResultKey = bigResult.ResultKey,
                        Data = bigResult.Data,
                        TotalCount = bigResult.TotalCount
                    };
                    //if (input.Query.WithSummary)
                    //    rslt.Summary = dataManager.GetAnalyticSummary(input);
                    return rslt;
                }
                else
                    return timeVariationAnalyticRecords;
            }
            else
                return null;
        }

        #region Private Methods

        internal List<AnalyticRecord> ProcessSQLRecords(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, List<string> parentDimensionNames, List<string> measureNames, List<DimensionFilter> dimensionFiltes,
           RecordFilterGroup filterGroup, IEnumerable<DBAnalyticRecord> dbRecords, HashSet<string> availableDimensions, bool withSummary, out AnalyticRecord summaryRecord)
        {
            List<string> allDimensionNamesList = new List<string>();
            if (requestedDimensionNames != null)
                allDimensionNamesList.AddRange(requestedDimensionNames);
            if (parentDimensionNames != null)
                allDimensionNamesList.AddRange(parentDimensionNames);
            HashSet<string> allDimensionNames = new HashSet<string>(allDimensionNamesList);
            FillCalculatedDimensions(analyticTableQueryContext, requestedDimensionNames, dbRecords, availableDimensions);           
            List<AnalyticRecord> records = ApplyFinalGrouping(analyticTableQueryContext, requestedDimensionNames, allDimensionNames, measureNames, dbRecords, withSummary, out summaryRecord);
            var filteredRecords = ApplyFilters(analyticTableQueryContext, requestedDimensionNames, dimensionFiltes, filterGroup, records);
            return filteredRecords;
        }

        private void FillCalculatedDimensions(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, IEnumerable<DBAnalyticRecord> sqlRecords, HashSet<string> availableDimensions)
        {
            IEnumerable<AnalyticDimension> dimensionsToCalculate = requestedDimensionNames.Where(dimName => !availableDimensions.Contains(dimName)).Select(dimName => analyticTableQueryContext.GetDimensionConfig(dimName));
            foreach (var sqlRecord in sqlRecords)
            {
                foreach (var dimToCalculate in dimensionsToCalculate)
                {
                    var getDimensionValueContext = new GetDimensionValueContext(sqlRecord);
                    sqlRecord.GroupingValuesByDimensionName.Add(dimToCalculate.Name, new DBAnalyticRecordGroupingValue
                    {
                        Value = dimToCalculate.Evaluator.GetDimensionValue(getDimensionValueContext)
                    });
                }
            }
        }

        private List<AnalyticRecord> ApplyFilters(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, List<DimensionFilter> dimensionFiltes, RecordFilterGroup filterGroup, IEnumerable<AnalyticRecord> records)
        {
            List<AnalyticRecord> filteredRecords = new List<AnalyticRecord>();
            RecordFilterManager recordFilterManager = new RecordFilterManager();
            foreach(var record in records)
            {
                if (filterGroup != null)
                {
                    RecordFilterGenericFieldMatchContext recordFilterContext = new RecordFilterGenericFieldMatchContext(analyticTableQueryContext, record, requestedDimensionNames);
                    if (!recordFilterManager.IsFilterGroupMatch(filterGroup, recordFilterContext))
                        continue;
                }
                filteredRecords.Add(record);
            }
            return filteredRecords;
        }

        private List<AnalyticRecord> ApplyFinalGrouping(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, HashSet<string> allDimensionNames, List<string> measureNames, IEnumerable<DBAnalyticRecord> dbRecords, bool withSummary, out AnalyticRecord summaryRecord)
        {
            Dictionary<string, DBAnalyticRecord> groupedRecordsByDimensionsKey = new Dictionary<string, DBAnalyticRecord>();
            DBAnalyticRecord summarySQLRecord = new DBAnalyticRecord() { AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
            foreach (var sqlRecord in dbRecords)
            {
                string groupingKey = GetDimensionGroupingKey(requestedDimensionNames, sqlRecord);
                DBAnalyticRecord matchRecord;
                if (!groupedRecordsByDimensionsKey.TryGetValue(groupingKey, out matchRecord))
                {
                    groupedRecordsByDimensionsKey.Add(groupingKey, sqlRecord);
                }
                else
                {
                    UpdateAggregateValues(analyticTableQueryContext, matchRecord, sqlRecord);
                }
                if (withSummary)
                    UpdateAggregateValues(analyticTableQueryContext, summarySQLRecord, sqlRecord);
            }
            List<AnalyticRecord> analyticRecords = new List<AnalyticRecord>();
            foreach (var sqlRecord in groupedRecordsByDimensionsKey.Values)
            {
                AnalyticRecord analyticRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, sqlRecord, requestedDimensionNames, allDimensionNames, measureNames);
                analyticRecords.Add(analyticRecord);
            }
            if (withSummary)
                summaryRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, summarySQLRecord, null, allDimensionNames, measureNames);
            else
                summaryRecord = null;
            return analyticRecords;
        }

        private string GetDimensionGroupingKey(List<string> requestedDimensionNames, DBAnalyticRecord record)
        {
            StringBuilder builder = new StringBuilder();
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
            return builder.ToString();
        }

        private void UpdateAggregateValues(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord existingRecord, DBAnalyticRecord record)
        {
            foreach (var aggEntry in record.AggValuesByAggName)
            {
                var existingAgg = record.AggValuesByAggName[aggEntry.Key];
                var agg = aggEntry.Value;
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

        private AnalyticRecord BuildAnalyticRecordFromSQLRecord(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord, List<string> dimensionNames, HashSet<string> allDimensionNames, List<string> measureNames)
        {
            AnalyticRecord analyticRecord = new AnalyticRecord() { Time = dbRecord.Time, DimensionValues = new DimensionValue[dimensionNames.Count], MeasureValues = new MeasureValues() };

            if (dimensionNames != null)
            {
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
                var getMeasureValueContext = new GetMeasureValueContext(dbRecord, allDimensionNames);
                var measureValue = measureConfig.Evaluator.GetMeasureValue(getMeasureValueContext);
                analyticRecord.MeasureValues.Add(measureName, measureValue);
            }
            return analyticRecord;
        }

        #endregion

        #region Private Classes

        private class AnalyticRecordRequestHandler : BigDataRequestHandler<AnalyticQuery, AnalyticRecord, AnalyticRecord>
        {
            IAnalyticDataManager _dataManager;
            public AnalyticRecordRequestHandler(IAnalyticDataManager dataManager)
            {
                _dataManager = dataManager;
            }
            public override AnalyticRecord EntityDetailMapper(AnalyticRecord entity)
            {
                return entity;
            }

            public override IEnumerable<AnalyticRecord> RetrieveAllData(DataRetrievalInput<AnalyticQuery> input)
            {
                HashSet<string> includeDBDimensions;
                var dbRecords = _dataManager.GetAnalyticRecords(input, out includeDBDimensions);
                var query = input.Query;
                AnalyticRecord summaryRecord;
                if (dbRecords != null)
                    return (new AnalyticManager()).ProcessSQLRecords(new AnalyticTableQueryContext(query.TableId), query.DimensionFields, query.ParentDimensions, query.MeasureFields, query.Filters, query.FilterGroup,
                        dbRecords, includeDBDimensions, query.WithSummary, out summaryRecord);
                else
                    return null;
            }

        }

        private class TimeVariationAnalyticRecordRequestHandler : BigDataRequestHandler<TimeVariationAnalyticQuery, TimeVariationAnalyticRecord, TimeVariationAnalyticRecord>
        {
            IAnalyticDataManager _dataManager;
            public TimeVariationAnalyticRecordRequestHandler(IAnalyticDataManager dataManager)
            {
                _dataManager = dataManager;
            }
            public override TimeVariationAnalyticRecord EntityDetailMapper(TimeVariationAnalyticRecord entity)
            {
                return entity;
            }

            public override IEnumerable<TimeVariationAnalyticRecord> RetrieveAllData(DataRetrievalInput<TimeVariationAnalyticQuery> input)
            {
                return _dataManager.GetTimeVariationAnalyticRecords(input);
            }

        }

        private class RecordFilterGenericFieldMatchContext : IRecordFilterGenericFieldMatchContext
        {
            IAnalyticTableQueryContext _analyticTableQueryContext;
            AnalyticRecord _record;
            List<string> _requestedDimensionNames;

            public RecordFilterGenericFieldMatchContext(IAnalyticTableQueryContext analyticTableQueryContext, AnalyticRecord record, List<string> requestedDimensionNames)
            {
                if (_analyticTableQueryContext == null)
                    throw new ArgumentNullException("analyticTableQueryContext");
                if (record == null)
                    throw new ArgumentNullException("record");
                if (requestedDimensionNames == null)
                    throw new ArgumentNullException("requestedDimensionNames");
                _analyticTableQueryContext = analyticTableQueryContext;
                _record = record;
                _requestedDimensionNames = requestedDimensionNames;
            }
            public object GetFieldValue(string fieldName, out DataRecordFieldType fieldType)
            {
                string[] fieldParts = fieldName.Split('_');
                if(fieldParts[0] == "Dimension")
                {
                    string dimensionName = fieldParts[1];
                    var dimensionConfig = _analyticTableQueryContext.GetDimensionConfig(dimensionName);
                    fieldType = dimensionConfig.Config.FieldType;
                    int dimensionIndex = _requestedDimensionNames.IndexOf(dimensionName);
                    if (dimensionIndex < 0)
                        throw new Exception("dimensionIndex is negative");
                    if (_record.DimensionValues.Length <= dimensionIndex)
                        throw new Exception(String.Format("nb of dimensions {0} is less than dimensionIndex '{1}'", _record.DimensionValues.Length, dimensionIndex));
                    return _record.DimensionValues[dimensionIndex];
                }
                else
                {
                    string measureName = fieldParts[1];
                    var measureConfig = _analyticTableQueryContext.GetMeasureConfig(measureName);
                    fieldType = measureConfig.Config.FieldType;
                    Object measureValue;
                    if (!_record.MeasureValues.TryGetValue(measureName, out measureValue))
                        throw new NullReferenceException(String.Format("measureValue. measure name '{0}'", measureName));
                    return measureValue;
                }
            }
        }

        #endregion
    }
}
