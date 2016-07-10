using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class UpdateState : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);

            var dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            dataManager.UpdateRatePlanStatus(ownerType, ownerId, RatePlanStatus.Draft, RatePlanStatus.Completed);
        }
    }
}
