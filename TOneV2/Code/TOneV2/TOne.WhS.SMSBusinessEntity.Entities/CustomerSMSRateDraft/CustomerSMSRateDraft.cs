using System;
using System.Collections.Generic;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateDraft
    {
        public int CustomerID { get; set; }

        public Dictionary<int,CustomerSMSRateChange> SMSRates { get; set; }

        public DateTime EffectiveDate { get; set; }

        public ProcessStatus Status { get; set; }
    }

    public class CustomerSMSRateChange
    {
        public int MobileNetworkID { get; set; }

        public decimal NewRate { get; set; }
    }

    
}
