using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SupplierZoneRate
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal EffectiveRateValue { get; set; }
    }

    public class SupplierZoneRateByZone : Dictionary<long, SupplierZoneRate>
    {

    }
}
