using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityRoutingProductReadAllNoCache : ISaleEntityRoutingProductReader
    {
        SaleZoneRoutingProductsByOwner _allSaleZoneRoutingProductsByOwner;
        DefaultRoutingProductsByOwner _allDefaultRoutingProductByOwner;
        ISaleEntityRoutingProductDataManager _saleEntityRoutingProductDataManager;


        public SaleEntityRoutingProductReadAllNoCache(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            _allSaleZoneRoutingProductsByOwner = GetAllSaleZoneRoutingProductsByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
            _allDefaultRoutingProductByOwner = GetAllDefaultRoutingProductsByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
        }

        private DefaultRoutingProductsByOwner GetAllDefaultRoutingProductsByOwner(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DefaultRoutingProductsByOwner result = new DefaultRoutingProductsByOwner();
            result.DefaultRoutingProductsByCustomer = new Dictionary<int, DefaultRoutingProduct>();
            result.DefaultRoutingProductsByProduct = new Dictionary<int, DefaultRoutingProduct>();
            List<DefaultRoutingProduct> defaultRoutingProducts = _saleEntityRoutingProductDataManager.GetDefaultRoutingProducts(customerInfos, effectiveOn, isEffectiveInFuture).ToList();

            foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts)
            {
                Dictionary<int, DefaultRoutingProduct> defaultRoutingProductsByOwner = defaultRoutingProduct.OwnerType == SalePriceListOwnerType.Customer ? result.DefaultRoutingProductsByCustomer : result.DefaultRoutingProductsByProduct;

                if (!defaultRoutingProductsByOwner.ContainsKey(defaultRoutingProduct.OwnerId))
                    defaultRoutingProductsByOwner.Add(defaultRoutingProduct.OwnerId, defaultRoutingProduct);
            }
            return result;
        }

        private SaleZoneRoutingProductsByOwner GetAllSaleZoneRoutingProductsByOwner(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleZoneRoutingProductsByOwner result = new SaleZoneRoutingProductsByOwner();

            result.SaleZoneRoutingProductsByCustomer = new Dictionary<int, SaleZoneRoutingProductsByZone>();
            result.SaleZoneRoutingProductsByProduct = new Dictionary<int, SaleZoneRoutingProductsByZone>();

            IEnumerable<SaleZoneRoutingProduct> saleZoneRoutingProducts = _saleEntityRoutingProductDataManager.GetSaleZoneRoutingProducts(customerInfos, effectiveOn, isEffectiveInFuture);

            foreach (SaleZoneRoutingProduct saleZoneRoutingProduct in saleZoneRoutingProducts)
            {
                Dictionary<int, SaleZoneRoutingProductsByZone> saleZoneRoutingProductsByOwner = saleZoneRoutingProduct.OwnerType == SalePriceListOwnerType.Customer ? result.SaleZoneRoutingProductsByCustomer : result.SaleZoneRoutingProductsByProduct;
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
            return result;
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
