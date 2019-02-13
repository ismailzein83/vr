using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSPriceList
    {
        public long ID { get; set; }

        public int CustomerID { get; set; }

        public int CurrencyID { get; set; }

        public DateTime EffectiveOn { get; set; }

        public long ProcessInstanceID { get; set; }

        public int UserID { get; set; }
    }
}