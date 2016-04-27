using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasureConfigDetail
    {
        public int AnalyticItemConfigId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public AnalyticMeasureConfig Entity { get; set; }
    }
}
