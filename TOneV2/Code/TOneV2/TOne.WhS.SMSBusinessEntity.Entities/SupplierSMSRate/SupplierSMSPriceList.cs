using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSPriceList
    {
        public long ID { get; set; }

        public int SupplierID { get; set; }

        public int CurrencyID { get; set; }

        public DateTime EffectiveOn { get; set; }

        public long ProcessInstanceID { get; set; }

        public int UserID { get; set; }
    }
}