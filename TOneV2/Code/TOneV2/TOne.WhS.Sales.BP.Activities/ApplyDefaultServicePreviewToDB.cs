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

    public class ApplyDefaultServicePreviewToDBInput
    {
        public DefaultServicePreview DefaultServicePreview { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyDefaultServicePreviewToDBOutput
    {

    }

    #endregion

    public class ApplyDefaultServicePreviewToDB : BaseAsyncActivity<ApplyDefaultServicePreviewToDBInput, ApplyDefaultServicePreviewToDBOutput>
    {
        #region Input Arguments

        public InArgument<DefaultServicePreview> DefaultServicePreview { get; set; }

        #endregion

        protected override ApplyDefaultServicePreviewToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyDefaultServicePreviewToDBInput()
            {
                DefaultServicePreview = this.DefaultServicePreview.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyDefaultServicePreviewToDBOutput DoWorkWithResult(ApplyDefaultServicePreviewToDBInput inputArgument, AsyncActivityHandle handle)
        {
            DefaultServicePreview defaultServicePreview = inputArgument.DefaultServicePreview;

            if (defaultServicePreview != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IDefaultServicePreviewDataManager>();
                long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.Insert(defaultServicePreview);
            }

            return new ApplyDefaultServicePreviewToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyDefaultServicePreviewToDBOutput result)
        {

        }
    }
}
