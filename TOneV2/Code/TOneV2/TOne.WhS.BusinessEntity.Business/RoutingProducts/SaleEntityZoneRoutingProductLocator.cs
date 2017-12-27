using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityZoneRoutingProductLocator
    {
        ISaleEntityRoutingProductReader _reader;

        public SaleEntityZoneRoutingProductLocator(ISaleEntityRoutingProductReader reader)
        {
            _reader = reader;
        }

        public SaleEntityZoneRoutingProduct GetCustomerDefaultRoutingProduct(int customerId, int sellingProductId)
        {
            SaleEntityZoneRoutingProduct defaultRoutingProduct;
            if (!HasDefaultRP(SalePriceListOwnerType.Customer, customerId, out defaultRoutingProduct))
                HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out defaultRoutingProduct);

            return defaultRoutingProduct;
        }

        public SaleEntityZoneRoutingProduct GetSellingProductDefaultRoutingProduct(int sellingProductId)
        {
            SaleEntityZoneRoutingProduct defaultRoutingProduct;
            HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out defaultRoutingProduct);
            return defaultRoutingProduct;
        }

        public SaleEntityZoneRoutingProduct GetCustomerZoneRoutingProduct(int customerId, int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRoutingProduct customerZoneRoutingProduct;
            if (!HasRPOnZone(SalePriceListOwnerType.Customer, customerId, saleZoneId, out customerZoneRoutingProduct))
                if (!HasDefaultRP(SalePriceListOwnerType.Customer, customerId, out customerZoneRoutingProduct, saleZoneId))
                    if (!HasRPOnZone(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRoutingProduct))
                        HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out customerZoneRoutingProduct, saleZoneId);
            return customerZoneRoutingProduct;
        }

        public SaleEntityZoneRoutingProduct GetSellingProductZoneRoutingProduct(int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRoutingProduct customerZoneRoutingProduct;
            if (!HasRPOnZone(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRoutingProduct))
                HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out customerZoneRoutingProduct, saleZoneId);
            return customerZoneRoutingProduct;
        }

        private bool HasRPOnZone(SalePriceListOwnerType ownerType, int ownerId, long saleZoneId, out  SaleEntityZoneRoutingProduct customerZoneRoutingProduct)
        {
            var saleZoneRoutingProductByZone = _reader.GetRoutingProductsOnZones(ownerType, ownerId);
            if (saleZoneRoutingProductByZone != null)
            {
                SaleZoneRoutingProduct saleZoneRoutingProduct;
                if (saleZoneRoutingProductByZone.TryGetValue(saleZoneId, out saleZoneRoutingProduct))
                {
                    customerZoneRoutingProduct = new SaleEntityZoneRoutingProduct
                    {
                        SaleEntityZoneRoutingProductId = saleZoneRoutingProduct.SaleEntityRoutingProductId,
                        RoutingProductId = saleZoneRoutingProduct.RoutingProductId,
                        BED = saleZoneRoutingProduct.BED,
                        EED = saleZoneRoutingProduct.EED,
                        Source = ownerType == SalePriceListOwnerType.Customer ? SaleEntityZoneRoutingProductSource.CustomerZone : SaleEntityZoneRoutingProductSource.ProductZone
                    };
                    return true;
                }
            }
            customerZoneRoutingProduct = null;
            return false;
        }

        private bool HasDefaultRP(SalePriceListOwnerType ownerType, int ownerId, out SaleEntityZoneRoutingProduct customerZoneRoutingProduct, long? zoneId = null)
        {
            DefaultRoutingProduct defaultRoutingProduct = _reader.GetDefaultRoutingProduct(ownerType, ownerId, zoneId);

            if (defaultRoutingProduct != null)
            {
                customerZoneRoutingProduct = new SaleEntityZoneRoutingProduct
                {
                    SaleEntityZoneRoutingProductId = defaultRoutingProduct.SaleEntityRoutingProductId,
                    RoutingProductId = defaultRoutingProduct.RoutingProductId,
                    Source = ownerType == SalePriceListOwnerType.Customer ? SaleEntityZoneRoutingProductSource.CustomerDefault : SaleEntityZoneRoutingProductSource.ProductDefault,
                    BED = defaultRoutingProduct.BED,
                    EED = defaultRoutingProduct.EED
                };
                return true;
            }
            customerZoneRoutingProduct = null;
            return false;
        }
    }
}
