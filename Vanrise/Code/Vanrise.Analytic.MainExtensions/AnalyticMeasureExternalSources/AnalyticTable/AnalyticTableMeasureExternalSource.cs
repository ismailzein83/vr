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
        public override Guid ConfigId
        {
            get { return new Guid("56B17184-3E6C-4130-8E5A-329BB9720D8E"); }
        }

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
                            if (tryMapDimensionContext.MappedDimensionName != null && originalQuery.DimensionFields != null && originalQuery.DimensionFields.Contains(dimName))
                                mappedDimensionNames.Add(dimName, tryMapDimensionContext.MappedDimensionName);
                            break;
                        }
                    }
                    if(!isMatchRuleFound)
                        throw new Exception(String.Format("No DimensionMappingRule found for dimension '{0}'", dimName));
                }
            }
            List<string> measures = new List<string>();
            if(originalQuery.MeasureFields != null)
            {
                foreach(var origMeasure in originalQuery.MeasureFields)
                {
                    var tryMapMeasureContext = new MeasureMappingRuleTryMapMeasureContext
                    {
                        MeasureName = origMeasure,
                    };
                    foreach(var measureMappingRule in this.MeasureMappingRules)
                    {
                        measureMappingRule.Settings.ThrowIfNull("measureMappingRule.Settings");
                        if(measureMappingRule.Settings.TryMapMeasure(tryMapMeasureContext))
                        {
                            if(tryMapMeasureContext.MappedMeasureNames != null )
                            {
                                measures.AddRange(tryMapMeasureContext.MappedMeasureNames);
                            }
                            break;
                        }
                    }
                }
            }
            mappedQuery.MeasureFields = measures.Distinct().ToList();
            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord summaryRecord;
            var analyticRecords = analyticManager.GetAllFilteredRecords(mappedQuery, out summaryRecord);
            if (analyticRecords != null)
                return new AnalyticTableMeasureExternalSourceResult(analyticRecords, summaryRecord, mappedQuery, mappedDimensionNames);
            else
                return null;
        }

        HashSet<string> GetAllDimensionNames(AnalyticQuery query)
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

        void AddDimensionsNamesFromFilterGroup(HashSet<string> dimensionNames, RecordFilterGroup filterGroup)
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
            public string DimensionName
            {
                get;
                set;
            }

            public AnalyticQuery AnalyticQuery
            {
                get;
                set;
            }


            public string MappedDimensionName
            {
                get;
                set;
            }
        }

        private class MeasureMappingRuleTryMapMeasureContext : IMeasureMappingRuleTryMapMeasureContext
        {
            public string MeasureName
            {
                get;
                set;
            }

            public List<string> MappedMeasureNames
            {
                get;
                set;
            }
        }


        #endregion
    }
}
