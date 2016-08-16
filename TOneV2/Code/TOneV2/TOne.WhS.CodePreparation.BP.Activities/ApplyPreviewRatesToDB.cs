using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.CodePreparation.Data;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class ApplyPreviewRatesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyPreviewRatesToDB : DependentAsyncActivity<ApplyPreviewRatesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyPreviewRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISaleRatePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleRatePreviewDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedRates) =>
                    {
                        dataManager.ApplyPreviewRatesToDB(preparedRates);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyPreviewRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPreviewRatesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
