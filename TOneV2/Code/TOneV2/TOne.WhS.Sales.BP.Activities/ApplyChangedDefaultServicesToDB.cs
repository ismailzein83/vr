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

    public class ApplyChangedDefaultServicesToDBInput
    {
        public IEnumerable<ChangedDefaultService> ChangedDefaultServices { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedDefaultServicesToDBOutput
    {

    }
    
    #endregion

    public class ApplyChangedDefaultServicesToDB : BaseAsyncActivity<ApplyChangedDefaultServicesToDBInput, ApplyChangedDefaultServicesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedDefaultService>> ChangedDefaultServices { get; set; }

        #endregion

        protected override ApplyChangedDefaultServicesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedDefaultServicesToDBInput()
            {
                ChangedDefaultServices = this.ChangedDefaultServices.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedDefaultServicesToDBOutput DoWorkWithResult(ApplyChangedDefaultServicesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ChangedDefaultService> changedDefaultServices = inputArgument.ChangedDefaultServices;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            var dataManager = SalesDataManagerFactory.GetDataManager<IChangedDefaultServiceDataManager>();
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyChangedDefaultServicesToDB(changedDefaultServices);

            return new ApplyChangedDefaultServicesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedDefaultServicesToDBOutput result)
        {

        }
    }
}
