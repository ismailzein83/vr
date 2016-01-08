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
    public class ApplyChangeRatesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyChangedRatesToDB : DependentAsyncActivity<ApplyChangeRatesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }


        protected override void DoWork(ApplyChangeRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierRateDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedRates) =>
                    {
                        dataManager.ApplyChangedRatesToDB(preparedRates);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangeRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangeRatesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
