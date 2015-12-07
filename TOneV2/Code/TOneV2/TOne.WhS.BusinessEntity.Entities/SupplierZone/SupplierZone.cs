using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZone
    {
        public long SupplierZoneId { get; set; }
        public int CountryId { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
