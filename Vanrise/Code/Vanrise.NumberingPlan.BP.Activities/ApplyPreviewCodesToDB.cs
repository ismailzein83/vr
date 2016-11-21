using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.NumberingPlan.Data;

namespace Vanrise.NumberingPlan.BP.Activities
{

    public class ApplyPreviewCodesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyPreviewCodesToDB : DependentAsyncActivity<ApplyPreviewCodesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyPreviewCodesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISaleCodePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleCodePreviewDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodes) =>
                    {
                        dataManager.ApplyPreviewCodesToDB(preparedCodes);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyPreviewCodesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPreviewCodesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
