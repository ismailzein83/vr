using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable
{
    public class MeasureMappingRule
    {
        public string Name { get; set; }
        public MeasureMappingRuleSettings Settings { get; set; }
    }

    public abstract class MeasureMappingRuleSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool TryMapMeasure(IMeasureMappingRuleTryMapMeasureContext context);
    }

    public interface IMeasureMappingRuleTryMapMeasureContext
    {
        string MeasureName { get; }

        List<string> MappedMeasureNames { set; }
    }
}
