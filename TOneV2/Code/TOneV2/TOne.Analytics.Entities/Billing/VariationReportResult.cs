using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VariationReportResult
    {
        public IEnumerable<VariationReportsData> VariationReportsData { get; set; }
        public List<TimeRange> TimeRange { get; set; }
        public int TotalCount { get; set; }
        public List<decimal> TotalValues { get; set; }
        public decimal TotalAverage { get; set; }

        public decimal TotalPercentage { get; set; }
        public decimal TotalPreviousPercentage { get; set; }
  
    }
}
