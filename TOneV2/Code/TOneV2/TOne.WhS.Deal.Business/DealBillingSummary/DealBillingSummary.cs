using System;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealBillingSummary
    {
        public int DealId { get; set; }
        public int ZoneGroupNb { get; set; }
        public bool IsSale { get; set; }
        public DateTime BatchStart { get; set; }
        public int? TierNb { get; set; }
        public int? RateTierNb { get; set; }
        public decimal DurationInSeconds { get; set; }

        public bool IsEqual(DealBillingSummary dealBillingSummary)
        {
            if (dealBillingSummary.BatchStart != BatchStart)
                return false;

            if (dealBillingSummary.DealId != DealId)
                return false;

            if (dealBillingSummary.ZoneGroupNb != ZoneGroupNb)
                return false;

            if (dealBillingSummary.DurationInSeconds != DurationInSeconds)
                return false;

            if (dealBillingSummary.IsSale != IsSale)
                return false;

            if (dealBillingSummary.TierNb != TierNb)
                return false;

            if (dealBillingSummary.RateTierNb != RateTierNb)
                return false;

            return true;
        }
    }
}