using System;

namespace TOne.WhS.Deal.Entities
{
    public class DealZoneRate
    {
        public long DealZoneRateId { get; set; }
        public int DealId { get; set; }
        public int ZoneGroupNb { get; set; }
        public bool IsSale { get; set; }
        public int TierNb { get; set; }
        public long ZoneId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}