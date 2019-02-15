using System;
using System.Collections.Generic;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRateDraftToUpdate
    {
        public long? ProcessDraftID { get; set; }

        public int SupplierID { get; set; }

        public int CurrencyId { get; set; }

        public List<SupplierSMSRateChangeToUpdate> SMSRates { get; set; }

        public DateTime EffectiveDate { get; set; }
    }

    public class SupplierSMSRateChangeToUpdate
    {
        public int MobileNetworkID { get; set; }

        public decimal? NewRate { get; set; }
    }
}
