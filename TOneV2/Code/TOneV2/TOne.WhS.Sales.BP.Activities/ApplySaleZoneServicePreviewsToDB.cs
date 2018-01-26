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
    #region Classes

    public class ApplySaleZoneServicePreviewsToDBInput
    {
        public IEnumerable<SaleZoneServicePreview> SaleZoneServicePreviews { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplySaleZoneServicePreviewsToDBOutput
    {

    }

    #endregion

    public class ApplySaleZoneServicePreviewsToDB : BaseAsyncActivity<ApplySaleZoneServicePreviewsToDBInput, ApplySaleZoneServicePreviewsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServicePreview>> SaleZoneServicePreviews { get; set; }

        #endregion

        protected override ApplySaleZoneServicePreviewsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplySaleZoneServicePreviewsToDBInput()
            {
                SaleZoneServicePreviews = this.SaleZoneServicePreviews.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplySaleZoneServicePreviewsToDBOutput DoWorkWithResult(ApplySaleZoneServicePreviewsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZoneServicePreview> saleZoneServicePreviews = inputArgument.SaleZoneServicePreviews;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;

            if (saleZoneServicePreviews != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<ISaleZoneServicePreviewDataManager>();
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.ApplySaleZoneServicePreviewsToDB(saleZoneServicePreviews);
            }

            return new ApplySaleZoneServicePreviewsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplySaleZoneServicePreviewsToDBOutput result)
        {

        }
    }
}
