using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson
{
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public CustomerMapping CustomerMapping { get; set; }

        public SupplierMapping SupplierMapping { get; set; }
    }

    public class CustomerMapping
    {
        public string BO { get; set; }

        public string NationalOBA { get; set; }

        public string InternationalOBA { get; set; }

        public List<InTrunk> Trunks { get; set; }
    }

    public class SupplierMapping
    {
        public List<OutTrunk> Trunks { get; set; }

        public List<TrunkGroup> TrunkGroups { get; set; }
    }
}
