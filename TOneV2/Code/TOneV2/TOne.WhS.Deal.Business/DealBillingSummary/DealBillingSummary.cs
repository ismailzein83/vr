using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Business
{
    public class BaseDealBillingSummaryRecord
    {
        public DateTime BatchStart { get; set; }

        public int DealId { get; set; }

        public int DealZoneGroupNb { get; set; }

        public decimal DurationInSeconds { get; set; }
    }

    public class DealBillingSummaryRecord : BaseDealBillingSummaryRecord
    {
        public int DealTierNb { get; set; }

        public decimal DealRateTierNb { get; set; }
    }
}
