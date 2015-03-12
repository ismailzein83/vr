using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using System.Collections.Concurrent;
using TOne.Entities;
using Vanrise.BusinessProcess;
using System.Threading;
using TOne.LCR.Data;
using Vanrise.Queueing;
using TOne.Business;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes
    public class PrepareCodeMatchesForDBApplyInput
    {
        public BaseQueue<List<CodeMatch>> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareCodeMatchesForDBApply : DependentAsyncActivity<PrepareCodeMatchesForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<List<CodeMatch>>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareCodeMatchesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeMatchesForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(PrepareCodeMatchesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int bcpBatchSize = ConfigParameterManager.Current.GetBCPBatchSize();
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            List<CodeMatch> codeMatchesBatch = new List<CodeMatch>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (codeMatches) =>
                        {
                            codeMatchesBatch.AddRange(codeMatches);
                            if (codeMatchesBatch.Count >= bcpBatchSize)
                            {
                                Object preparedCodeMatches = dataManager.PrepareCodeMatchesForDBApply(codeMatchesBatch);
                                inputArgument.OutputQueue.Enqueue(preparedCodeMatches);
                                codeMatchesBatch = new List<CodeMatch>();
                            }
                        });
                }
                while (!ShouldStop(handle) && hasItem);

            });
            if (codeMatchesBatch.Count > 0)
            {
                Object preparedCodeMatches = dataManager.PrepareCodeMatchesForDBApply(codeMatchesBatch);
                inputArgument.OutputQueue.Enqueue(preparedCodeMatches);
            }
        }
    }
}