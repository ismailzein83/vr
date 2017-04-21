using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasureExternalSource
    {
        public Guid AnalyticMeasureExternalSourceConfigId { get; set; }
        public string Name { get; set; }
        public AnalyticMeasureExternalSourceConfig Config { get; set; }
    }
}
