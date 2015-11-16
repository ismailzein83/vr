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
                var defaultRoutingProductsByOwner = ownerType == Entities.SalePriceListOwnerType.Customer ? _allDefaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer : _allDefaultRoutingProductsByOwner.DefaultRoutingProductsByProduct;
                DefaultRoutingProduct defaultRoutingProduct;
                if (defaultRoutingProductsByOwner.TryGetValue(ownerId, out defaultRoutingProduct))
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
                    DefaultRoutingProductsByOwner defaultRoutingProductsByOwner = null;

                    IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
                    IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = dataManager.GetEffectiveDefaultRoutingProducts(DateTime.Now);

                    if (defaultRoutingProducts != null && defaultRoutingProducts.Count() > 0) // GetItemsSP returns an empty list instead of null
                    {
                        defaultRoutingProductsByOwner = new DefaultRoutingProductsByOwner();

                        foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts)
                        {
                            if (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct)
                            {
                                if (defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct == null)
                                    defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct = new Dictionary<int, DefaultRoutingProduct>();

                                defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct.Add(defaultRoutingProduct.OwnerId, defaultRoutingProduct);
                            }
                            else
                            {
                                if (defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer == null)
                                    defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer = new Dictionary<int, DefaultRoutingProduct>();

                                defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer.Add(defaultRoutingProduct.OwnerId, defaultRoutingProduct);
                            }
                        }
                    }

                    return defaultRoutingProductsByOwner;
                });
        }
    }
}
