using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneRate
    {
        public SupplierRate Rate { get; set; }

        public Dictionary<int, SupplierRate> RatesByRateType { get; set; }
    }
}
