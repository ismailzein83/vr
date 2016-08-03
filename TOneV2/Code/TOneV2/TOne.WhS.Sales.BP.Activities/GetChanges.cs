using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetChanges : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public OutArgument<Changes> Changes { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
            int ownerId = this.OwnerId.Get(context);

            var ratePlanDraftManager = new RatePlanDraftManager();
            Changes changes = ratePlanDraftManager.GetDraft(ownerType, ownerId);

            this.Changes.Set(context, changes);
        }
    }
}
