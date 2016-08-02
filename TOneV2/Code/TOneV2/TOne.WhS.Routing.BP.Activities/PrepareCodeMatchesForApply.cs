using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCodeMatchesForApplyInput
    {
        public BaseQueue<CodeMatchesBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }


    public sealed class PrepareCodeMatchesForApply : DependentAsyncActivity<PrepareCodeMatchesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareCodeMatchesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager codeMatchesDataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, codeMatchesDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, CodeMatchesBatch => CodeMatchesBatch.CodeMatches);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Code Matches For Apply is done", null);
        }

        protected override PrepareCodeMatchesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeMatchesForApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
