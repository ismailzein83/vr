using System;
using System.Collections.Generic;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SupplierSMSRateQuery
    {
        public int SupplierID { get; set; }

        public DateTime EffectiveDate { get; set; }

        public List<int> MobileCountryIds { get; set; }

        public List<int> MobileNetworkIds{ get; set; }
    }
}