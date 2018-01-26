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
    public class ApplyRatePreviewsToDBInput
    {
        public IEnumerable<RatePreview> RatePreviews { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyRatePreviewsToDBOutput
    {

    }

    public class ApplyRatePreviewsToDB : BaseAsyncActivity<ApplyRatePreviewsToDBInput, ApplyRatePreviewsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<RatePreview>> RatePreviews { get; set; }

        #endregion

        protected override ApplyRatePreviewsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyRatePreviewsToDBInput()
            {
                RatePreviews = this.RatePreviews.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyRatePreviewsToDBOutput DoWorkWithResult(ApplyRatePreviewsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<RatePreview> ratePreviews = inputArgument.RatePreviews;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;

            if (ratePreviews != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IRatePreviewDataManager>();
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.ApplyRatePreviewsToDB(ratePreviews);
            }

            return new ApplyRatePreviewsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyRatePreviewsToDBOutput result)
        {

        }
    }
}
