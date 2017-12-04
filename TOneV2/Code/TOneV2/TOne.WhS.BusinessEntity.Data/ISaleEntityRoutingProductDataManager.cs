using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleEntityRoutingProductDataManager : IDataManager
    {
        List<SaleZoneRoutingProduct> GetAllZoneRoutingProducts();

        List<DefaultRoutingProduct> GetAllDefaultRoutingProducts();

        IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture);

        IEnumerable<DefaultRoutingProduct> GetEffectiveDefaultRoutingProducts(DateTime effectiveOn);

        IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture);

        IEnumerable<SaleZoneRoutingProduct> GetEffectiveZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn);

        bool AreSaleEntityRoutingProductUpdated(ref object updateHandle);

        IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);

        IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);

        IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId);

        IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds);

        IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId);

        IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId, IEnumerable<long> saleZoneIds);
        IEnumerable<SaleZoneRoutingProduct> GetExistingZoneRoutingProductsByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED);

        IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds);
        IEnumerable<SaleZoneRoutingProduct> GetAllSaleZoneRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds);

        IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds);
        IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds, IEnumerable<long> zoneIds);
    }
}
