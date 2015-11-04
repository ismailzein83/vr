using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityRoutingProductManager
    {
        public int GetCustomerZoneRoutingProductId(int customerId, int sellingProductId, long saleZoneId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            int routingProductId;
            if (!HasRPOnZone(SalePriceListOwnerType.Customer, customerId, saleZoneId, effectiveOn, isEffectiveInFuture, out routingProductId))
                if (!HasDefaultRP(SalePriceListOwnerType.Customer, customerId, effectiveOn, isEffectiveInFuture, out routingProductId))
                    if (!HasRPOnZone(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, effectiveOn, isEffectiveInFuture, out routingProductId))
                        HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, effectiveOn, isEffectiveInFuture, out routingProductId);
            return routingProductId;
        }

        private bool HasRPOnZone(SalePriceListOwnerType ownerType, int ownerId, long saleZoneId, DateTime? effectiveOn, bool isEffectiveInFuture, out int routingProductId)
        {
            SaleZoneRoutingProduct saleZoneRoutingProduct;
            SaleZoneRoutingProductsByOwner allSaleZoneRoutingProductsByOwner = GetCachedSaleZoneRPsByOwner(effectiveOn, isEffectiveInFuture);
            if (allSaleZoneRoutingProductsByOwner != null)
            {
                var saleZoneRoutingProductsByOwner = ownerType == SalePriceListOwnerType.Customer ? allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByCustomer : allSaleZoneRoutingProductsByOwner.SaleZoneRoutingProductsByProduct;
                if (saleZoneRoutingProductsByOwner != null)
                {
                    SaleZoneRoutingProductsByZone saleZoneRoutingProductByZone;
                    if (saleZoneRoutingProductsByOwner.TryGetValue(ownerId, out saleZoneRoutingProductByZone))
                    {
                        if (saleZoneRoutingProductByZone.TryGetValue(saleZoneId, out saleZoneRoutingProduct))
                        {
                            routingProductId = saleZoneRoutingProduct.RoutingProductId;
                            return true;
                        }
                    }
                }
            }
            routingProductId = 0;
            return false;
        }
        
        private bool HasDefaultRP(SalePriceListOwnerType ownerType, int ownerId, DateTime? effectiveOn, bool isEffectiveInFuture, out int routingProductId)
        {
            DefaultRoutingProduct defaultRoutingProduct;
            DefaultRoutingProductByOwner allDefaultRoutingProductByOwner = GetCachedDefaultRPsByOwner(effectiveOn, isEffectiveInFuture);
            if (allDefaultRoutingProductByOwner != null)
            {
                var defaultRoutingProductsByOwner = ownerType == SalePriceListOwnerType.Customer ? allDefaultRoutingProductByOwner.DefaultRoutingProductsByCustomer : allDefaultRoutingProductByOwner.DefaultRoutingProductsByProduct;
                if (defaultRoutingProductsByOwner != null)
                {
                    if (defaultRoutingProductsByOwner.TryGetValue(ownerId, out defaultRoutingProduct))
                    {
                        routingProductId = defaultRoutingProduct.RoutingProductId;
                        return true;
                    }
                }
            }
            routingProductId = 0;
            return false;
        }

        private SaleZoneRoutingProductsByOwner GetCachedSaleZoneRPsByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        private DefaultRoutingProductByOwner GetCachedDefaultRPsByOwner(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        #region Private Classes

        private class SaleZoneRoutingProductsByOwner
        {
            public Dictionary<int, SaleZoneRoutingProductsByZone> SaleZoneRoutingProductsByCustomer { get; set; }

            public Dictionary<int, SaleZoneRoutingProductsByZone> SaleZoneRoutingProductsByProduct { get; set; }
        }

        private class SaleZoneRoutingProductsByZone : Dictionary<long, SaleZoneRoutingProduct>
        {

        }

        private class DefaultRoutingProductByOwner
        {
            public Dictionary<int, DefaultRoutingProduct> DefaultRoutingProductsByCustomer { get; set; }

            public Dictionary<int, DefaultRoutingProduct> DefaultRoutingProductsByProduct { get; set; }
        }

        #endregion
    }
}
