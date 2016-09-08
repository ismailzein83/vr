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

    public class ApplyPreviewZonesServicesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyPreviewZonesServicesToDB : DependentAsyncActivity<ApplyPreviewZonesServicesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyPreviewZonesServicesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierZoneServicePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZoneServicePreviewDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZonesServices) =>
                    {
                        dataManager.ApplyPreviewZonesServicesToDB(preparedZonesServices);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyPreviewZonesServicesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPreviewZonesServicesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
