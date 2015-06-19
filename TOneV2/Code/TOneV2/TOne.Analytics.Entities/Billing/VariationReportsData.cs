using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VariationReportsData
    {
        public string Name { get; set; }

        public decimal PeriodTypeValueAverage { get; set; }

        public decimal PeriodTypeValuePercentage { get; set; }

        public decimal PreviousPeriodTypeValuePercentage { get; set; }

        public List<decimal> TotalDurationsPerDate { get; set; }
        public string ID { get; set; }

    }
}
