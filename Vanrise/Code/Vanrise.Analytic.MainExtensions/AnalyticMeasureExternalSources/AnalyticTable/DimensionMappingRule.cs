using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable
{
    public class DimensionMappingRule
    {
        public string Name { get; set; }

        public DimensionMappingRuleSettings Settings { get; set; }
    }

    public abstract class DimensionMappingRuleSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool TryMapDimension(IDimensionMappingRuleTryMapDimensionContext context);
    }

    public interface IDimensionMappingRuleTryMapDimensionContext
    {
        string DimensionName { get; }

        AnalyticQuery AnalyticQuery { get; }

        string MappedDimensionName { set; }
    }
}
