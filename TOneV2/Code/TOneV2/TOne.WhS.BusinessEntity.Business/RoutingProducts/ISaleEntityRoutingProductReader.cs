using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISaleEntityRoutingProductReader
    {
        SaleZoneRoutingProductsByZone GetRoutingProductsOnZones(SalePriceListOwnerType ownerType, int ownerId);

        DefaultRoutingProduct GetDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId);
    }

    public class SaleZoneRoutingProductsByOwner
    {
        public Dictionary<int, SaleZoneRoutingProductsByZone> SaleZoneRoutingProductsByCustomer { get; set; }

        public Dictionary<int, SaleZoneRoutingProductsByZone> SaleZoneRoutingProductsByProduct { get; set; }
    }

    public class SaleZoneRoutingProductsByZone : Dictionary<long, SaleZoneRoutingProduct>
    {

    }

    public class DefaultRoutingProductsByOwner
    {
        public Dictionary<int, DefaultRoutingProduct> DefaultRoutingProductsByCustomer { get; set; }

        public Dictionary<int, DefaultRoutingProduct> DefaultRoutingProductsByProduct { get; set; }
    }
}
