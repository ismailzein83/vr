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
    public class ApplyRatePlanPreviewSummaryToDBInput
    {
        public RatePlanPreviewSummary RatePlanPreviewSummary { get; set; }
    }

    public class ApplyRatePlanPreviewSummaryToDBOutput
    {

    }

    public class ApplyRatePlanPreviewSummaryToDB : BaseAsyncActivity<ApplyRatePlanPreviewSummaryToDBInput, ApplyRatePlanPreviewSummaryToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<RatePlanPreviewSummary> RatePlanPreviewSummary { get; set; }

        #endregion

        protected override ApplyRatePlanPreviewSummaryToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyRatePlanPreviewSummaryToDBInput()
            {
                RatePlanPreviewSummary = this.RatePlanPreviewSummary.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyRatePlanPreviewSummaryToDBOutput DoWorkWithResult(ApplyRatePlanPreviewSummaryToDBInput inputArgument, AsyncActivityHandle handle)
        {
            RatePlanPreviewSummary summary = inputArgument.RatePlanPreviewSummary;

            if (summary == null)
                throw new NullReferenceException("summary");

            var dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanPreviewSummaryDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            dataManager.ApplyRatePlanPreviewSummaryToDB(summary);

            return new ApplyRatePlanPreviewSummaryToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyRatePlanPreviewSummaryToDBOutput result)
        {

        }
    }
}
