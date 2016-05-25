using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanDataManager : IDataManager
    {
        bool InsertPriceList(SalePriceList priceList, out int priceListId);

        Changes GetChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status);

        bool InsertOrUpdateChanges(SalePriceListOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status);

        bool UpdateRatePlanStatus(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus existingStatus, RatePlanStatus newStatus);

        bool CancelRatePlanChanges(SalePriceListOwnerType ownerType, int ownerId);
    }
}
