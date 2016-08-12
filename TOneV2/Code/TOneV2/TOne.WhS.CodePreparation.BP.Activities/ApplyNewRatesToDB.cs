using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyNewRatesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyNewRatesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyNewRatesToDBInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSaleRateDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleRateDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedRates) =>
                    {
                        dataManager.ApplyNewRatesToDB(preparedRates);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewRatesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
