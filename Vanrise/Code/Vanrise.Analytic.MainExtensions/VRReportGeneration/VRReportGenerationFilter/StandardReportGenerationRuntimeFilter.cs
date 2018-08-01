using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions
{
    public class StandardReportGenerationRuntimeFilter : VRReportGenerationRuntimeFilter
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public override VRReportGenerationFilterContent GetFilterContent(IVRReportGenerationRuntimeFilterContext context)
        {
            return new VRReportGenerationFilterContent
            {
                FromTime = FromTime,
                ToTime = ToTime
            };
        }
    }
}
