using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Business
{
    public class BaseDealBillingSummary
    {
        public DateTime BatchStart { get; set; }

        public int DealId { get; set; }

        public int DealZoneGroupNb { get; set; }

        public decimal DurationInSeconds { get; set; }
    }

    public class DealBillingSummary : BaseDealBillingSummary
    {
        public int DealTierNb { get; set; }

        public decimal DealRateTierNb { get; set; }
    }
}
