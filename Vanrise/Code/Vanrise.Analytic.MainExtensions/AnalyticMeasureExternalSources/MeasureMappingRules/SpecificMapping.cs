using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules
{
    public class SpecificMapping : MeasureMappingRuleSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("017EAC3C-BDE1-451A-A3C6-3FBF808115E6"); }
        }

        public string MeasureName { get; set; }

        public List<string> MappedMeasures { get; set; }

        public override bool TryMapMeasure(IMeasureMappingRuleTryMapMeasureContext context)
        {
            if (context.MeasureName == this.MeasureName)
            {
                context.MappedMeasureNames = this.MappedMeasures;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
