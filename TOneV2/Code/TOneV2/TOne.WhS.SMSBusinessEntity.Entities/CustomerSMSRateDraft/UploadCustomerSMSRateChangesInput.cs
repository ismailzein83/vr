using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class UploadCustomerSMSRateChangesInput
    {
        public long FileId { get; set; }

        public int CustomerID { get; set; }

        public int CurrencyId { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
