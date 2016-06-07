using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingDefaultRoutingProduct
    {
        public int RoutingProductId { get; set; }

        public DefaultRoutingProduct DefaultRoutingProductEntity { get; set; }

        public ChangedDefaultRoutingProduct ChangedRoutingProduct { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
