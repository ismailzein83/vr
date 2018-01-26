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
    public class ApplySaleZoneRoutingProductPreviewsToDBInput
    {
        public IEnumerable<SaleZoneRoutingProductPreview> SaleZoneRoutingProductPreviews { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplySaleZoneRoutingProductPreviewsToDBOutput
    {

    }

    public class ApplySaleZoneRoutingProductPreviewsToDB : BaseAsyncActivity<ApplySaleZoneRoutingProductPreviewsToDBInput, ApplySaleZoneRoutingProductPreviewsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductPreview>> SaleZoneRoutingProductPreviews { get; set; }

        #endregion

        protected override ApplySaleZoneRoutingProductPreviewsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplySaleZoneRoutingProductPreviewsToDBInput()
            {
                SaleZoneRoutingProductPreviews = this.SaleZoneRoutingProductPreviews.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplySaleZoneRoutingProductPreviewsToDBOutput DoWorkWithResult(ApplySaleZoneRoutingProductPreviewsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZoneRoutingProductPreview> saleZoneRoutingProductPreviews = inputArgument.SaleZoneRoutingProductPreviews;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;

            if (saleZoneRoutingProductPreviews != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<ISaleZoneRoutingProductPreviewDataManager>();
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.ApplySaleZoneRoutingProductPreviewsToDB(saleZoneRoutingProductPreviews);
            }

            return new ApplySaleZoneRoutingProductPreviewsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplySaleZoneRoutingProductPreviewsToDBOutput result)
        {

        }
    }
}
