using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityRoutingProductReadAllNoCache : ISaleEntityRoutingProductReader
    {
        SaleZoneRoutingProductsByOwner _allSaleZoneRoutingProductsByOwner;
        DefaultRoutingProductsByOwner _allDefaultRoutingProductByOwner;

        public SaleEntityRoutingProductReadAllNoCache(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _allSaleZoneRoutingProductsByOwner = GetAllSaleZoneRoutingProductsByOwner(effectiveOn, isEffectiveInFuture);
            _allDefaultRoutingProductByOwner = GetAllDefaultRoutingProductsByOwner(effectiveOn, isEffectiveInFuture);
        }

        private DefaultRoutingProductsByOwner GetAllDefaultRoutingProductsByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        private SaleZoneRoutingProductsByOwner GetAllSaleZoneRoutingProductsByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public SaleZoneRoutingProductsByZone GetRoutingProductsOnZones(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var saleZoneRoutingProductsByOwner = ownerType == SalePriceListOwnerType.Customer ? _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByCustomer : _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByProduct;
            SaleZoneRoutingProductsByZone saleZoneRoutingProductByZone;
            saleZoneRoutingProductsByOwner.TryGetValue(ownerId, out saleZoneRoutingProductByZone);
            return saleZoneRoutingProductByZone;                   
        }

        public Entities.DefaultRoutingProduct GetDefaultRoutingProduct(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var defaultRoutingProductsByOwner = ownerType == SalePriceListOwnerType.Customer ? _allDefaultRoutingProductByOwner.DefaultRoutingProductsByCustomer : _allDefaultRoutingProductByOwner.DefaultRoutingProductsByProduct;
            DefaultRoutingProduct defaultRoutingProduct;
            defaultRoutingProductsByOwner.TryGetValue(ownerId, out defaultRoutingProduct);
            return defaultRoutingProduct;
        }
    }
}
