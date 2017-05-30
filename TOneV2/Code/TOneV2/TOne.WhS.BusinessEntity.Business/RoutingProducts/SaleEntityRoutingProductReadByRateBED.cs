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
        DefaultRoutingProductsByOwner _allDefaultRoutingProductByOwner;
        Dictionary<long, DateTime> _zoneIdsWithRateBED;
        Dictionary<int, List<DefaultRoutingProduct>> _defaultRoutingProductByCustomerId;
        Dictionary<int, List<DefaultRoutingProduct>> _defaultRoutingProductBySellingProductId;
        public SaleEntityRoutingProductReadByRateBED(IEnumerable<int> customerIds, Dictionary<long, DateTime> zoneIdsWithRateBED)
        {
            this._zoneIdsWithRateBED = zoneIdsWithRateBED;
            DateTime minimumDate = zoneIdsWithRateBED.Min(itm => itm.Value);
            _allSaleZoneRoutingProductsByOwner = GetAllZoneRoutingProduct(customerIds, minimumDate);
            GetAllDefaultRoutingProduct(customerIds, minimumDate);
        }

        private void GetAllDefaultRoutingProduct(IEnumerable<int> customerIds, DateTime minimumDate)
        {
            _defaultRoutingProductByCustomerId = new Dictionary<int, List<DefaultRoutingProduct>>();
            _defaultRoutingProductBySellingProductId = new Dictionary<int, List<DefaultRoutingProduct>>();
            ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = saleEntityRoutingProductDataManager.GetDefaultRoutingProductsEffectiveAfter(customerIds, minimumDate);
            foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts)
            {
                if (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.Customer)
                {
                    List<DefaultRoutingProduct> defaultCustomerRoutingProduct = _defaultRoutingProductByCustomerId.GetOrCreateItem(defaultRoutingProduct.OwnerId);
                    defaultCustomerRoutingProduct.Add(defaultRoutingProduct);
                }
                if (defaultRoutingProduct.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    List<DefaultRoutingProduct> defaultSPRoutingProduct = _defaultRoutingProductBySellingProductId.GetOrCreateItem(defaultRoutingProduct.OwnerId);
                    defaultSPRoutingProduct.Add(defaultRoutingProduct);
                }
            }
        }

        private SaleZoneRoutingProductsByOwner GetAllZoneRoutingProduct(IEnumerable<int> customerIds, DateTime minimumDate)
        {
            SaleZoneRoutingProductsByOwner saleZoneRoutingProductsByOwnerResult = new SaleZoneRoutingProductsByOwner
            {
                SaleZoneRoutingProductsByCustomer = new Dictionary<int, SaleZoneRoutingProductsByZone>(),
                SaleZoneRoutingProductsByProduct = new Dictionary<int, SaleZoneRoutingProductsByZone>()
            };
            ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            IEnumerable<SaleZoneRoutingProduct> saleZoneRoutingProducts = saleEntityRoutingProductDataManager.GetSaleZoneRoutingProductsByZoneIds(customerIds,
                minimumDate, this._zoneIdsWithRateBED.Keys.ToList());

            foreach (var saleZoneRoutingProduct in saleZoneRoutingProducts)
            {
                DateTime zoneEffectiveDateTime;
                if (!this._zoneIdsWithRateBED.TryGetValue(saleZoneRoutingProduct.SaleZoneId, out zoneEffectiveDateTime))
                    continue; // Throw Exception
                if (saleZoneRoutingProduct.BED <= zoneEffectiveDateTime && (!saleZoneRoutingProduct.EED.HasValue || saleZoneRoutingProduct.EED < zoneEffectiveDateTime))
                {
                    Dictionary<int, SaleZoneRoutingProductsByZone> saleZoneRoutingProductsByOwner =
                        saleZoneRoutingProduct.OwnerType == SalePriceListOwnerType.Customer
                            ? saleZoneRoutingProductsByOwnerResult.SaleZoneRoutingProductsByCustomer
                            : saleZoneRoutingProductsByOwnerResult.SaleZoneRoutingProductsByProduct;
                    SaleZoneRoutingProductsByZone saleZoneRoutingProductsByZone;
                    if (!saleZoneRoutingProductsByOwner.TryGetValue(saleZoneRoutingProduct.OwnerId, out saleZoneRoutingProductsByZone))
                    {
                        saleZoneRoutingProductsByZone = new SaleZoneRoutingProductsByZone();
                        saleZoneRoutingProductsByOwner.Add(saleZoneRoutingProduct.OwnerId, saleZoneRoutingProductsByZone);
                    }
                    if (!saleZoneRoutingProductsByZone.ContainsKey(saleZoneRoutingProduct.SaleZoneId))
                    {
                        saleZoneRoutingProductsByZone.Add(saleZoneRoutingProduct.SaleZoneId, saleZoneRoutingProduct);
                    }
                }
            }
            return saleZoneRoutingProductsByOwnerResult;
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
            if (_zoneIdsWithRateBED.TryGetValue(zoneId.Value, out rateBED))
            {
                List<DefaultRoutingProduct> defaultRoutingProducts = new List<DefaultRoutingProduct>();
                if (ownerType == SalePriceListOwnerType.Customer)
                    _defaultRoutingProductByCustomerId.TryGetValue(ownerId, out defaultRoutingProducts);
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                    _defaultRoutingProductBySellingProductId.TryGetValue(ownerId, out defaultRoutingProducts);

                if (defaultRoutingProducts != null)
                    return defaultRoutingProducts.FirstOrDefault(defaultRp => defaultRp.BED <= rateBED && (!defaultRp.EED.HasValue || defaultRp.EED > rateBED));
            }
            return null;
        }
    }
}
