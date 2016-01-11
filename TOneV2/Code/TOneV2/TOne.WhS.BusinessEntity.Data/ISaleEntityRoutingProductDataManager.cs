﻿using System;
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

        bool InsertDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, NewDefaultRoutingProduct newDefaultRoutingProducts);

        bool UpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DefaultRoutingProductChange defaultRoutingProductChange);

        bool InsertZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<NewZoneRoutingProduct> newZoneRoutingProducts);

        bool UpdateZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ZoneRoutingProductChange> zoneRoutingProductChanges);

        bool AreSaleEntityRoutingProductUpdated(ref object updateHandle);
    }
}
