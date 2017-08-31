using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules
{
    public class SameDimensionName : DimensionMappingRuleSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("D8B533F7-3375-4BD3-8D2C-65C137E7BD51"); }
        }

        public override bool TryMapDimension(IDimensionMappingRuleTryMapDimensionContext context)
        {
            context.MappedDimensionName = context.DimensionName;
            return true;
        }
    }
}
