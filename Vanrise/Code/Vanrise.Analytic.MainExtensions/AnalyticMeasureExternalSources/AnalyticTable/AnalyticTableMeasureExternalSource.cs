using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Analytic.Business;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable
{
    public class AnalyticTableMeasureExternalSource : AnalyticMeasureExternalSourceExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("56B17184-3E6C-4130-8E5A-329BB9720D8E"); } }

        public Guid AnalyticTableId { get; set; }

        public List<DimensionMappingRule> DimensionMappingRules { get; set; }

        public List<MeasureMappingRule> MeasureMappingRules { get; set; }

        public override AnalyticMeasureExternalSourceResult Execute(IAnalyticMeasureExternalSourceContext context)
        {
            this.DimensionMappingRules.ThrowIfNull("this.DimensionMappingRules");
            context.AnalyticQuery.ThrowIfNull("context.AnalyticQuery");

            var originalQuery = context.AnalyticQuery;
            var mappedQuery = originalQuery.VRDeepCopy();
            mappedQuery.TableId = this.AnalyticTableId;

            var dimensionNames = GetAllDimensionNames(mappedQuery);

            Dictionary<string, string> mappedDimensionNames = new Dictionary<string, string>();
            if (dimensionNames != null)
            {
                foreach (var dimName in dimensionNames)
                {
                    bool isMatchRuleFound = false;
                    foreach (var dimensionMappingRule in this.DimensionMappingRules)
                    {
                        dimensionMappingRule.Settings.ThrowIfNull("dimensionMappingRule.Settings");
                        var tryMapDimensionContext = new DimensionMappingRuleTryMapDimensionContext
                        {
                            DimensionName = dimName,
                            AnalyticQuery = mappedQuery
                        };

                        if (dimensionMappingRule.Settings.TryMapDimension(tryMapDimensionContext))
                        {
                            isMatchRuleFound = true;
                            if (tryMapDimensionContext.MappedDimensionName != null)
                                mappedDimensionNames.Add(dimName, tryMapDimensionContext.MappedDimensionName);
                            break;
                        }
                    }
                    if (!isMatchRuleFound)
                        throw new Exception(String.Format("No DimensionMappingRule found for dimension '{0}'", dimName));
                }
            }

            List<string> measures = new List<string>();
            if (originalQuery.MeasureFields != null)
            {
                foreach (var origMeasure in originalQuery.MeasureFields)
                {
                    var tryMapMeasureContext = new MeasureMappingRuleTryMapMeasureContext { MeasureName = origMeasure };

                    foreach (var measureMappingRule in this.MeasureMappingRules)
                    {
                        measureMappingRule.Settings.ThrowIfNull("measureMappingRule.Settings");
                        if (measureMappingRule.Settings.TryMapMeasure(tryMapMeasureContext))
                        {
                            if (tryMapMeasureContext.MappedMeasureNames != null)
                            {
                                measures.AddRange(tryMapMeasureContext.MappedMeasureNames);
                            }
                            break;
                        }
                    }
                }
            }
            mappedQuery.MeasureFields = measures.Distinct().ToList();

            MapQueryDimensionFields(mappedQuery, mappedDimensionNames);
            MapQueryFilters(mappedQuery, mappedDimensionNames);
            MapQueryFilterGroup(mappedQuery, mappedDimensionNames);

            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord summaryRecord;
            var analyticRecords = analyticManager.GetAllFilteredRecords(mappedQuery, out summaryRecord);
            if (analyticRecords != null)
                return new AnalyticTableMeasureExternalSourceResult(analyticRecords, summaryRecord, mappedQuery, mappedDimensionNames);
            else
                return null;
        }

        private void MapQueryDimensionFields(AnalyticQuery mappedQuery, Dictionary<string, string> mappedDimensionNames)
        {
            if (mappedQuery.DimensionFields != null)
            {
                var originalDimensionFields = mappedQuery.DimensionFields;
                mappedQuery.DimensionFields = null;
                foreach (var origDimField in originalDimensionFields)
                {
                    string mappedDimension;
                    if (mappedDimensionNames.TryGetValue(origDimField, out mappedDimension))
                    {
                        if (mappedQuery.DimensionFields == null)
                            mappedQuery.DimensionFields = new List<string>();
                        mappedQuery.DimensionFields.Add(mappedDimension);
                    }
                }
            }
        }

        private void MapQueryFilters(AnalyticQuery mappedQuery, Dictionary<string, string> mappedDimensionNames)
        {
            if (mappedQuery.Filters != null)
            {
                var originalFilters = mappedQuery.Filters;
                mappedQuery.Filters = null;
                foreach (var originalFilter in originalFilters)
                {
                    string mappedDimension;
                    if (mappedDimensionNames.TryGetValue(originalFilter.Dimension, out mappedDimension))
                    {
                        DimensionFilter mappedFilter = new DimensionFilter { Dimension = mappedDimension, FilterValues = originalFilter.FilterValues };
                        if (mappedQuery.Filters == null)
                            mappedQuery.Filters = new List<DimensionFilter>();
                        mappedQuery.Filters.Add(mappedFilter);
                    }
                }
            }
        }

        private void MapQueryFilterGroup(AnalyticQuery mappedQuery, Dictionary<string, string> mappedDimensionNames)
        {
            if (mappedQuery.FilterGroup != null)
            {
                bool filterBecomesEmpty;
                MapFilterGroup(mappedQuery.FilterGroup, mappedDimensionNames, out filterBecomesEmpty);
                if (filterBecomesEmpty)
                    mappedQuery.FilterGroup = null;
            }
        }

        private void MapFilterGroup(GenericData.Entities.RecordFilterGroup filterGroup, Dictionary<string, string> mappedDimensionNames, out bool filterBecomesEmpty)
        {
            if (filterGroup.Filters != null && filterGroup.Filters.Count > 0)
            {
                for (int i = filterGroup.Filters.Count - 1; i >= 0; i--)
                {
                    var filter = filterGroup.Filters[i];

                    if (!String.IsNullOrEmpty(filter.FieldName))
                    {
                        string mappedDimensionName;
                        if (mappedDimensionNames.TryGetValue(filter.FieldName, out mappedDimensionName))
                            filter.FieldName = mappedDimensionName;
                        else
                            filterGroup.Filters.RemoveAt(i);
                    }
                    else
                    {
                        RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                        if (childFilterGroup != null)
                        {
                            bool subFilterBecomesEmpty;
                            MapFilterGroup(childFilterGroup, mappedDimensionNames, out subFilterBecomesEmpty);
                            if (subFilterBecomesEmpty)
                                filterGroup.Filters.RemoveAt(i);
                        }
                    }
                }
                filterBecomesEmpty = filterGroup.Filters.Count == 0;
            }
            else
            {
                filterBecomesEmpty = true;
            }
        }

        private HashSet<string> GetAllDimensionNames(AnalyticQuery query)
        {
            HashSet<string> dimensionNames = new HashSet<string>();
            if (query.DimensionFields != null)
            {
                foreach (var dim in query.DimensionFields)
                {
                    dimensionNames.Add(dim);
                }
            }
            if (query.Filters != null)
            {
                foreach (var filter in query.Filters)
                {
                    dimensionNames.Add(filter.Dimension);
                }
            }
            if (query.FilterGroup != null)
            {
                AddDimensionsNamesFromFilterGroup(dimensionNames, query.FilterGroup);
            }
            return dimensionNames;
        }

        private void AddDimensionsNamesFromFilterGroup(HashSet<string> dimensionNames, RecordFilterGroup filterGroup)
        {
            if (filterGroup.Filters != null)
            {
                foreach (var filter in filterGroup.Filters)
                {
                    if (filter.FieldName != null)
                    {
                        dimensionNames.Add(filter.FieldName);
                    }
                    RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                    if (childFilterGroup != null)
                        AddDimensionsNamesFromFilterGroup(dimensionNames, childFilterGroup);
                }
            }
        }

        #region Private Classes

        private class DimensionMappingRuleTryMapDimensionContext : IDimensionMappingRuleTryMapDimensionContext
        {
            public string DimensionName { get; set; }

            public AnalyticQuery AnalyticQuery { get; set; }

            public string MappedDimensionName { get; set; }
        }

        private class MeasureMappingRuleTryMapMeasureContext : IMeasureMappingRuleTryMapMeasureContext
        {
            public string MeasureName { get; set; }

            public List<string> MappedMeasureNames { get; set; }
        }

        #endregion
    }
}