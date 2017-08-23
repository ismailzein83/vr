using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class InheritedRatesByZoneId : Dictionary<long, ZoneInheritedRates>
    {

    }
    public class ZoneInheritedRates
    {
        public List<SaleRate> NormalRates { get; set; }
        public Dictionary<int, List<SaleRate>> OtherRatesByRateTypeId { get; set; }
        public ZoneInheritedRates()
        {
            NormalRates = new List<SaleRate>();
            OtherRatesByRateTypeId = new Dictionary<int, List<SaleRate>>();
        }
    }
}
