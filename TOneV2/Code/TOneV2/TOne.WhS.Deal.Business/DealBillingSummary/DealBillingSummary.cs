using System;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class BaseDealBillingSummary
    {
        public DateTime BatchStart { get; set; }

        public int DealId { get; set; }

        public int DealZoneGroupNb { get; set; }

        public decimal DurationInSeconds { get; set; }

        public bool IsSale { get; set; }
    }

    public class DealBillingSummary : BaseDealBillingSummary
    {
        public int DealTierNb { get; set; }

        public int DealRateTierNb { get; set; }

        public bool IsEqual(DealBillingSummary dealBillingSummary)
        {
            if (dealBillingSummary.BatchStart != BatchStart)
                return false;

            if (dealBillingSummary.DealId != DealId)
                return false;

            if (dealBillingSummary.DealZoneGroupNb != DealZoneGroupNb)
                return false;

            if (dealBillingSummary.DurationInSeconds != DurationInSeconds)
                return false;

            if (dealBillingSummary.IsSale != IsSale)
                return false;

            if (dealBillingSummary.DealTierNb != DealTierNb)
                return false;

            if (dealBillingSummary.DealRateTierNb != DealRateTierNb)
                return false;

            return true;
        }
    }
}
