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
        List<string> mappedQueryDimensionNames = new List<string>();

        AnalyticManager _analyticManager = new AnalyticManager();
        Dictionary<string, AnalyticRecord> _recordsByDimensionKey = new Dictionary<string, AnalyticRecord>();

        public AnalyticTableMeasureExternalSourceResult(List<AnalyticRecord> records, AnalyticRecord summaryRecord, AnalyticQuery mappedQuery, Dictionary<string, string> mappedDimensionNames)
        {
            _qPendingRecords = new Queue<AnalyticRecord>(records);
            _summaryRecord = summaryRecord;
            _mappedQuery = mappedQuery;
            if (mappedDimensionNames != null)
            {
                foreach (var entry in mappedDimensionNames)
                {
                    if (mappedQuery.DimensionFields != null && mappedQuery.DimensionFields.Contains(entry.Value))
                    {
                        receivedQueryDimensionNames.Add(entry.Key);
                        mappedQueryDimensionNames.Add(entry.Value);
                    }
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
                string recordToMatchDimensionGroupingKey = GetDimensionsGroupingKey(receivedQueryDimensionNames, context.Record);

                if (!_recordsByDimensionKey.TryGetValue(recordToMatchDimensionGroupingKey, out matchRecord))
                {
                    while (_qPendingRecords.Count > 0)
                    {
                        var record = _qPendingRecords.Dequeue();
                        string recordDimensionGroupingKey = GetDimensionsGroupingKey(mappedQueryDimensionNames, record);
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
                return GetMeasureValue(matchRecord, context.MeasureName);
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

        private string GetDimensionsGroupingKey(List<string> dimensions, AnalyticRecord record)
        {
            Func<string, object> getDimensionValue = (dimName) =>
            {
                dimensions.ThrowIfNull("dimensions");
                record.DimensionValues.ThrowIfNull("record.DimensionValues");
                int dimIndex = dimensions.IndexOf(dimName);
                if (dimIndex < 0)
                    throw new Exception(String.Format("Dimension '{0}' is not found in query dimensions", dimName));
                if (record.DimensionValues.Length <= dimIndex)
                    throw new Exception(String.Format("record.DimensionValues.Length '{0}' <= dimIndex '{1}'", record.DimensionValues.Length, dimIndex));
                return record.DimensionValues[dimIndex].Value;
            };
            return AnalyticManager.GetRecordDimensionsGroupingKey(dimensions, record.Time, getDimensionValue);
        }
    }
}
