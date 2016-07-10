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
    public class ApplyRatePlanPreviewSummaryToDB : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<RatePlanPreviewSummary> RatePlanPreviewSummary { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            RatePlanPreviewSummary summary = RatePlanPreviewSummary.Get(context);

            if (summary == null)
                throw new NullReferenceException("summary");

            var dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanPreviewSummaryDataManager>();
            dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            dataManager.ApplyRatePlanPreviewSummaryToDB(summary);
        }
    }
}
