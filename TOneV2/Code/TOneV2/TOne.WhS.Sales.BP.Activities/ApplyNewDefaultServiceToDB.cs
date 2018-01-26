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

    public class ApplyNewDefaultServiceToDBInput
    {
        public NewDefaultService NewDefaultService { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyNewDefaultServiceToDBOutput
    {

    }

    #endregion

    public class ApplyNewDefaultServiceToDB : BaseAsyncActivity<ApplyNewDefaultServiceToDBInput, ApplyNewDefaultServiceToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<NewDefaultService> NewDefaultService { get; set; }

        #endregion

        protected override ApplyNewDefaultServiceToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewDefaultServiceToDBInput()
            {
                NewDefaultService = this.NewDefaultService.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewDefaultServiceToDBOutput DoWorkWithResult(ApplyNewDefaultServiceToDBInput inputArgument, AsyncActivityHandle handle)
        {
            NewDefaultService newDefaultService = inputArgument.NewDefaultService;

            if (newDefaultService != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<INewDefaultServiceDataManager>();
                long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.Insert(newDefaultService);
            }

            return new ApplyNewDefaultServiceToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewDefaultServiceToDBOutput result)
        {

        }
    }
}
