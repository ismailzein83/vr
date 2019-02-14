using System;
using System.Collections.Generic;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateQuery
    {
        public int CustomerID { get; set; }

        public DateTime EffectiveDate { get; set; }

        public List<int> MobileCountryIds { get; set; }

        public List<int> MobileNetworkIds{ get; set; }
    }
}
