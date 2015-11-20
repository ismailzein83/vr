using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOption : IRouteOptionOrderTarget, IRouteOptionFilterTarget, IRouteOptionPercentageTarget
    {
        public int SupplierId { get; set; }

        public Decimal SupplierRate { get; set; }

        public Decimal? Percentage { get; set; }
    }
}
