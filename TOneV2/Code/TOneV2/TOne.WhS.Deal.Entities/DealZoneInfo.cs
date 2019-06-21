using System;

namespace TOne.WhS.Deal.Entities
{
    public class DealZoneInfo
    {
        public long ZoneId { get; set; }
        public int DealId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public DealStatus DealStatus { get; set; }

    }
}
