using System;

namespace TOne.WhS.Deal.Entities
{
    public class DayToReprocess
    {
        public long DayToReprocessId { get; set; }
        public DateTime Date { get; set; }
        public bool IsSale { get; set; }
        public int CarrierAccountId { get; set; }
    }
}