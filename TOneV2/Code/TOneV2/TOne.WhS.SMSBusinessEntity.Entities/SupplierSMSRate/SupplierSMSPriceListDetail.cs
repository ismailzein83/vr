using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSPriceListDetail
    {
        public long ID { get; set; }

        public string SupplierName { get; set; }

        public DateTime EffectiveOn { get; set; }

        public string CurrencyName { get; set; }

        public string UserName { get; set; }
    }
}