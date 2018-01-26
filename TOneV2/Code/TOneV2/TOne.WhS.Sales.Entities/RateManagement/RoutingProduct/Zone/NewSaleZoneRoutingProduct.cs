using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class NewSaleZoneRoutingProduct
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public long SaleEntityRoutingProductId { get; set; }

        public int RoutingProductId { get; set; }

        public long SaleZoneId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
