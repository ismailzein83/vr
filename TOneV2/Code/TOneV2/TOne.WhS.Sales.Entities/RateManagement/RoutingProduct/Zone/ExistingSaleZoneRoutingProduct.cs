using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingSaleZoneRoutingProduct
    {
        public SaleZoneRoutingProduct SaleZoneRoutingProductEntity { get; set; }

        public ChangedSaleZoneRoutingProduct ChangedSaleZoneRoutingProduct { get; set; }

        public DateTime BED
        {
            get { return SaleZoneRoutingProductEntity.BED; }
        }

        public DateTime? EED
        {
            get { return ChangedSaleZoneRoutingProduct != null ? ChangedSaleZoneRoutingProduct.EED : SaleZoneRoutingProductEntity.EED; }
        }
    }
}
