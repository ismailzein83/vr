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
    public class ApplyChangedRatesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyChangedRatesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyChangedRatesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangedRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            long processInstanceID = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            IChangedSaleRateDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleRateDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        dataManager.ApplyChangedRatesToDB(preparedCodeMatch, processInstanceID);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedRatesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
