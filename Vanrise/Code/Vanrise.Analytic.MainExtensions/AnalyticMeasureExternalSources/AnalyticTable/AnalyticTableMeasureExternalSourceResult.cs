using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable
{
    public class AnalyticTableMeasureExternalSourceResult : AnalyticMeasureExternalSourceResult
    {
        Queue<AnalyticRecord> _qPendingRecords;
        AnalyticRecord _summaryRecord;
        AnalyticQuery _mappedQuery;
        List<string> receivedQueryDimensionNames = new List<string>();
        
        AnalyticManager _analyticManager = new AnalyticManager();
        Dictionary<string, AnalyticRecord> _recordsByDimensionKey = new Dictionary<string, AnalyticRecord>();
        List<AnalyticResultSubTable> _resultSubTables;
        Dictionary<int, SubTableMatching> _subTableMatchingsByReceivedTableIndex = new Dictionary<int, SubTableMatching>();

        public AnalyticTableMeasureExternalSourceResult(List<AnalyticRecord> records, AnalyticRecord summaryRecord, AnalyticQuery mappedQuery,
            Dictionary<string, string> dimensionMappings, Dictionary<int, int> subTablesIndexMappings, List<AnalyticResultSubTable> resultSubTables)
        {
            _qPendingRecords = new Queue<AnalyticRecord>(records);
            _summaryRecord = summaryRecord;
            _mappedQuery = mappedQuery;
            _resultSubTables = resultSubTables;
            Dictionary<string, string> dimensionMappingsByMappedDimension = new Dictionary<string, string>();
            foreach (var entry in dimensionMappings)
            {
                if (!dimensionMappingsByMappedDimension.ContainsKey(entry.Value))
                    dimensionMappingsByMappedDimension.Add(entry.Value, entry.Key);
            }
            if (mappedQuery.DimensionFields != null)
            {
                foreach (var mappedDimension in mappedQuery.DimensionFields)
                {
                    receivedQueryDimensionNames.Add(dimensionMappingsByMappedDimension[mappedDimension]);
                }
            }
            if (subTablesIndexMappings != null)
            {
                foreach (var subTableIndexMappingEntry in subTablesIndexMappings)
                {
                    int receivedSubTableIndex = subTableIndexMappingEntry.Key;
                    int mappedSubTableIndex = subTableIndexMappingEntry.Value;
                    _subTableMatchingsByReceivedTableIndex.Add(receivedSubTableIndex, new SubTableMatching(mappedSubTableIndex, mappedQuery, resultSubTables, dimensionMappingsByMappedDimension));
                }
            }
        }

        public override dynamic GetMatchRecordMeasureValue(IAnalyticMeasureExternalSourceResultGetMatchRecordMesureValueContext context)
        {
            AnalyticRecord matchRecord;
            if (context.IsSummaryRecord)
            {                
                matchRecord = _summaryRecord;
            }
            else
            {
                string recordToMatchDimensionGroupingKey = GetDimensionsGroupingKey(receivedQueryDimensionNames, context.DBRecord);

                if (!_recordsByDimensionKey.TryGetValue(recordToMatchDimensionGroupingKey, out matchRecord))
                {
                    while (_qPendingRecords.Count > 0)
                    {
                        var record = _qPendingRecords.Dequeue();
                        string recordDimensionGroupingKey = GetDimensionValuesGroupingKey(record.Time, record.DimensionValues);
                        _recordsByDimensionKey.Add(recordDimensionGroupingKey, record);
                        if (recordDimensionGroupingKey == recordToMatchDimensionGroupingKey)
                        {
                            matchRecord = record;
                            break;
                        }
                    }
                }
            }

            if (matchRecord != null)
            {
                if (!context.SubTableIndex.HasValue)
                    return GetMeasureValue(matchRecord, context.MeasureName);
                else
                    return GetSubTableMeasureValue(matchRecord, context.SubTableIndex.Value, context.DBRecord, context.MeasureName);
            }
            else
                return null;
        }

        private Object GetMeasureValue(AnalyticRecord record, string measureName)
        {
            record.MeasureValues.ThrowIfNull("record.MeasureValues");
            MeasureValue measureValue;
            if (!record.MeasureValues.TryGetValue(measureName, out measureValue))
                throw new NullReferenceException(String.Format("measureValue '{0}'", measureName));
            return measureValue.Value;
        }

        private dynamic GetSubTableMeasureValue(AnalyticRecord matchRecord, int receivedSubTableIndex, DBAnalyticRecord receivedDBRecord, string measureName)
        {
            SubTableMatching subTableMapping;
            if (!_subTableMatchingsByReceivedTableIndex.TryGetValue(receivedSubTableIndex, out subTableMapping))
                throw new NullReferenceException(String.Format("subTableMapping '{0}'", receivedSubTableIndex));
            return subTableMapping.GetMeasureValue(matchRecord, receivedDBRecord, measureName);
        }

        private static string GetDimensionValuesGroupingKey(DateTime? recordTime, DimensionValue[] dimensionValues)
        {
            return AnalyticManager.GetDimensionValuesGroupingKey(recordTime, dimensionValues);
        }

        private static string GetDimensionsGroupingKey(List<string> dimensions, DBAnalyticRecord record)
        {
            Func<string, object> getDimensionValue = (dimName) =>
            {
                dimensions.ThrowIfNull("dimensions");
                record.GroupingValuesByDimensionName.ThrowIfNull("record.DimensionValues");
                DBAnalyticRecordGroupingValue dimensionValue;
                if (!record.GroupingValuesByDimensionName.TryGetValue(dimName, out dimensionValue))
                    throw new Exception(String.Format("Dimension '{0}' is not found in record GroupingValuesByDimensionName", dimensionValue));
                return dimensionValue.Value;
            };
            return AnalyticManager.GetRecordDimensionsGroupingKey(dimensions, record.Time, getDimensionValue);
        }

        #region Private Classes

        private class SubTableMatching
        {
            List<string> _receivedDimensionNames;

            public Queue<DimensionValue[]> _qPendingDimensionValues;

            List<string> _orderedDimensionKeys = new List<string>();

            int _mappedSubTableIndex;

            public SubTableMatching(int mappedSubTableIndex, AnalyticQuery mappedQuery, List<AnalyticResultSubTable> resultSubTables, Dictionary<string, string> dimensionMappingsByMappedDimension)
            {
                _mappedSubTableIndex = mappedSubTableIndex;
                
                AnalyticQuerySubTable mappedQuerySubTable = mappedQuery.SubTables[mappedSubTableIndex];
                if (mappedQuerySubTable.Dimensions != null)
                {
                    _receivedDimensionNames = new List<string>();
                    foreach (var mappedDimension in mappedQuerySubTable.Dimensions)
                    {
                        _receivedDimensionNames.Add(dimensionMappingsByMappedDimension[mappedDimension]);
                    }
                }

                AnalyticResultSubTable resultSubTable = resultSubTables[mappedSubTableIndex];
                _qPendingDimensionValues = new Queue<DimensionValue[]>();
                foreach (var dimValues in resultSubTable.DimensionValues)
                {
                    _qPendingDimensionValues.Enqueue(dimValues);
                }
            }

            public dynamic GetMeasureValue(AnalyticRecord record, DBAnalyticRecord receivedDBRecord, string measureName)
            {                
                string recordToMatchDimensionGroupingKey = GetDimensionsGroupingKey(_receivedDimensionNames, receivedDBRecord);

                int dimensionKeyIndex = _orderedDimensionKeys.IndexOf(recordToMatchDimensionGroupingKey);
                if(dimensionKeyIndex < 0)
                {
                    while (_qPendingDimensionValues.Count > 0)
                    {
                        DimensionValue[] dimensionValues = _qPendingDimensionValues.Dequeue();
                        string groupingKey = GetDimensionValuesGroupingKey(null, dimensionValues);
                        _orderedDimensionKeys.Add(groupingKey);
                        dimensionKeyIndex++;
                        if (groupingKey == recordToMatchDimensionGroupingKey)
                        {
                            break;
                        }
                    }
                }

                record.SubTables.ThrowIfNull("record.SubTables");
                if(record.SubTables.Count <= _mappedSubTableIndex)
                    throw new Exception(String.Format("record.SubTables.Count is less than _mappedSubTableIndex. _mappedSubTableIndex: {0}", _mappedSubTableIndex));
                var recordSubTable = record.SubTables[_mappedSubTableIndex];
                recordSubTable.MeasureValues.ThrowIfNull("recordSubTable.MeasureValues");
                if (recordSubTable.MeasureValues.Count <= dimensionKeyIndex)
                    throw new Exception(String.Format("recordSubTable.MeasureValues.Count is less than dimensionKeyIndex. dimensionKeyIndex: {0}", dimensionKeyIndex));
                MeasureValues matchMeasureValues = recordSubTable.MeasureValues[dimensionKeyIndex];
                MeasureValue measureValue;
                if (!matchMeasureValues.TryGetValue(measureName, out measureValue))
                    throw new NullReferenceException(String.Format("measureValue '{0}'", measureName));
                return measureValue.Value;
            }
        }

        #endregion
    }
}
