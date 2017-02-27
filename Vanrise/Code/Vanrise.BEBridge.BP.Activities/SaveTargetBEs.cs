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
        public TargetBESynchronizerInitializeContext SynchronizerInitializeContext { get; set; }
        public BaseQueue<BatchProcessingContext> BatchProcessingContextQueue { get; set; }
    }
    public sealed class SaveTargetBEs : DependentAsyncActivity<UpdateTargetBEsInput>
    {
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }
        [RequiredArgument]
        public InArgument<TargetBESynchronizerInitializeContext> SynchronizerInitializeContext { get; set; }
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
                        TargetSynchronizerInsertContext insertContext = new TargetSynchronizerInsertContext(handle);
                        insertContext.InitializationData = inputArgument.SynchronizerInitializeContext.InitializationData;
                        TargetSynchronizerUpdateContext updateContext = new TargetSynchronizerUpdateContext(handle);
                        batchProcessingContext.SaveTargetBEs((targetsToInsert) =>
                        {
                            insertContext.TargetBE = targetsToInsert;

                            inputArgument.TargetBESynchronizer.InsertBEs(insertContext);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} {1} Inserted.", insertContext.TargetBE.Count, inputArgument.TargetBESynchronizer.Name);
                        }, (targetsToUpdate) =>
                        {
                            updateContext.TargetBE = targetsToUpdate;
                            inputArgument.TargetBESynchronizer.UpdateBEs(updateContext);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} {1} Updated.", updateContext.TargetBE.Count, inputArgument.TargetBESynchronizer.Name);
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
                BatchProcessingContextQueue = this.BatchProcessingContextQueue.Get(context),
                SynchronizerInitializeContext = this.SynchronizerInitializeContext.Get(context)
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
            AsyncActivityHandle _handle;

            public TargetSynchronizerInsertContext(AsyncActivityHandle handle)
            {
                if (handle == null)
                    throw new ArgumentNullException("handle");
                _handle = handle;
            }

            public List<ITargetBE> TargetBE
            {
                get;
                set;
            }


            public object InitializationData
            {
                get;
                set;
            }


            public void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);
            }

            public void WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }


            public void WriteHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteHandledException(ex);
            }

            public void WriteBusinessHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteBusinessHandledException(ex);
            }
        }
        private class TargetSynchronizerUpdateContext : ITargetBESynchronizerUpdateBEsContext
        { 
            AsyncActivityHandle _handle;

            public TargetSynchronizerUpdateContext(AsyncActivityHandle handle)
            {
                if (handle == null)
                    throw new ArgumentNullException("handle");
                _handle = handle;
            }

            public List<ITargetBE> TargetBE
            {
                get;
                set;
            }


            public void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);
            }

            public void WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }


            public void WriteHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteHandledException(ex);
            }

            public void WriteBusinessHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteBusinessHandledException(ex);
            }
        }

        #endregion
    }
}
