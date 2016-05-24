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

namespace Vanrise.Analytic.Business
{
    public class AnalyticManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            if (input.Query.FromTime == input.Query.ToTime)
                return null;
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();

            if (input.SortByColumnName.Contains("MeasureValues"))
            {
                string[] measureProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""]", measureProperty[0], measureProperty[1]);
            }
            SetDataManagerData(input.Query.TableId, dataManager);
            return BigDataManager.Instance.RetrieveData(input, new AnalyticRecordRequestHandler(dataManager));
        }

        private static void SetDataManagerData(int tableId, IAnalyticDataManager dataManager)
        {
            dataManager.AnalyticTableQueryContext = new AnalyticTableQueryContext(tableId);

        }

        #region Private Methods

        internal List<AnalyticRecord> ProcessSQLRecords(IAnalyticTableQueryContext analyticTableQueryContext, List<string> requestedDimensionNames, List<string> parentDimensionNames, List<string> measureNames, List<DimensionFilter> dimensionFilters,
           RecordFilterGroup filterGroup, IEnumerable<DBAnalyticRecord> dbRecords, HashSet<string> availableDimensions, bool withSummary, out AnalyticRecord summaryRecord)
        {
            List<string> allDimensionNamesList = new List<string>();
            if (requestedDimensionNames != null)
                allDimensionNamesList.AddRange(requestedDimensionNames);
            if (parentDimensionNames != null)
                allDimensionNamesList.AddRange(parentDimensionNames);
            HashSet<string> allDimensionNames = new HashSet<string>(allDimensionNamesList);
            FillCalculatedDimensions(analyticTableQueryContext, requestedDimensionNames, dbRecords, availableDimensions, dimensionFilters, filterGroup);           
            List<AnalyticRecord> records = ApplyFinalGroupingAndFiltering(analyticTableQueryContext, dbRecords,requestedDimensionNames, allDimensionNames, measureNames, dimensionFilters, filterGroup, withSummary, out summaryRecord);
            return records;
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
                        var getDimensionValueContext = new GetDimensionValueContext(sqlRecord);
                        sqlRecord.GroupingValuesByDimensionName.Add(dimToCalculate.Name, new DBAnalyticRecordGroupingValue
                        {
                            Value = dimToCalculate.Evaluator.GetDimensionValue(getDimensionValueContext)
                        });
                    }
                }
            }
        }

        private List<AnalyticRecord> ApplyFinalGroupingAndFiltering(IAnalyticTableQueryContext analyticTableQueryContext,  IEnumerable<DBAnalyticRecord> dbRecords, List<string> requestedDimensionNames, HashSet<string> allDimensionNames, List<string> measureNames,List<DimensionFilter> dimensionFilters, RecordFilterGroup filterGroup, bool withSummary, out AnalyticRecord summaryRecord)
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
                foreach(var groupingValue in matchRecord.GroupingValuesByDimensionName)
                {
                    if (groupingValue.Value.AllValues == null)
                        groupingValue.Value.AllValues = new List<dynamic>();
                    groupingValue.Value.AllValues.Add(dbRecord.GroupingValuesByDimensionName[groupingValue.Key].Value);
                }
                if (withSummary)
                {
                    if (summarySQLRecord == null)
                    {
                        summarySQLRecord = new DBAnalyticRecord { AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
                        foreach (var aggValueEntry in dbRecord.AggValuesByAggName)
                        {
                            summarySQLRecord.AggValuesByAggName.Add(aggValueEntry.Key, aggValueEntry.Value.Clone() as DBAnalyticRecordAggValue);
                        }
                    }
                    else
                        UpdateAggregateValues(analyticTableQueryContext, summarySQLRecord, dbRecord);
                }
            }
            List<AnalyticRecord> analyticRecords = new List<AnalyticRecord>();
            foreach (var dbRecord in groupedRecordsByDimensionsKey.Values)
            {
                AnalyticRecord analyticRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, dbRecord, requestedDimensionNames, allDimensionNames, measureNames);
                analyticRecords.Add(analyticRecord);
            }
            if (withSummary && summarySQLRecord != null)
                summaryRecord = BuildAnalyticRecordFromSQLRecord(analyticTableQueryContext, summarySQLRecord, null, allDimensionNames, measureNames);
            else
                summaryRecord = null;
            return analyticRecords;
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
                foreach(var dimFilter in dimensionFilters)
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
                            if (!nonNullFilterValues.Contains(Convert.ChangeType(dimensionValue.Value, nonNullFilterValues[0].GetType())))
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

        private AnalyticRecord BuildAnalyticRecordFromSQLRecord(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord, List<string> dimensionNames, HashSet<string> allDimensionNames, List<string> measureNames)
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

            AnalyticRecord _summaryRecord;
            public override IEnumerable<AnalyticRecord> RetrieveAllData(DataRetrievalInput<AnalyticQuery> input)
            {
                HashSet<string> includeDBDimensions;
                var dbRecords = _dataManager.GetAnalyticRecords(input, out includeDBDimensions);
                var query = input.Query;
                
                if (dbRecords != null)
                    return (new AnalyticManager()).ProcessSQLRecords(new AnalyticTableQueryContext(query.TableId), query.DimensionFields, query.ParentDimensions, query.MeasureFields, query.Filters, query.FilterGroup,
                        dbRecords, includeDBDimensions, query.WithSummary, out _summaryRecord);
                else
                    return null;
            }

            protected override BigResult<AnalyticRecord> AllRecordsToBigResult(DataRetrievalInput<AnalyticQuery> input, IEnumerable<AnalyticRecord> allRecords)
            {
                if (input.Query.TopRecords.HasValue)
                {
                    List<string> orderByMeasures = input.Query.MeasureFields;
                    if (orderByMeasures == null || orderByMeasures.Count() == 0)
                        throw new NullReferenceException("orderByMeasures");
                    string firstMeasureName = orderByMeasures[0];
                    IOrderedEnumerable<AnalyticRecord> orderedRecords = allRecords.OrderByDescending(record => record.MeasureValues[firstMeasureName]);
                    if (orderByMeasures.Count > 1)
                    {
                        for (int i = 1; i < orderByMeasures.Count; i++)
                        {
                            string measureName = orderByMeasures[i];
                            orderedRecords = orderedRecords.ThenByDescending(itm => itm.MeasureValues[measureName]);
                        }
                    }
                    return new AnalyticSummaryBigResult<AnalyticRecord>()
                    {
                        Data = orderedRecords.Take(input.Query.TopRecords.Value),
                        TotalCount = input.Query.TopRecords.Value
                    };
                }
                else
                {
                    var bigResult = allRecords.ToBigResult(input, null, (entity) => this.EntityDetailMapper(entity));
                    if (bigResult != null)
                    {
                        var analyticBigResult = new AnalyticSummaryBigResult<AnalyticRecord>()
                        {
                            ResultKey = bigResult.ResultKey,
                            Data = bigResult.Data,
                            TotalCount = bigResult.TotalCount
                        };
                        if (input.Query.WithSummary)
                            analyticBigResult.Summary = _summaryRecord;
                        return analyticBigResult;
                    }
                    else
                        return null;
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

        #endregion
    }
}
