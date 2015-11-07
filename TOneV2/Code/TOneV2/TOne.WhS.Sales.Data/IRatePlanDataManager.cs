using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanDataManager : IDataManager
    {
        bool InsertSalePriceList(SalePriceList salePriceList, out int salePriceListId);

        bool CloseAndInsertSaleRates(int customerId, List<SaleRate> newSaleRates);

        Changes GetChanges(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status);

        bool InsertOrUpdateChanges(RatePlanOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status);

        #region Junk Code
        /*
        bool SetRatePlanStatusIfExists(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status);

        RatePlan GetRatePlan(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status);

        bool InsertOrUpdateRatePlan(RatePlan ratePlan);
        */
        #endregion
    }
}
