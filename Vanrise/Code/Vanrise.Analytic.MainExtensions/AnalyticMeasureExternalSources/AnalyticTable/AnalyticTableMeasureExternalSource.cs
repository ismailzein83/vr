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
            var mappedQuery = new AnalyticQuery
            {
                TableId = this.AnalyticTableId,
                FromTime = originalQuery.FromTime,
                ToTime = originalQuery.ToTime,
                CurrencyId = originalQuery.CurrencyId,
                LastHours = originalQuery.LastHours,
                TimeGroupingUnit = originalQuery.TimeGroupingUnit,
                WithSummary = originalQuery.WithSummary
            };


            var dimensionNames = GetAllDimensionNames(originalQuery);

            Dictionary<string, string> dimensionMappings = new Dictionary<string, string>();
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
                            DimensionName = dimName//,
                            //AnalyticQuery = mappedQuery
                        };

                        if (dimensionMappingRule.Settings.TryMapDimension(tryMapDimensionContext))
                        {
                            isMatchRuleFound = true;
                            if (tryMapDimensionContext.MappedDimensionName != null)
                                dimensionMappings.Add(dimName, tryMapDimensionContext.MappedDimensionName);
                            break;
                        }
                    }
                    if (!isMatchRuleFound)
                        throw new Exception(String.Format("No DimensionMappingRule found for dimension '{0}'", dimName));
                }
            }
            
            mappedQuery.MeasureFields = MapMeasures(originalQuery.MeasureFields);

            mappedQuery.DimensionFields = MapDimensionFields(originalQuery.DimensionFields, dimensionMappings);
            mappedQuery.ParentDimensions = MapDimensionFields(originalQuery.ParentDimensions, dimensionMappings);
            mappedQuery.Filters = MapDimensionFilters(originalQuery.Filters, dimensionMappings);
            mappedQuery.FilterGroup = MapFilterGroup(originalQuery.FilterGroup, dimensionMappings);

            Dictionary<int, int> subTablesIndexMapping;
            mappedQuery.SubTables = MapSubTables(originalQuery.SubTables, dimensionMappings, out subTablesIndexMapping);

            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord summaryRecord;
            List<AnalyticResultSubTable> resultSubTables;
            var analyticRecords = analyticManager.GetAllFilteredRecords(mappedQuery, out summaryRecord, out resultSubTables);
            if (analyticRecords != null)
                return new AnalyticTableMeasureExternalSourceResult(analyticRecords, summaryRecord, mappedQuery, dimensionMappings, subTablesIndexMapping, resultSubTables);
            else
                return null;
        }

        private List<AnalyticQuerySubTable> MapSubTables(List<AnalyticQuerySubTable> originalSubTables, Dictionary<string, string> mappedDimensionNames, out Dictionary<int, int> subTablesIndexMapping)
        {
            List<AnalyticQuerySubTable> subTables = null;
            subTablesIndexMapping = new Dictionary<int, int>();
            if(originalSubTables != null)
            {
                for(int originalSubTableIndex = 0;originalSubTableIndex < originalSubTables.Count;originalSubTableIndex++)
                {
                    var originalSubTable = originalSubTables[originalSubTableIndex];
                    var mappedMeasures = MapMeasures(originalSubTable.Measures);
                    if(mappedMeasures != null && mappedMeasures.Count > 0)
                    {
                        if (subTables == null)
                            subTables = new List<AnalyticQuerySubTable>();
                        var subTable = new AnalyticQuerySubTable
                        {
                            Dimensions = MapDimensionFields(originalSubTable.Dimensions, mappedDimensionNames),
                            Measures = mappedMeasures
                        };
                        subTables.Add(subTable);
                        subTablesIndexMapping.Add(originalSubTableIndex, subTables.Count - 1);
                    }                    
                }
            }
            return subTables;
        }

        private List<string> MapMeasures(List<string> originalMeasures)
        {
            List<string> measures = new List<string>();
            if (originalMeasures != null)
            {
                foreach (var origMeasure in originalMeasures)
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
            return measures.Distinct().ToList();
        }

        private List<string> MapDimensionFields(List<string> originalDimensionFields, Dictionary<string, string> mappedDimensionNames)
        {
            List<string> mappedDimensions = null;
            if (originalDimensionFields != null)
            {
                foreach (var origDimField in originalDimensionFields)
                {
                    string mappedDimension;
                    if (mappedDimensionNames.TryGetValue(origDimField, out mappedDimension))
                    {
                        if (mappedDimensions == null)
                            mappedDimensions = new List<string>();
                        mappedDimensions.Add(mappedDimension);
                    }
                }
            }
            return mappedDimensions;
        }

        private List<DimensionFilter> MapDimensionFilters(List<DimensionFilter> originalFilters, Dictionary<string, string> mappedDimensionNames)
        {
            List<DimensionFilter> mappedFilters = null;  
            if (originalFilters != null)
            {
                foreach (var originalFilter in originalFilters)
                {
                    string mappedDimension;
                    if (mappedDimensionNames.TryGetValue(originalFilter.Dimension, out mappedDimension))
                    {
                        DimensionFilter mappedFilter = new DimensionFilter { Dimension = mappedDimension, FilterValues = originalFilter.FilterValues };
                        if (mappedFilters == null)
                            mappedFilters = new List<DimensionFilter>();
                        mappedFilters.Add(mappedFilter);
                    }
                }
            }
            return mappedFilters;
        }

        private RecordFilterGroup MapFilterGroup(RecordFilterGroup originalFilterGroup, Dictionary<string, string> mappedDimensionNames)
        {
            RecordFilterGroup mappedFilterGroup = null;
            if (originalFilterGroup != null)
            {
                var filterGroupCopy = originalFilterGroup.VRDeepCopy();
                MapFieldsInFilterGroup(filterGroupCopy, mappedDimensionNames);
                if (filterGroupCopy.Filters != null && filterGroupCopy.Filters.Count > 0)
                    mappedFilterGroup = filterGroupCopy;
            }
            return mappedFilterGroup;
        }

        private void MapFieldsInFilterGroup(RecordFilterGroup filterGroup, Dictionary<string, string> mappedDimensionNames)
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
                            MapFieldsInFilterGroup(childFilterGroup, mappedDimensionNames);
                            if (childFilterGroup.Filters == null || childFilterGroup.Filters.Count == 0)
                                filterGroup.Filters.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private HashSet<string> GetAllDimensionNames(AnalyticQuery query)
        {
            List<string> dimensionNames = new List<string>();
            if (query.DimensionFields != null)
                dimensionNames.AddRange(query.DimensionFields);
            if(query.ParentDimensions != null)
                dimensionNames.AddRange(query.ParentDimensions); 
            if (query.Filters != null)
                dimensionNames.AddRange(query.Filters.Select(itm => itm.Dimension));
            if (query.FilterGroup != null)
                AddDimensionsNamesFromFilterGroup(dimensionNames, query.FilterGroup);
            if(query.SubTables != null)
            {
                foreach(var subTable in query.SubTables)
                {
                    if(subTable.Dimensions != null)
                        dimensionNames.AddRange(subTable.Dimensions);
                }
            }
            return dimensionNames.ToHashSet();
        }

        private void AddDimensionsNamesFromFilterGroup(List<string> dimensionNames, RecordFilterGroup filterGroup)
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

            //public AnalyticQuery AnalyticQuery { get; set; }

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