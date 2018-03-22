using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    internal class GetMeasureValueContext : IGetMeasureValueContext
    {
        IAnalyticTableQueryContext _analyticTableQueryContext;
        AnalyticQuery _query;
        string _recordGroupingKey;
        DBAnalyticRecord _sqlRecord;
        HashSet<string> _allDimensions;
        AnalyticRecord _analyticRecord;
        bool _isSummaryRecord;

        internal GetMeasureValueContext(IAnalyticTableQueryContext analyticTableQueryContext, string recordGroupingKey, DBAnalyticRecord sqlRecord, HashSet<string> allDimensions,
            AnalyticRecord analyticRecord, bool isSummaryRecord)
        {
            if (analyticTableQueryContext == null)
                throw new ArgumentNullException("analyticTableQueryContext");
            if (analyticTableQueryContext.Query == null)
                throw new ArgumentNullException("analyticTableQueryContext.Query");
            if (sqlRecord == null)
                throw new ArgumentNullException("sqlRecord");
            if (allDimensions == null)
                throw new ArgumentNullException("allDimensions");

            analyticRecord.ThrowIfNull("analyticRecord");
            _analyticTableQueryContext = analyticTableQueryContext;
            _query = _analyticTableQueryContext.Query;
            _recordGroupingKey = recordGroupingKey;
            _sqlRecord = sqlRecord;
            _allDimensions = allDimensions;
            _analyticRecord = analyticRecord;
            _isSummaryRecord = isSummaryRecord;
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
            return _analyticTableQueryContext.GetDimensionNamesFromQueryFilters().Contains(filterName);
        }

        public dynamic GetExternalSourceMatchRecordMeasureValue(string sourceName, string measureName)
        {
            var externalSourceRslt = _analyticTableQueryContext.GetMeasureExternalSourceResult(sourceName);
            if (externalSourceRslt != null)
            {
                var getValueContext = new AnalyticMeasureExternalSourceResultGetMatchRecordMesureValueContext
                {
                    Query = _query,
                    Record = _analyticRecord,
                    IsSummaryRecord = _isSummaryRecord,
                    MeasureName = measureName
                };
                return externalSourceRslt.GetMatchRecordMeasureValue(getValueContext);
            }
            else
            {
                return null;
            }
        }

        #region Private Methods

        private class AnalyticMeasureExternalSourceResultGetMatchRecordMesureValueContext : IAnalyticMeasureExternalSourceResultGetMatchRecordMesureValueContext
        {
            public AnalyticQuery Query { get; set; }

            public AnalyticRecord Record { get; set; }

            public bool IsSummaryRecord { get; set; }

            public string MeasureName { get; set; }
        }

        #endregion
    }
}