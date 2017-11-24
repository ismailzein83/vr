using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerZoneRates
    {
        public CustomerZoneRates()
        {
            SellingProductZoneRatesByType = new Dictionary<RateTypeKey, List<SaleRate>>();
            CustomerZoneRatesByType = new Dictionary<RateTypeKey, List<SaleRate>>();
        }

        public Dictionary<RateTypeKey, List<SaleRate>> SellingProductZoneRatesByType { get; set; }

        public Dictionary<RateTypeKey, List<SaleRate>> CustomerZoneRatesByType { get; set; }
    }
}
