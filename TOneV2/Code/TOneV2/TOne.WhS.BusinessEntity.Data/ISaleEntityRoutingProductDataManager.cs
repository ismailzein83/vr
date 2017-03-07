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
        IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture);

        IEnumerable<DefaultRoutingProduct> GetEffectiveDefaultRoutingProducts(DateTime effectiveOn);

        IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture);

        IEnumerable<SaleZoneRoutingProduct> GetEffectiveZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn);

        bool AreSaleEntityRoutingProductUpdated(ref object updateHandle);

        IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);

        IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);

        IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsBySellingProducts(IEnumerable<int> sellingProductIds);

        IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsBySellingProducts(IEnumerable<int> sellingProductIds, IEnumerable<long> saleZoneIds);

        IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId);

        IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds);
    }
}
