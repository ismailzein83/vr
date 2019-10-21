using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCodeMatchesForApplyInput
    {
        public int RoutingDatabaseId { get; set; }
        public bool ShouldApplyCodeZoneMatch { get; set; }
        public BaseQueue<CodeMatchesBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareCodeMatchesForApply : DependentAsyncActivity<PrepareCodeMatchesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
		public InArgument<bool> ShouldApplyCodeZoneMatch { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareCodeMatchesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager codeMatchesDataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
			codeMatchesDataManager.ShouldApplyCodeZoneMatch = inputArgument.ShouldApplyCodeZoneMatch;
            codeMatchesDataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            PrepareDataForDBApply(previousActivityStatus, handle, codeMatchesDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, CodeMatchesBatch => CodeMatchesBatch.CodeMatches);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Code Matches For Apply is done", null);
        }

        protected override PrepareCodeMatchesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeMatchesForApplyInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
				ShouldApplyCodeZoneMatch = this.ShouldApplyCodeZoneMatch.Get(context),
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