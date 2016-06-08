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
        public DefaultRoutingProduct DefaultRoutingProductEntity { get; set; }

        public ChangedDefaultRoutingProduct ChangedDefaultRoutingProduct { get; set; }

        public DateTime BED 
        {
            get { return DefaultRoutingProductEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedDefaultRoutingProduct != null ? ChangedDefaultRoutingProduct.EED : DefaultRoutingProductEntity.EED; }
        }
    }
}
