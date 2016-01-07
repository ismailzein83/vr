using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyNewCodesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyNewCodesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyNewCodesToDBInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewCodesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            long processInstanceID = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            INewSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleCodeDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodes) =>
                    {
                        dataManager.ApplyNewCodesToDB(preparedCodes, processInstanceID);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewCodesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewCodesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
