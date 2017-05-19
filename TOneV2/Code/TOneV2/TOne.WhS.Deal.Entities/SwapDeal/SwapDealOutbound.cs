using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealOutbound
    {
        public string Name { get; set; }

        public int CountryId { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public int Volume { get; set; }

        public Decimal Rate { get; set; }
    }
}
