using System;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRateDetail
    {
        public long ID { get; set; }

        public string MobileCountryName { get; set; }

        public int MobileNetworkID { get; set; }

        public string MobileNetworkName { get; set; }

        public decimal Rate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public SMSFutureRate FutureRate { get; set; }
    }
}
