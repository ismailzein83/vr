using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class SupplierRouteDetail : SupplierRoute
    {
        public int SupplierZoneId { get; set; }

        public decimal Rate { get; set; }

        public int ServicesFlag { get; set; }

        public int Priority { get; set; }

    }
}
