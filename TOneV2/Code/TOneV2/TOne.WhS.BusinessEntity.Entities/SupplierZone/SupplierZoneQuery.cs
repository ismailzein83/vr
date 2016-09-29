using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneQuery
    {
        public int SupplierId { get; set; }
        public List<int> Countries { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveOn { get; set; }
    }
}
