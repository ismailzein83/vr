using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimension
    {
        public int AnalyticDimensionConfigId { get; set; }

        public AnalyticDimensionConfig Config { get; set; }
    }
}
