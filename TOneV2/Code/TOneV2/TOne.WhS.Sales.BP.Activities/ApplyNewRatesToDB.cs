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
        public IEnumerable<NewRate> OwnerNewRates { get; set; }
        public IEnumerable<NewRate> NewRatesToFillGapsDueToClosingCountry { get; set; }
        public IEnumerable<NewRate> NewRatesToFillGapsDueToChangeSellingProductRates { get; set; }

        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyNewRatesToDBOutput
    {

    }

    public class ApplyNewRatesToDB : BaseAsyncActivity<ApplyNewRatesToDBInput, ApplyNewRatesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> OwnerNewRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRatesToFillGapsDueToClosingCountry { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRatesToFillGapsDueToChangeSellingProductRates { get; set; }

        #endregion

        protected override ApplyNewRatesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewRatesToDBInput()
            {
                OwnerNewRates = this.OwnerNewRates.Get(context),
                NewRatesToFillGapsDueToClosingCountry = this.NewRatesToFillGapsDueToClosingCountry.Get(context),
                NewRatesToFillGapsDueToChangeSellingProductRates = this.NewRatesToFillGapsDueToChangeSellingProductRates.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewRatesToDBOutput DoWorkWithResult(ApplyNewRatesToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<NewRate> ownerNewRates = inputArgument.OwnerNewRates;
            IEnumerable<NewRate> newRatesToFillGapsDueToClosingCountry = inputArgument.NewRatesToFillGapsDueToClosingCountry;
            IEnumerable<NewRate> newRatesToFillGapsDueToChangeSellingProductRates = inputArgument.NewRatesToFillGapsDueToChangeSellingProductRates;

            List<NewRate> newRates = new List<NewRate>();
            newRates.AddRange(ownerNewRates);
            newRates.AddRange(newRatesToFillGapsDueToClosingCountry);
            newRates.AddRange(newRatesToFillGapsDueToChangeSellingProductRates);

            INewSaleRateDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewSaleRateDataManager>();
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyNewRatesToDB(newRates);

            return new ApplyNewRatesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewRatesToDBOutput result)
        {

        }
    }
}
