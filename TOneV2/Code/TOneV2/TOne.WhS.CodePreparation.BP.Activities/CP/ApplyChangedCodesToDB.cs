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
    public class ApplyChangedCodesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyChangedCodesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyChangedCodesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangedCodesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            long processInstanceID = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            IChangedSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleCodeDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        dataManager.ApplyChangedCodesToDB(preparedCodeMatch, processInstanceID);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedCodesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedCodesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
