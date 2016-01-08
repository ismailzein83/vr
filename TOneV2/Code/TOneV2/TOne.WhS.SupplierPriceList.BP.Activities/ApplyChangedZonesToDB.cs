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
    public class ApplyChangeZonesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyChangedZonesToDB : DependentAsyncActivity<ApplyChangeZonesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangeZonesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSupplierZoneDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierZoneDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZones) =>
                    {
                        dataManager.ApplyChangedZonesToDB(preparedZones);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangeZonesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangeZonesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
