using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Vanrise.Analytic.Business
{
    public class AnalyticTableQueryContext : IAnalyticTableQueryContext
    {
        #region Properties\Ctor

        AnalyticQuery _query;
        DateTime _queryToTime;
        AnalyticTable _table;
        Dictionary<string, AnalyticMeasureExternalSourceResult> _measureExternalSourceResults;
        List<string> _dimensionNamesFromQueryFilters;

        public AnalyticQuery Query { get { return _query; } }

        public DateTime FromTime { get { return _query.FromTime; } }

        public DateTime ToTime { get { return _queryToTime; } }

        public Dictionary<string, dynamic> QueryParameters { get { return _query.QueryParameters; } }

        public TimeGroupingUnit? TimeGroupingUnit { get { return _query.TimeGroupingUnit; } }

        public int? TopRecords { get { return _query.TopRecords; } }

        Dictionary<string, AnalyticDimension> _dimensions;
        public Dictionary<string, AnalyticDimension> Dimensions { get { return _dimensions; } }

        Dictionary<string, AnalyticAggregate> _aggregates;
        public Dictionary<string, AnalyticAggregate> Aggregates { get { return _aggregates; } }

        Dictionary<string, AnalyticMeasure> _measures;
        public Dictionary<string, AnalyticMeasure> Measures { get { return _measures; } }

        Dictionary<string, AnalyticJoin> _joins;
        public Dictionary<string, AnalyticJoin> Joins { get { return _joins; } }

        Dictionary<string, AnalyticMeasureExternalSource> _measureExternalSources;
        public Dictionary<string, AnalyticMeasureExternalSource> MeasureExternalSources { get { return _measureExternalSources; } }

        public AnalyticTableQueryContext(AnalyticQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            _query = query;
            _queryToTime = _query.ToTime.HasValue ? _query.ToTime.Value : AnalyticManager.GenerateQueryToTime(query.FromTime);

            var analyticTableId = _query.TableId;
            _table = new AnalyticTableManager().GetAnalyticTableById(analyticTableId);
            if (_table == null)
                throw new NullReferenceException(String.Format("table. ID '{0}'", analyticTableId));
            if (_table.Settings == null)
                throw new NullReferenceException(String.Format("table.Settings. ID '{0}'", analyticTableId));

            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            _dimensions = analyticItemConfigManager.GetDimensions(analyticTableId);
            _aggregates = analyticItemConfigManager.GetAggregates(analyticTableId);
            _measures = analyticItemConfigManager.GetMeasures(analyticTableId);
            _joins = analyticItemConfigManager.GetJoins(analyticTableId);
            _measureExternalSources = analyticItemConfigManager.GetMeasureExternalSources(analyticTableId);

            FillDimensionNamesFromQueryFilters();
        }

        #endregion

        #region Public Methods

        public AnalyticTable GetTable()
        {
            return _table;
        }

        public AnalyticDimension GetDimensionConfig(string dimensionName)
        {
            if (_dimensions == null)
                throw new NullReferenceException("_dimensions");

            AnalyticDimension dimension;
            if (!_dimensions.TryGetValue(dimensionName, out dimension))
                throw new NullReferenceException(String.Format("dimension '{0}'", dimensionName));

            return dimension;
        }

        public AnalyticAggregate GetAggregateConfig(string aggregateName)
        {
            if (_aggregates == null)
                throw new NullReferenceException("_aggregates");

            AnalyticAggregate aggregate;
            if (!_aggregates.TryGetValue(aggregateName, out aggregate))
                throw new NullReferenceException(String.Format("aggregate '{0}'", aggregateName));

            return aggregate;
        }

        public AnalyticMeasure GetMeasureConfig(string measureName)
        {
            if (_measures == null)
                throw new NullReferenceException("_measures");

            AnalyticMeasure measure;
            if (!_measures.TryGetValue(measureName, out measure))
                throw new NullReferenceException(String.Format("measure '{0}'", measureName));

            return measure;
        }

        public AnalyticJoin GetJoinContig(string joinName)
        {
            if (_joins == null)
                throw new NullReferenceException("_joins");

            AnalyticJoin join;
            if (!_joins.TryGetValue(joinName, out join))
                throw new NullReferenceException(String.Format("join '{0}'", joinName));

            return join;
        }

        public AnalyticMeasureExternalSource GetMeasureExternalSourceConfig(string measureExternalSourceName)
        {
            if (_measureExternalSources == null)
                throw new NullReferenceException("_measureExternalSources");

            AnalyticMeasureExternalSource measureExternalSource;
            if (!_measureExternalSources.TryGetValue(measureExternalSourceName, out measureExternalSource))
                throw new NullReferenceException(String.Format("measureExternalSource '{0}'", measureExternalSourceName));

            return measureExternalSource;
        }

        public List<string> GetDimensionNamesFromQueryFilters()
        {
            return _dimensionNamesFromQueryFilters;
        }

        public AnalyticMeasureExternalSourceResult GetMeasureExternalSourceResult(string sourceName)
        {
            if (_measureExternalSourceResults == null)
                _measureExternalSourceResults = new Dictionary<string, AnalyticMeasureExternalSourceResult>();

            AnalyticMeasureExternalSourceResult result;
            if (!_measureExternalSourceResults.TryGetValue(sourceName, out result))
            {
                var externalSource = GetMeasureExternalSourceConfig(sourceName);

                var analyticQuery = _query.VRDeepCopy();
                analyticQuery.FilterGroup = new RecordFilterManager().ReBuildRecordFilterGroupWithExcludedFields(analyticQuery.FilterGroup, _measures.MapRecords(x => x.Key).ToList());

                var measureExternalSourceContext = new AnalyticMeasureExternalSourceContext
                {
                    AnalyticQuery = analyticQuery
                };
                result = externalSource.Config.ExtendedSettings.Execute(measureExternalSourceContext);
                _measureExternalSourceResults.Add(sourceName, result);
            }

            return result;
        }

        public T GetOrCreateCachedObjectBasedOnItemConfig<T>(object cacheName, Func<T> createObject)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AnalyticItemConfigManager.CacheManager>().GetOrCreateObject(cacheName, createObject);
        }

        #endregion

        #region Private Methods

        private void FillDimensionNamesFromQueryFilters()
        {
            List<string> dimensionNames = new List<string>();

            if (_query.Filters != null && _query.Filters.Count > 0)
                dimensionNames.AddRange(_query.Filters.Select(itm => itm.Dimension));

            if (_query.FilterGroup != null)
                FillDimensionNamesFromFilterGroup(dimensionNames, _query.FilterGroup);

            _dimensionNamesFromQueryFilters = dimensionNames.Distinct().ToList();
        }

        private void FillDimensionNamesFromFilterGroup(List<string> dimensionNames, RecordFilterGroup filterGroup)
        {
            foreach (var filter in filterGroup.Filters)
            {
                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                {
                    FillDimensionNamesFromFilterGroup(dimensionNames, childFilterGroup);
                }
                else
                {
                    if (_dimensions.Any(dimension => dimension.Value.Name == filter.FieldName))
                        dimensionNames.Add(filter.FieldName);
                }
            }
        }

        #endregion

        #region Private Classes

        private class AnalyticMeasureExternalSourceContext : IAnalyticMeasureExternalSourceContext
        {
            public AnalyticQuery AnalyticQuery { get; set; }
        }

        #endregion
    }
}