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
    public class ApplyNewZonesServicesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyNewZonesServicesToDB : DependentAsyncActivity<ApplyNewZonesServicesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewZonesServicesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSupplierZonesServicesDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierZonesServicesDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZonesServices) =>
                    {
                        dataManager.ApplyNewZonesServicesToDB(preparedZonesServices);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewZonesServicesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewZonesServicesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
