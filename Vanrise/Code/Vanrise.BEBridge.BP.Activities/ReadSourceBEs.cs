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
    }

    public sealed class ReadSourceBEs : DependentAsyncActivity<ReadSourceBEsInput>
    {
        [RequiredArgument]
        public InArgument<SourceBEReader> SourceReader { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<SourceBatches>> SourceBatches { get; set; }

        public InArgument<List<BaseQueue<BatchProcessingContext>>> OutputQueues { get; set; }

        protected override void DoWork(ReadSourceBEsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            List<BatchProcessingContext> pendingBatchProcessings = new List<BatchProcessingContext>();
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

            Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBEBatchRetrieved = (sourceBEBatch, sourceRetrievedContext) =>
            {
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Source BE file {0} read.", sourceBEBatch.BatchName);
                var batchProcessContext = new BatchProcessingContext(sourceBEBatch, inputArgument.SourceReader, inputArgument.OutputQueues);
                lock (pendingBatchProcessings)
                {
                    pendingBatchProcessings.Add(batchProcessContext);
                }
            };
            SourceBEReaderRetrieveUpdatedBEsContext sourceBEReaderContext = new SourceBEReaderRetrieveUpdatedBEsContext(onSourceBEBatchRetrieved);
            inputArgument.SourceReader.RetrieveUpdatedBEs(sourceBEReaderContext);
            while (taskCheckPendingBatches != null)//wait all pending batches
            {
                System.Threading.Thread.Sleep(250);
            }
        }

        protected override ReadSourceBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ReadSourceBEsInput
            {
                SourceBatches = this.SourceBatches.Get(context),
                SourceReader = this.SourceReader.Get(context),
                OutputQueues = this.OutputQueues.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.SourceBatches.Get(context) == null)
                this.SourceBatches.Set(context, new MemoryQueue<SourceBatches>());
            base.OnBeforeExecute(context, handle);
        }

        #region Context Implementations

        private class SourceBEReaderRetrieveUpdatedBEsContext : ISourceBEReaderRetrieveUpdatedBEsContext
        {

            Action<SourceBEBatch, SourceBEBatchRetrievedContext> _onSourceBEBatchRetrieved;

            public SourceBEReaderRetrieveUpdatedBEsContext(Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBEBatchRetrieved)
            {
                _onSourceBEBatchRetrieved = onSourceBEBatchRetrieved;
            }

            public void OnSourceBEBatchRetrieved(SourceBEBatch sourceBEs, SourceBEBatchRetrievedContext context)
            {
                _onSourceBEBatchRetrieved(sourceBEs, context);
            }
        }

        #endregion

    }

}
