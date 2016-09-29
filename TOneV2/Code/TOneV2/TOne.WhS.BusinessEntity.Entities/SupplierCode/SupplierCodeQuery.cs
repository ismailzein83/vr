using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierCodeQuery
    {
        public string Code { get; set; }

        public int SupplierId { get; set; }

        public List<int> ZoneIds { get; set; }

        public DateTime EffectiveOn { get; set; }
    }
}
