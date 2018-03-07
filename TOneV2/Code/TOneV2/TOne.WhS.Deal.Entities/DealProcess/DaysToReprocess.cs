using System;

namespace TOne.WhS.Deal.Entities
{
    public class DaysToReprocess
    {
        public long DaysToReprocessId { get; set; }
        public DateTime Date { get; set; }
        public bool IsSale { get; set; }
        public int CarrierAccountId { get; set; }
    }
}