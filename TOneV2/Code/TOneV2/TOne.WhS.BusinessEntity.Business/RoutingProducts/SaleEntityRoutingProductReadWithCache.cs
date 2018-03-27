using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

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
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
                () =>
                {
                    SaleZoneRoutingProductsByZone zoneRoutingProductsByZone = new SaleZoneRoutingProductsByZone();

                    ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                    IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts = dataManager.GetEffectiveZoneRoutingProducts(ownerType, ownerId, _effectiveOn);

                    foreach (SaleZoneRoutingProduct zoneRoutingProduct in zoneRoutingProducts)
                        zoneRoutingProductsByZone.Add(zoneRoutingProduct.SaleZoneId, cacheManager.CacheAndGetSaleZoneRP(zoneRoutingProduct));

                    return zoneRoutingProductsByZone;
                });
        }

        public DefaultRoutingProduct GetDefaultRoutingProduct(Entities.SalePriceListOwnerType ownerType, int ownerId, long? saleZoneId)
        {
            DefaultRoutingProductsByOwner _allDefaultRoutingProductsByOwner = GetCachedDefaultRoutingProducts();
            if (_allDefaultRoutingProductsByOwner != null)
            {
                var defaultRoutingProductsByOwner = ownerType == Entities.SalePriceListOwnerType.Customer ? _allDefaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer : _allDefaultRoutingProductsByOwner.DefaultRoutingProductsByProduct;
                DefaultRoutingProduct defaultRoutingProduct;
                if (defaultRoutingProductsByOwner.TryGetValue(ownerId, out defaultRoutingProduct))
                    return defaultRoutingProduct;
            }
            return null;
        }

        private DefaultRoutingProductsByOwner GetCachedDefaultRoutingProducts()
        {
            string cacheName = String.Format("GetCachedDefaultRoutingProducts_{0}", _effectiveOn.Date);
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
                () =>
                {

                    DefaultRoutingProductsByOwner defaultRoutingProductsByOwner = new DefaultRoutingProductsByOwner();
                    defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct = new Dictionary<int, DefaultRoutingProduct>();
                    defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer = new Dictionary<int, DefaultRoutingProduct>();

                    ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                    IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = dataManager.GetEffectiveDefaultRoutingProducts(_effectiveOn);
					var carrierAccountManager = new CarrierAccountManager();

					if (defaultRoutingProducts != null)
                    {
                        foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts)
                        {
                            var cachedDefaultRP = cacheManager.CacheAndGetDefaultRP(defaultRoutingProduct);
							if (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct)
								defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct.Add(defaultRoutingProduct.OwnerId, cachedDefaultRP);
							else
							{
								if (!carrierAccountManager.IsCarrierAccountDeleted(defaultRoutingProduct.OwnerId))
									defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer.Add(defaultRoutingProduct.OwnerId, cachedDefaultRP);
							}
                        }
                    }

                    return defaultRoutingProductsByOwner;
                });
        }
    }
}
