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
    public class SaleEntityRoutingProductReadByRateBED : ISaleEntityRoutingProductReader
    {
        SaleZoneRoutingProductsByOwner _allSaleZoneRoutingProductsByOwner;
        Dictionary<long, DateTime> _zoneIdsWithRateBED;
        Dictionary<int, List<DefaultRoutingProduct>> _defaultRoutingProductByCustomerId;
        Dictionary<int, List<DefaultRoutingProduct>> _defaultRoutingProductBySellingProductId;
        public SaleEntityRoutingProductReadByRateBED(IEnumerable<int> customerIds, Dictionary<long, DateTime> zoneIdsWithRateBED)
        {
            this._zoneIdsWithRateBED = zoneIdsWithRateBED;
            LoadDictionaries();
        }
        
        public SaleZoneRoutingProductsByZone GetRoutingProductsOnZones(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            var saleZoneRoutingProductsByOwner = ownerType == SalePriceListOwnerType.Customer
                ? _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByCustomer
                : _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByProduct;
            SaleZoneRoutingProductsByZone saleZoneRoutingProductByZone;
            saleZoneRoutingProductsByOwner.TryGetValue(ownerId, out saleZoneRoutingProductByZone);
            return saleZoneRoutingProductByZone;
        }

        public DefaultRoutingProduct GetDefaultRoutingProduct(Entities.SalePriceListOwnerType ownerType, int ownerId, long? zoneId)
        {
            DateTime rateBED;
            if (zoneId.HasValue && _zoneIdsWithRateBED.TryGetValue(zoneId.Value, out rateBED))
            {
                List<DefaultRoutingProduct> defaultRoutingProducts = null;
                if (ownerType == SalePriceListOwnerType.Customer)
                    _defaultRoutingProductByCustomerId.TryGetValue(ownerId, out defaultRoutingProducts);
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                    _defaultRoutingProductBySellingProductId.TryGetValue(ownerId, out defaultRoutingProducts);

                if (defaultRoutingProducts != null)
                    return defaultRoutingProducts.FindRecord(defaultRp => defaultRp.IsEffective(rateBED));
            }
            return null;
        }


        #region Private Methods

        private struct OwnerKey
        {
            public SalePriceListOwnerType OwnerType { get; set; }
            public int OwnerId { get; set; }
        }

        private void LoadDictionaries()
        {
            var allZoneRPs = GetAllCachedSaleZoneRoutingProducts();
            _allSaleZoneRoutingProductsByOwner = new SaleZoneRoutingProductsByOwner { SaleZoneRoutingProductsByCustomer = new Dictionary<int, SaleZoneRoutingProductsByZone>(), SaleZoneRoutingProductsByProduct = new Dictionary<int, SaleZoneRoutingProductsByZone>() };
            foreach (var zoneRPsByOwner in allZoneRPs)
            {
                SaleZoneRoutingProductsByZone newZoneRPsByZone = new SaleZoneRoutingProductsByZone();
                foreach (var zoneRPByZone in zoneRPsByOwner.Value)
                {
                    long zoneId = zoneRPByZone.Key;
                    DateTime zoneRateBED;
                    if (this._zoneIdsWithRateBED.TryGetValue(zoneId, out zoneRateBED))
                    {
                        var effectiveZoneRP = zoneRPByZone.Value.FindRecord(itm => itm.IsEffective(zoneRateBED));
                        if (effectiveZoneRP != null)
                            newZoneRPsByZone.Add(zoneId, effectiveZoneRP);
                    }
                }
                if (zoneRPsByOwner.Key.OwnerType == SalePriceListOwnerType.Customer)
                    _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByCustomer.Add(zoneRPsByOwner.Key.OwnerId, newZoneRPsByZone);
                else
                    _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByProduct.Add(zoneRPsByOwner.Key.OwnerId, newZoneRPsByZone);
            }

            var allDefaultRPs = GetAllCachedDefaultRoutingProducts();
            _defaultRoutingProductByCustomerId = new Dictionary<int, List<DefaultRoutingProduct>>();
            _defaultRoutingProductBySellingProductId = new Dictionary<int, List<DefaultRoutingProduct>>();
            foreach (var defaultRPsByOwner in allDefaultRPs)
            {
                if (defaultRPsByOwner.Key.OwnerType == SalePriceListOwnerType.Customer)
                    _defaultRoutingProductByCustomerId.Add(defaultRPsByOwner.Key.OwnerId, defaultRPsByOwner.Value);
                else
                    _defaultRoutingProductBySellingProductId.Add(defaultRPsByOwner.Key.OwnerId, defaultRPsByOwner.Value);
            }
        }

        private Dictionary<OwnerKey, Dictionary<long, List<SaleZoneRoutingProduct>>> GetAllCachedSaleZoneRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>().GetOrCreateObject("GetAllCachedSaleZoneRoutingProducts", () =>
            {
                ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                var allSaleZoneRoutingProducts = saleEntityRoutingProductDataManager.GetAllZoneRoutingProducts();
                Dictionary<OwnerKey, Dictionary<long, List<SaleZoneRoutingProduct>>> rslt = new Dictionary<OwnerKey, Dictionary<long, List<SaleZoneRoutingProduct>>>();

                if (allSaleZoneRoutingProducts != null)
                {
                    foreach (var szRP in allSaleZoneRoutingProducts)
                    {
                        rslt.GetOrCreateItem(new OwnerKey { OwnerId = szRP.OwnerId, OwnerType = szRP.OwnerType }).GetOrCreateItem(szRP.SaleZoneId).Add(szRP);
                    }
                }
                return rslt;
            });
        }

        private Dictionary<OwnerKey, List<DefaultRoutingProduct>> GetAllCachedDefaultRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityRoutingProductCacheManager>().GetOrCreateObject("GetAllCachedDefaultRoutingProducts", () =>
            {
                ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                var allSaleZoneRoutingProducts = saleEntityRoutingProductDataManager.GetAllDefaultRoutingProducts();
                Dictionary<OwnerKey, List<DefaultRoutingProduct>> rslt = new Dictionary<OwnerKey, List<DefaultRoutingProduct>>();

                if (allSaleZoneRoutingProducts != null)
                {
                    foreach (var szRP in allSaleZoneRoutingProducts)
                    {
                        rslt.GetOrCreateItem(new OwnerKey { OwnerId = szRP.OwnerId, OwnerType = szRP.OwnerType }).Add(szRP);
                    }
                }
                return rslt;
            });
        }

        #endregion
    }
}
