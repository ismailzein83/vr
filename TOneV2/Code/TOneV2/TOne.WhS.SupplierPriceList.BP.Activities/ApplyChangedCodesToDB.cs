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
    public class ApplyChangeCodesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyChangedCodesToDB : DependentAsyncActivity<ApplyChangeCodesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangeCodesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierCodeDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodes) =>
                    {
                        dataManager.ApplyChangedCodesToDB(preparedCodes);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangeCodesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangeCodesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
