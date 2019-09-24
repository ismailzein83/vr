using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SMSFutureRate
    {
        public decimal Rate { get; set; }

        public string CurrencySymbol { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
