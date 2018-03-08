using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules
{
    public enum SameDimensionNameType { AllDimensions = 0, SpecificDimensions = 1 }
    public class SameDimensionName : DimensionMappingRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("D8B533F7-3375-4BD3-8D2C-65C137E7BD51"); } }
        public SameDimensionNameType Type { get; set; }
        public List<string> DimensionNames { get; set; }

        public override bool TryMapDimension(IDimensionMappingRuleTryMapDimensionContext context)
        {
            if (this.Type == SameDimensionNameType.AllDimensions || (this.DimensionNames != null && this.DimensionNames.Contains(context.DimensionName)))
            {
                context.MappedDimensionName = context.DimensionName;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
