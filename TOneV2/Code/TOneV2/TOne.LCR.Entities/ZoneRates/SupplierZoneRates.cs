using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class  SupplierZoneRates
    {
        //public Dictionary<string, ZoneRates> SuppliersZonesRates { get; set; }

        public Dictionary<int, RateInfo> RatesByZoneId { get; set; }

        //public RateInfo[] OrderedRates { get; set; }
    }
}
