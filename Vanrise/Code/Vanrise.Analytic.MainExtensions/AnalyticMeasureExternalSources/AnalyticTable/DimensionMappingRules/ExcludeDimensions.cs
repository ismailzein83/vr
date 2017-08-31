using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules
{
    public class ExcludeDimensions : DimensionMappingRuleSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1132614A-DF0C-4ADC-9CE0-6064194495F6"); }
        }


        public List<string> ExcludedDimensions { get; set; }

        public override bool TryMapDimension(IDimensionMappingRuleTryMapDimensionContext context)
        {
            string dimName = context.DimensionName;
            if (this.ExcludedDimensions.Contains(dimName))
            {
                var query = context.AnalyticQuery;
                if (query.DimensionFields != null && query.DimensionFields.Contains(dimName))
                    query.DimensionFields.Remove(dimName);
                if (query.Filters != null)
                    query.Filters.RemoveAll(itm => itm.Dimension == dimName);
                if (query.FilterGroup != null)
                {
                    bool filterBecomesEmpty;
                    ExcludeFilterFromGroup(query.FilterGroup, dimName, out filterBecomesEmpty);
                    if (filterBecomesEmpty)
                        query.FilterGroup = null;
                }
                return true;
            }
            else
                return false;
        }

        private void ExcludeFilterFromGroup(GenericData.Entities.RecordFilterGroup filterGroup, string dimName, out bool filterBecomesEmpty)
        {
            if (filterGroup.Filters != null)
            {
                int originalFilterCount = filterGroup.Filters.Count;
                for (int i = filterGroup.Filters.Count - 1; i >= 0; i--)
                {
                    var filter = filterGroup.Filters[i];

                    if (filter.FieldName == dimName)
                    {
                        filterGroup.Filters.RemoveAt(i);
                    }
                    else
                    {
                        RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                        if (childFilterGroup != null)
                        {
                            bool subFilterBecomesEmpty;
                            ExcludeFilterFromGroup(childFilterGroup, dimName, out subFilterBecomesEmpty);
                            if (subFilterBecomesEmpty)
                                filterGroup.Filters.RemoveAt(i);
                        }
                    }
                }
                if (filterGroup.Filters.Count != originalFilterCount)
                    filterBecomesEmpty = filterGroup.Filters.Count == 0 || filterGroup.LogicalOperator == RecordQueryLogicalOperator.Or;
            }
            filterBecomesEmpty = false;
        }
    }
}
