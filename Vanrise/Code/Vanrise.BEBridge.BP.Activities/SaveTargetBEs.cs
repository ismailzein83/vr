using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace Vanrise.BEBridge.BP.Activities
{
    public class UpdateTargetBEsInput
    {
        public TargetBESynchronizer TargetBESynchronizer { get; set; }
        public BaseQueue<BatchProcessingContext> BatchProcessingContextQueue { get; set; }
    }
    public sealed class SaveTargetBEs : DependentAsyncActivity<UpdateTargetBEsInput>
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<BatchProcessingContext>> BatchProcessingContextQueue { get; set; }
        protected override void DoWork(UpdateTargetBEsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.BatchProcessingContextQueue.TryDequeue((batchProcessingContext) =>
                    {
                        TargetSynchronizerInsertContext insertContext = new TargetSynchronizerInsertContext();
                        TargetSynchronizerUpdateContext updateContext = new TargetSynchronizerUpdateContext();
                        batchProcessingContext.SaveTargetBEs((targetsToInsert) =>
                        {
                            insertContext.TargetBE = targetsToInsert;
                            inputArgument.TargetBESynchronizer.InsertBEs(insertContext);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Targets Inserted.", insertContext.TargetBE.Count);
                        }, (targetsToUpdate) =>
                        {
                            updateContext.TargetBE = targetsToUpdate;
                            inputArgument.TargetBESynchronizer.UpdateBEs(updateContext);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Targets Updated.", updateContext.TargetBE.Count);
                        });
                    });

                } while (!ShouldStop(handle) && hasItem);
            });
        }
        protected override UpdateTargetBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateTargetBEsInput
            {
                TargetBESynchronizer = this.TargetBESynchronizer.Get(context),
                BatchProcessingContextQueue = this.BatchProcessingContextQueue.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.BatchProcessingContextQueue.Get(context) == null)
                this.BatchProcessingContextQueue.Set(context, new MemoryQueue<BatchProcessingContext>());
            base.OnBeforeExecute(context, handle);
        }

        #region Context Implementation

        private class TargetSynchronizerInsertContext : ITargetBESynchronizerInsertBEsContext
        {
            public List<ITargetBE> TargetBE
            {
                get;
                set;
            }
        }
        private class TargetSynchronizerUpdateContext : ITargetBESynchronizerUpdateBEsContext
        {
            public List<ITargetBE> TargetBE
            {
                get;
                set;
            }
        }

        #endregion
    }
}
