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
    #region Public Classes

    public class ApplyDefaultRoutingProductPreviewToDBInput
    {
        public DefaultRoutingProductPreview DefaultRoutingProductPreview { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyDefaultRoutingProductPreviewToDBOutput
    {

    }

    #endregion

    public class ApplyDefaultRoutingProductPreviewToDB : BaseAsyncActivity<ApplyDefaultRoutingProductPreviewToDBInput, ApplyDefaultRoutingProductPreviewToDBOutput>
    {
        #region Input Arguments

        public InArgument<DefaultRoutingProductPreview> DefaultRoutingProductPreview { get; set; }

        #endregion

        protected override ApplyDefaultRoutingProductPreviewToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyDefaultRoutingProductPreviewToDBInput()
            {
                DefaultRoutingProductPreview = this.DefaultRoutingProductPreview.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyDefaultRoutingProductPreviewToDBOutput DoWorkWithResult(ApplyDefaultRoutingProductPreviewToDBInput inputArgument, AsyncActivityHandle handle)
        {
            DefaultRoutingProductPreview defaultRoutingProductPreview = inputArgument.DefaultRoutingProductPreview;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;

            if (defaultRoutingProductPreview != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IDefaultRoutingProductPreviewDataManager>();
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.Insert(defaultRoutingProductPreview);
            }

            return new ApplyDefaultRoutingProductPreviewToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyDefaultRoutingProductPreviewToDBOutput result)
        {

        }
    }
}
