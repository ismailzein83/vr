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

    public class ApplyPreviewOtherRatesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyPreviewOtherRatesToDB : DependentAsyncActivity<ApplyPreviewOtherRatesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyPreviewOtherRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierOtherRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierOtherRatePreviewDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedOtherRates) =>
                    {
                        dataManager.ApplyPreviewOtherRatesToDB(preparedOtherRates);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyPreviewOtherRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPreviewOtherRatesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
