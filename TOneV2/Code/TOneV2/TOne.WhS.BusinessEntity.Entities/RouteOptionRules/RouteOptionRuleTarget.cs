using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteOptionRuleTarget
    {
        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal Rate { get; set; }

        public Decimal Percentage { get; set; }

        public bool IsBlock { get; set; }
    }
}
