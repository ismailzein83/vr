using System;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRate : IDateEffectiveSettings, IBusinessEntityInfo
    {
        public long ID { get; set; }

        public int PriceListID { get; set; }

        public int MobileNetworkID { get; set; }

        public decimal Rate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}