using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IRatePlanDataManager : IDataManager
    {
        bool InsertSalePriceList(SalePriceList salePriceList, out int salePriceListId);

        bool CloseAndInsertSaleRates(int customerId, List<SaleRate> newSaleRates);

        bool SetRatePlanStatusIfExists(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status);

        RatePlan GetRatePlan(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status);

        bool InsertOrUpdateRatePlan(RatePlan ratePlan);
    }
}
