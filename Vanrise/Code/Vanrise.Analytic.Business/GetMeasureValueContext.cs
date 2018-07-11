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
        DBAnalyticRecord _dbRecord;
        HashSet<string> _allDimensions;
        bool _isSummaryRecord;
        int? _subTableIndex;
        string _recordGroupingKey;
        string _subTableRecordGroupingKey;

        internal GetMeasureValueContext(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord dbRecord, HashSet<string> allDimensions,
            bool isSummaryRecord, int? subTableIndex, string recordGroupingKey, string subTableRecordGroupingKey)
        {
            if (analyticTableQueryContext == null)
                throw new ArgumentNullException("analyticTableQueryContext");
            if (dbRecord == null)
                throw new ArgumentNullException("dbRecord");
            if (allDimensions == null)
                throw new ArgumentNullException("allDimensions");

            _analyticTableQueryContext = analyticTableQueryContext;
            _query = _analyticTableQueryContext.Query;
            _dbRecord = dbRecord;
            _allDimensions = allDimensions;
            _isSummaryRecord = isSummaryRecord;
            _subTableIndex = subTableIndex;
            _recordGroupingKey = recordGroupingKey;
            _subTableRecordGroupingKey = subTableRecordGroupingKey;
        }

        public dynamic GetAggregateValue(string aggregateName)
        {
            DBAnalyticRecordAggValue aggValue;
            if (!_dbRecord.AggValuesByAggName.TryGetValue(aggregateName, out aggValue))
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
            if (!_dbRecord.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
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
            return _analyticTableQueryContext.FromTime;
        }

        public DateTime GetQueryToTime()
        {
            return _analyticTableQueryContext.ToTime;
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
                    DBRecord = _dbRecord,
                    IsSummaryRecord = _isSummaryRecord,
                    MeasureName = measureName,
                    SubTableIndex = _subTableIndex,
                    RecordGroupingKey = _recordGroupingKey,
                    SubTableRecordGroupingKey = _subTableRecordGroupingKey
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

            public DBAnalyticRecord DBRecord { get; set; }

            public bool IsSummaryRecord { get; set; }

            public string MeasureName { get; set; }

            public int? SubTableIndex { get; set; }


            public string RecordGroupingKey
            {
                get;
                set;
            }

            public string SubTableRecordGroupingKey
            {
                get;
                set;
            }
        }

        #endregion
    }
}