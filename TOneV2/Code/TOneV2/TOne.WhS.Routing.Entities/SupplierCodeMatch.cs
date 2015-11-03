using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SupplierCodeMatch
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public string SupplierCode { get; set; }
    }

    public class SupplierCodeMatchBySupplier : Dictionary<int, List<SupplierCodeMatch>>
    {
    }
}
