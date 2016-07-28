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
    public class ApplyNewRatesToDBInput
    {
        public IEnumerable<NewRate> NewRates { get; set; }
    }

    public class ApplyNewRatesToDBOutput
    {

    }

    public class ApplyNewRatesToDB : BaseAsyncActivity<ApplyNewRatesToDBInput, ApplyNewRatesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        #endregion

        protected override ApplyNewRatesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewRatesToDBInput()
            {
                NewRates = this.NewRates.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewRatesToDBOutput DoWorkWithResult(ApplyNewRatesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<NewRate> newRates = inputArgument.NewRates;

            INewSaleRateDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewSaleRateDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            dataManager.ApplyNewRatesToDB(newRates);

            return new ApplyNewRatesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewRatesToDBOutput result)
        {

        }
    }
}
