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
    public class ApplyChangedZonesServicesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyChangedZonesServicesToDB : DependentAsyncActivity<ApplyChangedZonesServicesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }


        protected override void DoWork(ApplyChangedZonesServicesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSupplierZonesServicesDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierZonesServicesDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZonesServices) =>
                    {
                        dataManager.ApplyChangedZonesServicesToDB(preparedZonesServices);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedZonesServicesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedZonesServicesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
