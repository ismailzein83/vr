using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateDetail
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
