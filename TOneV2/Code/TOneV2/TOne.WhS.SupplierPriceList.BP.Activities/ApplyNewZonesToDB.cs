using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class ApplyNewZonesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyNewZonesToDB : DependentAsyncActivity<ApplyNewZonesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewZonesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSupplierZoneDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierZoneDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZones) =>
                    {
                        dataManager.ApplyNewZonesToDB(preparedZones);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewZonesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewZonesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
