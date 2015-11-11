using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanDataManager : IDataManager
    {
        bool InsertPriceList(SalePriceList priceList, out int priceListId);

        Changes GetChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status);

        bool InsertOrUpdateChanges(SalePriceListOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status);

        bool sp_RatePlan_SetStatusIfExists(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status);
    }
}
