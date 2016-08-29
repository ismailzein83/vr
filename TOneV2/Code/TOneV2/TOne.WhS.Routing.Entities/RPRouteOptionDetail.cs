using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionDetail
    {
        public RPRouteOption Entity { get; set; }
        public string SupplierName { get; set; }
        public decimal ConvertedSupplierRate { get; set; }
    }
}
