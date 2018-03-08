using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules
{
    public class SpecificDimensionMapping : DimensionMappingRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("1E93563D-CCA6-48F4-BD34-A21B26A3B773"); } }
        public string DimensionName { get; set; }
        public string MappedDimensionName { get; set; }

        public override bool TryMapDimension(IDimensionMappingRuleTryMapDimensionContext context)
        {
            if (context.DimensionName == this.DimensionName)
            {
                context.MappedDimensionName = this.MappedDimensionName;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}