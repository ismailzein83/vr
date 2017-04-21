using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    internal class GetMeasureValueContext : IGetMeasureValueContext
    {
        AnalyticQuery _query;
        string _recordGroupingKey;
        DBAnalyticRecord _sqlRecord;
        HashSet<string> _allDimensions;
        IAnalyticTableQueryContext _analyticTableQueryContext;
        Dictionary<string, AnalyticMeasureExternalSourceProcessedResult> _measureExternalSourcesResults;
        internal GetMeasureValueContext(IAnalyticTableQueryContext analyticTableQueryContext, string recordGroupingKey, DBAnalyticRecord sqlRecord, HashSet<string> allDimensions, Dictionary<string, AnalyticMeasureExternalSourceProcessedResult> measureExternalSourcesResults)
        {
            if (analyticTableQueryContext == null)
                throw new ArgumentNullException("analyticTableQueryContext");
            if (analyticTableQueryContext.Query == null)
                throw new ArgumentNullException("analyticTableQueryContext.Query");
            if (sqlRecord == null)
                throw new ArgumentNullException("sqlRecord");
            if (allDimensions == null)
                throw new ArgumentNullException("allDimensions");
            _analyticTableQueryContext = analyticTableQueryContext;
            _query = _analyticTableQueryContext.Query;
            _recordGroupingKey = recordGroupingKey;
            _sqlRecord = sqlRecord;
            _allDimensions = allDimensions;
            _measureExternalSourcesResults = measureExternalSourcesResults;
        }
        public dynamic GetAggregateValue(string aggregateName)
        {
            DBAnalyticRecordAggValue aggValue;
            if (!_sqlRecord.AggValuesByAggName.TryGetValue(aggregateName, out aggValue))
                throw new NullReferenceException(String.Format("aggValue. AggName '{0}'", aggregateName));
            return aggValue.Value;
        }

        public bool IsGroupingDimensionIncluded(string dimensionName)
        {
            return _allDimensions.Contains(dimensionName);
        }

        public List<dynamic> GetAllDimensionValues(string dimensionName)
        {
            DBAnalyticRecordGroupingValue groupingValue;
            if (!_sqlRecord.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                throw new NullReferenceException(String.Format("groupingValue. dimensionName '{0}'", dimensionName));
            var allValues = groupingValue.AllValues;
            if (allValues == null)
                throw new NullReferenceException("allValues");
            return allValues;
        }

        public List<dynamic> GetDistinctDimensionValues(string dimensionName)
        {
            return GetAllDimensionValues(dimensionName).Distinct().ToList();
        }

        public DateTime GetQueryFromTime()
        {
            return _query.FromTime;
        }

        public DateTime GetQueryToTime()
        {
            return _query.ToTime.HasValue ? _query.ToTime.Value : DateTime.Now;
        }

        public bool IsFilterIncluded(string filterName)
        {
            return _analyticTableQueryContext.GetDimensionNames(_query.FilterGroup).Contains(filterName);
        }


        public object GetExternalSourceValue(string sourceName, string measureName)
        {
            _measureExternalSourcesResults.ThrowIfNull("_measureExternalSourcesResults");
            AnalyticMeasureExternalSourceProcessedResult sourceResult;
            if (!_measureExternalSourcesResults.TryGetValue(sourceName, out sourceResult))
                throw new NullReferenceException(String.Format("sourceResult. Name '{0}'", sourceName));
            Dictionary<string, Object> measureValues;
            if(_recordGroupingKey != null)
            {
                AnalyticMeasureExternalSourceRecord matchRecord;
                if (sourceResult.RecordsByDimensionKey != null && sourceResult.RecordsByDimensionKey.TryGetValue(_recordGroupingKey, out matchRecord))
                    measureValues = matchRecord.MeasureValues;
                else
                    return null;
            }
            else//in case of summary record
            {
                measureValues = sourceResult.OriginalResult.SummaryMeasureValues;
            }
            if (measureValues == null)
                throw new NullReferenceException(string.Format("measureValues. sourceName '{0}', grouping Key '{1}'", sourceName, _recordGroupingKey));
            Object measureValue;
            if (!measureValues.TryGetValue(measureName, out measureValue))
                throw new NullReferenceException(String.Format("measureValue. Name '{0}' sourceName '{1}'", measureName, sourceName));
            return measureValue;
        }
    }
}
