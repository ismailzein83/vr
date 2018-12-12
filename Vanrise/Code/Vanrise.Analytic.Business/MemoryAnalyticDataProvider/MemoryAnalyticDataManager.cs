using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public abstract class MemoryAnalyticDataManager : IAnalyticDataManager
    {
        IAnalyticTableQueryContext _analyticTableQueryContext;
        public IAnalyticTableQueryContext QueryContext
        {
            get
            {
                return _analyticTableQueryContext;
            }
            set
            {
                _analyticTableQueryContext = value;
            }
        }

        public IEnumerable<DBAnalyticRecord> GetAnalyticRecords(IAnalyticDataManagerGetAnalyticRecordsContext context)
        {
            var query = context.Query;
            var includedSQLDimensions = context.AllQueryDBDimensions;
            var includedSQLDimensionsList = includedSQLDimensions.ToList();
            List<AnalyticDimension> dimensionConfigs = includedSQLDimensions.Select(dimName => GetDimensionConfig(dimName)).ToList();
            HashSet<string> includedSQLAggregateNames = context.AllQueryDBAggregates;
            List<AnalyticAggregate> aggregateConfigs = includedSQLAggregateNames.Select(aggName => GetAggregateConfig(aggName)).ToList();
            List<DBAnalyticRecord> groupedRecords = new List<DBAnalyticRecord>();
            List<string> neededFieldNames = new List<string>();
            neededFieldNames.AddRange(dimensionConfigs.Select(dimConfig => dimConfig.Config.SQLExpression).Distinct());
            neededFieldNames.AddRange(aggregateConfigs.Select(aggConfig => aggConfig.Config.SQLColumn).Distinct());
            var rawRecords = GetRawRecords(query, neededFieldNames);
            if (rawRecords != null && rawRecords.Count > 0)
            {
                Dictionary<string, StagingGroupedMemoryRecord> stagingGroupedRecordsByGroupingKey = new Dictionary<string, StagingGroupedMemoryRecord>();
                foreach (var rawRecord in rawRecords)
                {
                    rawRecord.FieldValues.ThrowIfNull("rawRecord.FieldValues");
                    Dictionary<string, DBAnalyticRecordGroupingValue> groupingValuesByDimName = new Dictionary<string, DBAnalyticRecordGroupingValue>();
                    foreach (var dimensionConfig in dimensionConfigs)
                    {
                        groupingValuesByDimName.Add(dimensionConfig.Name, new DBAnalyticRecordGroupingValue { Value = GetRawRecordFieldValueFromDimension(rawRecord, dimensionConfig) });
                    }
                    string groupingKey = AnalyticManager.GetRecordDimensionsGroupingKey(includedSQLDimensionsList, rawRecord.Time, (dimName) => groupingValuesByDimName[dimName].Value);
                    StagingGroupedMemoryRecord stagingGroupedRecord;
                    if (stagingGroupedRecordsByGroupingKey.TryGetValue(groupingKey, out stagingGroupedRecord))
                    {
                        foreach (var aggConfig in aggregateConfigs)
                        {
                            var aggFieldValue = GetRawRecordFieldValueFromAggregate(rawRecord, aggConfig);
                            stagingGroupedRecord.AggFieldValues[aggConfig.Name].Add(aggFieldValue);
                        }
                    }
                    else
                    {
                        stagingGroupedRecord = new StagingGroupedMemoryRecord { Time = rawRecord.Time, GroupingValuesByDimensionName = groupingValuesByDimName, AggFieldValues = new Dictionary<string, List<dynamic>>() };
                        foreach (var aggConfig in aggregateConfigs)
                        {
                            var aggFieldValue = GetRawRecordFieldValueFromAggregate(rawRecord, aggConfig);
                            stagingGroupedRecord.AggFieldValues.Add(aggConfig.Name, new List<dynamic> { aggFieldValue });
                        }
                        stagingGroupedRecordsByGroupingKey.Add(groupingKey, stagingGroupedRecord);
                    }
                }
                foreach (var groupedRecord in stagingGroupedRecordsByGroupingKey.Values)
                {
                    var dbRecord = new DBAnalyticRecord
                    {
                        Time = groupedRecord.Time,
                        GroupingValuesByDimensionName = groupedRecord.GroupingValuesByDimensionName,
                        AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>()
                    };
                    foreach (var aggFieldValuesEntry in groupedRecord.AggFieldValues)
                    {
                        string aggregateName = aggFieldValuesEntry.Key;
                        List<dynamic> aggFieldValues = aggFieldValuesEntry.Value;
                        AnalyticAggregateType aggregateType = GetAggregateConfig(aggregateName).Config.AggregateType;
                        dynamic aggregateValue;
                        switch (aggregateType)
                        {
                            case AnalyticAggregateType.Count: aggregateValue = aggFieldValues.Count; break;
                            case AnalyticAggregateType.Sum: aggregateValue = aggFieldValues.Where(itm => itm != null).Select<dynamic, Decimal>(itm => Convert.ToDecimal(itm)).Sum(itm => itm); break;
                            case AnalyticAggregateType.Max: aggregateValue = aggFieldValues.Max(); break;
                            case AnalyticAggregateType.Min: aggregateValue = aggFieldValues.Min(); break;
                            default: throw new NotSupportedException(String.Format("AggregateType '{0}'", aggregateType));
                        }
                        dbRecord.AggValuesByAggName.Add(aggregateName, new DBAnalyticRecordAggValue { Value = aggregateValue });
                    }
                    groupedRecords.Add(dbRecord);
                }
            }
            return groupedRecords;
        }
        
        private dynamic GetRawRecordFieldValueFromDimension(RawMemoryRecord rawRecord, AnalyticDimension dimensionConfig)
        {
            return GetRawRecordFieldValue(rawRecord, dimensionConfig.Config.SQLExpression);
        }

        private dynamic GetRawRecordFieldValueFromAggregate(RawMemoryRecord rawRecord, AnalyticAggregate aggregateConfig)
        {
            return GetRawRecordFieldValue(rawRecord, aggregateConfig.Config.SQLColumn);
        }

        private dynamic GetRawRecordFieldValue(RawMemoryRecord rawRecord, string fieldName)
        {
            dynamic fieldValue;
            if (!rawRecord.FieldValues.TryGetValue(fieldName, out fieldValue))
                throw new Exception(String.Format("Invalid Field name '{0}'", fieldName));
            return fieldValue;
        }

        public abstract List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames);

        #region Private Methods
        
        AnalyticTable GetTable()
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetTable();
        }

        AnalyticDimension GetDimensionConfig(string dimensionName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetDimensionConfig(dimensionName);
        }

        AnalyticAggregate GetAggregateConfig(string aggregateName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetAggregateConfig(aggregateName);
        }

        AnalyticMeasure GetMeasureConfig(string measureName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetMeasureConfig(measureName);
        }

        List<string> GetDimensionNamesFromQueryFilters()
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetDimensionNamesFromQueryFilters();
        }

        #endregion
    }



    public class RawMemoryRecord
    {
        public DateTime? Time { get; set; }

        public Dictionary<string, dynamic> FieldValues { get; set; }
    }

    public class StagingGroupedMemoryRecord
    {
        public DateTime? Time { get; set; }

        public Dictionary<string, DBAnalyticRecordGroupingValue> GroupingValuesByDimensionName { get; set; }

        public Dictionary<string, List<dynamic>> AggFieldValues { get; set; }
    }
}
