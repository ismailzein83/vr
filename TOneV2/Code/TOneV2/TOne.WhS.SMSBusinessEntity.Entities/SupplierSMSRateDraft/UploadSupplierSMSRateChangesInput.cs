using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class UploadSupplierSMSRateChangesInput
    {
        public long FileId { get; set; }

        public int SupplierID { get; set; }

        public int CurrencyId { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
