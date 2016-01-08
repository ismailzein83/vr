using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ApplyNewRatesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyNewRatesToDB : DependentAsyncActivity<ApplyNewRatesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierRateDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedRates) =>
                    {
                        dataManager.ApplyNewRatesToDB(preparedRates);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewRatesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
