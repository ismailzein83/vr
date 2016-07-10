using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ApplySaleZoneRoutingProductPreviewsToDB : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductPreview>> SaleZoneRoutingProductPreviews { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZoneRoutingProductPreview> saleZoneRoutingProductPreviews = SaleZoneRoutingProductPreviews.Get(context);

            if (saleZoneRoutingProductPreviews == null)
                return;

            var dataManager = SalesDataManagerFactory.GetDataManager<ISaleZoneRoutingProductPreviewDataManager>();
            dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            dataManager.ApplySaleZoneRoutingProductPreviewsToDB(saleZoneRoutingProductPreviews);
        }
    }
}
