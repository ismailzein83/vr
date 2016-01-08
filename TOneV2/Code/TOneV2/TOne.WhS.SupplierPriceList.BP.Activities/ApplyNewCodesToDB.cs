using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Data;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ApplyNewCodesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyNewCodesToDB : DependentAsyncActivity<ApplyNewCodesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewCodesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSupplierCodeDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierCodeDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodes) =>
                    {
                        dataManager.ApplyNewCodesToDB(preparedCodes);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewCodesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewCodesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
