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
        
        public DefaultRoutingProduct GetCustomerDefaultRoutingProduct(int customerId, int sellingProductId)
        {
            DefaultRoutingProduct defaultRoutingProduct;
            if (!HasDefaultRP(SalePriceListOwnerType.Customer, customerId, out defaultRoutingProduct))
                HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out defaultRoutingProduct);

            return defaultRoutingProduct;
        }

        public DefaultRoutingProduct GetSellingProductDefaultRoutingProduct(int sellingProductId)
        {
            DefaultRoutingProduct defaultRoutingProduct;
            HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out defaultRoutingProduct);
            return defaultRoutingProduct;
        }

        public SaleEntityZoneRoutingProduct GetCustomerZoneRoutingProduct(int customerId, int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRoutingProduct customerZoneRoutingProduct;
            if (!HasRPOnZone(SalePriceListOwnerType.Customer, customerId, saleZoneId, out customerZoneRoutingProduct))
                if (!HasDefaultRP(SalePriceListOwnerType.Customer, customerId, out customerZoneRoutingProduct))
                    if (!HasRPOnZone(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRoutingProduct))
                        HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out customerZoneRoutingProduct);
            return customerZoneRoutingProduct;
        }

        public SaleEntityZoneRoutingProduct GetSellingProductZoneRoutingProduct(int sellingProductId, long saleZoneId)
        {
            SaleEntityZoneRoutingProduct customerZoneRoutingProduct;
            if (!HasRPOnZone(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRoutingProduct))
                HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out customerZoneRoutingProduct);
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
                        RoutingProductId = saleZoneRoutingProduct.RoutingProductId,
                        Source = ownerType == SalePriceListOwnerType.Customer ? SaleEntityZoneRoutingProductSource.CustomerZone : SaleEntityZoneRoutingProductSource.ProductZone
                    };
                    return true;
                }
            }
            customerZoneRoutingProduct = null;
            return false;
        }

        private bool HasDefaultRP(SalePriceListOwnerType ownerType, int ownerId, out SaleEntityZoneRoutingProduct customerZoneRoutingProduct)
        {
            DefaultRoutingProduct defaultRoutingProduct = _reader.GetDefaultRoutingProduct(ownerType, ownerId);

            if (defaultRoutingProduct != null)
            {
                customerZoneRoutingProduct = new SaleEntityZoneRoutingProduct
                {
                    RoutingProductId = defaultRoutingProduct.RoutingProductId,
                    Source = ownerType == SalePriceListOwnerType.Customer ? SaleEntityZoneRoutingProductSource.CustomerDefault : SaleEntityZoneRoutingProductSource.ProductDefault
                };
                return true;
            }
            customerZoneRoutingProduct = null;
            return false;
        }

        private bool HasDefaultRP(SalePriceListOwnerType ownerType, int ownerId, out DefaultRoutingProduct defaultRoutingProduct)
        {
            defaultRoutingProduct = _reader.GetDefaultRoutingProduct(ownerType, ownerId);
            return (defaultRoutingProduct != null);
        }
    }
}
