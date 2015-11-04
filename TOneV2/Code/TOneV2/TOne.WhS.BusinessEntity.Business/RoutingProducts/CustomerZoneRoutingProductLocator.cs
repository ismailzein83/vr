using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneRoutingProductLocator
    {
        ISaleEntityRoutingProductReader _reader;
        public CustomerZoneRoutingProductLocator(ISaleEntityRoutingProductReader reader)
        {
            _reader = reader;
        }

        public CustomerZoneRoutingProduct GetCustomerZoneRoutingProduct(int customerId, int sellingProductId, long saleZoneId)
        {
            CustomerZoneRoutingProduct customerZoneRoutingProduct;
            if (!HasRPOnZone(SalePriceListOwnerType.Customer, customerId, saleZoneId, out customerZoneRoutingProduct))
                if (!HasDefaultRP(SalePriceListOwnerType.Customer, customerId, out customerZoneRoutingProduct))
                    if (!HasRPOnZone(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId, out customerZoneRoutingProduct))
                        HasDefaultRP(SalePriceListOwnerType.SellingProduct, sellingProductId, out customerZoneRoutingProduct);
            return customerZoneRoutingProduct;
        }

        private bool HasRPOnZone(SalePriceListOwnerType ownerType, int ownerId, long saleZoneId, out  CustomerZoneRoutingProduct customerZoneRoutingProduct)
        {
            var saleZoneRoutingProductByZone = _reader.GetRoutingProductsOnZones(ownerType, ownerId);
            if (saleZoneRoutingProductByZone != null)
            {
                SaleZoneRoutingProduct saleZoneRoutingProduct;
                if (saleZoneRoutingProductByZone.TryGetValue(saleZoneId, out saleZoneRoutingProduct))
                {
                    customerZoneRoutingProduct = new CustomerZoneRoutingProduct
                    {
                        RoutingProductId = saleZoneRoutingProduct.RoutingProductId,
                        Source = ownerType == SalePriceListOwnerType.Customer ? CustomerZoneRoutingProductSource.CustomerZone : CustomerZoneRoutingProductSource.ProductZone
                    };
                    return true;
                }
            }
            customerZoneRoutingProduct = null;
            return false;
        }

        private bool HasDefaultRP(SalePriceListOwnerType ownerType, int ownerId, out CustomerZoneRoutingProduct customerZoneRoutingProduct)
        {
            DefaultRoutingProduct defaultRoutingProduct = _reader.GetDefaultRoutingProduct(ownerType, ownerId);

            if (defaultRoutingProduct != null)
            {
                customerZoneRoutingProduct = new CustomerZoneRoutingProduct
                {
                    RoutingProductId = defaultRoutingProduct.RoutingProductId,
                    Source = ownerType == SalePriceListOwnerType.Customer ? CustomerZoneRoutingProductSource.CustomerDefault : CustomerZoneRoutingProductSource.ProductDefault
                };
                return true;
            }
            customerZoneRoutingProduct = null;
            return false;
        }
    }
}
