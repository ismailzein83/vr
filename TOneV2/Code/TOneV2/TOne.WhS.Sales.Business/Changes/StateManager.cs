using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class StateManager
    {
        #region Public Methods

        public Changes GetChanges(SalePriceListOwnerType ownerType, int ownerId)
        {
            IRatePlanDataManager dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
        }

        #endregion
    }
}
