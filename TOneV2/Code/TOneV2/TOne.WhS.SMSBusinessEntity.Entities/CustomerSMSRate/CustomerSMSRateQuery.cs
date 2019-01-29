using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateQuery
    {
        public int CustomerID { get; set; }

        public string MobileCountryName { get; set; }

        public string MobileNetworkName { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
