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

    public class ApplyPreviewZonesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyPreviewZonesRatesToDB : DependentAsyncActivity<ApplyPreviewZonesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyPreviewZonesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZones) =>
                    {
                        dataManager.ApplyPreviewZonesToDB(preparedZones);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyPreviewZonesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPreviewZonesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
