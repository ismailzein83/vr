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
        public override Guid ConfigId { get { return new Guid("1132614A-DF0C-4ADC-9CE0-6064194495F6"); } }
        public List<string> ExcludedDimensions { get; set; }

        public override bool TryMapDimension(IDimensionMappingRuleTryMapDimensionContext context)
        {
            string dimName = context.DimensionName;
            if (this.ExcludedDimensions.Contains(dimName))
                return true;
            else
                return false;
        }
    }
}
