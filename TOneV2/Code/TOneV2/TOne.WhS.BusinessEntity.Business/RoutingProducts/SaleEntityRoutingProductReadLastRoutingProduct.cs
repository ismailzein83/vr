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
    public class SaleEntityRoutingProductReadLastRoutingProduct : ISaleEntityRoutingProductReader
    {
        SaleZoneRoutingProductsByOwner _allSaleZoneRoutingProductsByOwner;
        Dictionary<int, List<DefaultRoutingProduct>> _defaultRoutingProductByCustomerId;
        Dictionary<int, List<DefaultRoutingProduct>> _defaultRoutingProductBySellingProductId;
        public SaleEntityRoutingProductReadLastRoutingProduct()
        {
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
            if (zoneId.HasValue)
            {
                List<DefaultRoutingProduct> defaultRoutingProducts = null;
                if (ownerType == SalePriceListOwnerType.Customer)
                    _defaultRoutingProductByCustomerId.TryGetValue(ownerId, out defaultRoutingProducts);
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                    _defaultRoutingProductBySellingProductId.TryGetValue(ownerId, out defaultRoutingProducts);

                if (defaultRoutingProducts != null)
                    return defaultRoutingProducts.FindRecord(defaultRp => defaultRp.EED == null);
            }
            return null;
        }

        private void LoadDictionaries()
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            _allSaleZoneRoutingProductsByOwner = new SaleZoneRoutingProductsByOwner { SaleZoneRoutingProductsByCustomer = new Dictionary<int, SaleZoneRoutingProductsByZone>(), SaleZoneRoutingProductsByProduct = new Dictionary<int, SaleZoneRoutingProductsByZone>() };

            var allZoneRPs = routingProductManager.GetAllCachedSaleZoneRoutingProducts();
            foreach (var zoneRPsByOwner in allZoneRPs)
            {
                SaleZoneRoutingProductsByZone newZoneRPsByZone = new SaleZoneRoutingProductsByZone();
                foreach (var zoneRPByZone in zoneRPsByOwner.Value)
                {
                    long zoneId = zoneRPByZone.Key;
                    var lastZoneRP = zoneRPByZone.Value.FindRecord(itm => itm.EED == null);
                    if (lastZoneRP != null)
                        newZoneRPsByZone.Add(zoneId, lastZoneRP);
                }
                if (zoneRPsByOwner.Key.OwnerType == SalePriceListOwnerType.Customer)
                    _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByCustomer.Add(zoneRPsByOwner.Key.OwnerId, newZoneRPsByZone);
                else
                    _allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByProduct.Add(zoneRPsByOwner.Key.OwnerId, newZoneRPsByZone);
            }

            var allDefaultRPs = routingProductManager.GetAllCachedDefaultRoutingProducts();
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

    }
}