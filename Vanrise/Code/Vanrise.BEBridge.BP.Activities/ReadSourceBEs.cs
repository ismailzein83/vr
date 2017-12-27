using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace Vanrise.BEBridge.BP.Activities
{

    public class ReadSourceBEsInput
    {
        public SourceBEReader SourceReader { get; set; }
        public BaseQueue<SourceBatches> SourceBatches { get; set; }
        public List<BaseQueue<BatchProcessingContext>> OutputQueues { get; set; }
        public Guid BeDefinitionId { get; set; }
    }

    public class ReadSourceBEsOutput
    {
        public object ReaderState { get; set; }
    }
    public sealed class ReadSourceBEs : DependentAsyncActivity<ReadSourceBEsInput, ReadSourceBEsOutput>
    {
        [RequiredArgument]
        public InArgument<SourceBEReader> SourceReader { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<SourceBatches>> SourceBatches { get; set; }
        [RequiredArgument]
        public InArgument<List<BaseQueue<BatchProcessingContext>>> OutputQueues { get; set; }
        [RequiredArgument]
        public InArgument<Guid> BeDefinitionId { get; set; }
        [RequiredArgument]
        public OutArgument<object> ReaderState { get; set; }
        protected override ReadSourceBEsOutput DoWorkWithResult(ReadSourceBEsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ProcessManager processManager = new ProcessManager();

            List<BatchProcessingContext> pendingBatchProcessings = new List<BatchProcessingContext>();
            Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBEBatchRetrieved = (sourceBEBatch, sourceRetrievedContext) =>
            {
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Source BE file {0} read.", sourceBEBatch.BatchName);
                var batchProcessContext = new BatchProcessingContext(sourceBEBatch, inputArgument.SourceReader, inputArgument.OutputQueues,inputArgument.BeDefinitionId);
                lock (pendingBatchProcessings)
                {
                    pendingBatchProcessings.Add(batchProcessContext);
                }
            };

            #region Source Reader

            SourceBEReaderRetrieveUpdatedBEsContext sourceBEReaderContext =
               new SourceBEReaderRetrieveUpdatedBEsContext(onSourceBEBatchRetrieved)
               {
                   ReaderState = processManager.GetDefinitionObjectState<object>(handle.SharedInstanceData.InstanceInfo.DefinitionID, inputArgument.BeDefinitionId.ToString())
               };
            sourceBEReaderContext.BEReceiveDefinitionId = inputArgument.BeDefinitionId;
            inputArgument.SourceReader.RetrieveUpdatedBEs(sourceBEReaderContext);

            #endregion

            #region taskCheckPendingBatches thread

            System.Threading.Tasks.Task taskCheckPendingBatches = new System.Threading.Tasks.Task(() =>
            {
                while (pendingBatchProcessings.Count > 0)
                {
                    HashSet<BatchProcessingContext> completedBatches = new HashSet<BatchProcessingContext>(pendingBatchProcessings.Where(batchProcessing => batchProcessing.IsComplete));
                    if (completedBatches.Count > 0)
                    {
                        lock (pendingBatchProcessings)
                        {
                            pendingBatchProcessings.RemoveAll(batchProcessing => completedBatches.Contains(batchProcessing));
                        }
                    }
                    if (pendingBatchProcessings.Count > 0)
                        System.Threading.Thread.Sleep(250);
                }
                taskCheckPendingBatches = null;
            });
            taskCheckPendingBatches.Start();

            #endregion

            while (taskCheckPendingBatches != null)//wait all pending batches
            {
                System.Threading.Thread.Sleep(250);
            }

            return new ReadSourceBEsOutput
            {
                ReaderState = sourceBEReaderContext.ReaderState
            };
        }
        protected override ReadSourceBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ReadSourceBEsInput
            {
                SourceBatches = this.SourceBatches.Get(context),
                SourceReader = this.SourceReader.Get(context),
                OutputQueues = this.OutputQueues.Get(context),
                BeDefinitionId = this.BeDefinitionId.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.SourceBatches.Get(context) == null)
                this.SourceBatches.Set(context, new MemoryQueue<SourceBatches>());
            base.OnBeforeExecute(context, handle);
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, ReadSourceBEsOutput result)
        {
            this.ReaderState.Set(context, result.ReaderState);
        }

        #region Context Implementations

        private class SourceBEReaderRetrieveUpdatedBEsContext : ISourceBEReaderRetrieveUpdatedBEsContext
        {

            Action<SourceBEBatch, SourceBEBatchRetrievedContext> _onSourceBEBatchRetrieved;

            public SourceBEReaderRetrieveUpdatedBEsContext(Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBeBatchRetrieved)
            {
                _onSourceBEBatchRetrieved = onSourceBeBatchRetrieved;
            }

            public void OnSourceBEBatchRetrieved(SourceBEBatch sourceBEs, SourceBEBatchRetrievedContext context)
            {
                _onSourceBEBatchRetrieved(sourceBEs, context);
            }

            public object ReaderState
            {
                get;
                set;
            }

            public Guid BEReceiveDefinitionId
            {
                get;
                set;
            }
        }

        #endregion
    }

}
