using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Data;

namespace TOne.WhS.Sales.BP.Activities
{
    public class DeleteRatePlanDraft : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            ratePlanDataManager.DeleteRatePlanDraft(ratePlanContext.OwnerType, ratePlanContext.OwnerId);
        }
    }
}
