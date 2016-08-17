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
        public BaseQueue<TargetBEBatch> TargerBEsToInsert { get; set; }
        public BaseQueue<TargetBEBatch> TargerBEsToUpdate { get; set; }
    }
    public sealed class UpdateTargetBEs : DependentAsyncActivity<UpdateTargetBEsInput>
    {
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }
        public InOutArgument<BaseQueue<TargetBEBatch>> TargerBEsToInsert { get; set; }
        public InOutArgument<BaseQueue<TargetBEBatch>> TargerBEsToUpdate { get; set; }

        protected override void DoWork(UpdateTargetBEsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.TargerBEsToInsert.TryDequeue((targetsToInsert) =>
                    {
                        TargetSynchronizerInsertContext context = new TargetSynchronizerInsertContext();
                        context.TargetBE = targetsToInsert.TargetBEs.ToList();
                        inputArgument.TargetBESynchronizer.InsertBEs(context);
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Targets Inserted", context.TargetBE.Count);
                    });
                    hasItem = inputArgument.TargerBEsToUpdate.TryDequeue((targetsToUpdate) =>
                    {
                        TargetSynchronizerUpdateContext context = new TargetSynchronizerUpdateContext();
                        context.TargetBE = targetsToUpdate.TargetBEs.ToList();
                        inputArgument.TargetBESynchronizer.UpdateBEs(context);
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Targets Updated", context.TargetBE.Count);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

        }
        protected override UpdateTargetBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateTargetBEsInput
            {
                TargerBEsToInsert = this.TargerBEsToInsert.Get(context),
                TargerBEsToUpdate = this.TargerBEsToUpdate.Get(context),
                TargetBESynchronizer = this.TargetBESynchronizer.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (TargerBEsToInsert.Get(context) == null)
                TargerBEsToInsert.Set(context, new MemoryQueue<TargetBEBatch>());
            if (TargerBEsToUpdate.Get(context) == null)
                TargerBEsToUpdate.Set(context, new MemoryQueue<TargetBEBatch>());
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
