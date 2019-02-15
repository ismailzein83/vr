using System;
using System.Collections.Generic;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRateDraft
    {
        public int SupplierID { get; set; }

        public int CurrencyId { get; set; }

        public Dictionary<int, SupplierSMSRateChange> SMSRates { get; set; }

        public DateTime EffectiveDate { get; set; }

        public ProcessStatus Status { get; set; }
    }

    public class SupplierSMSRateChange
    {
        public int MobileNetworkID { get; set; }

        public decimal NewRate { get; set; }
    }
}