using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityRoutingProductReadWithCache : ISaleEntityRoutingProductReader
    {
        DateTime _effectiveOn;
        public SaleEntityRoutingProductReadWithCache(DateTime effectiveOn)
        {
            _effectiveOn = effectiveOn;
        }

        public SaleZoneRoutingProductsByZone GetRoutingProductsOnZones(SalePriceListOwnerType ownerType, int ownerId)
        {
            string cacheName = String.Format("GetRoutingProductsOnZones_{0}_{1}_{2}", ownerType, ownerId, _effectiveOn.Date);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    throw new NotImplementedException();
                    return new SaleZoneRoutingProductsByZone();
                });
        }

        public DefaultRoutingProduct GetDefaultRoutingProduct(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            DefaultRoutingProductsByOwner _allDefaultRoutingProductsByOwner = GetCachedDefaultRoutingProducts();
            if (_allDefaultRoutingProductsByOwner != null)
            {
                var defaultRoutingProductByOwner = ownerType == Entities.SalePriceListOwnerType.Customer ? _allDefaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer : _allDefaultRoutingProductsByOwner.DefaultRoutingProductsByProduct;
                DefaultRoutingProduct defaultRoutingProduct;
                if (defaultRoutingProductByOwner.TryGetValue(ownerId, out defaultRoutingProduct))
                    return defaultRoutingProduct;
            }
            return null;
        }

        private DefaultRoutingProductsByOwner GetCachedDefaultRoutingProducts()
        {
            string cacheName = String.Format("GetCachedDefaultRoutingProducts_{0}", _effectiveOn);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    throw new NotImplementedException();
                    return new DefaultRoutingProductsByOwner();
                });
        }
    }
}
