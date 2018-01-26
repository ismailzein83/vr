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
    public class ApplyChangedRatesToDBInput
    {
        public IEnumerable<ChangedRate> ChangedRates { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedRatesToDBOutput
    {

    }

    public class ApplyChangedRatesToDB : BaseAsyncActivity<ApplyChangedRatesToDBInput, ApplyChangedRatesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        #endregion

        protected override ApplyChangedRatesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedRatesToDBInput()
            {
                ChangedRates = this.ChangedRates.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedRatesToDBOutput DoWorkWithResult(ApplyChangedRatesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ChangedRate> changedRates = inputArgument.ChangedRates;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            IChangedSaleRateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IChangedSaleRateDataManager>();
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyChangedRatesToDB(changedRates);

            return new ApplyChangedRatesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedRatesToDBOutput result)
        {

        }
    }
}
