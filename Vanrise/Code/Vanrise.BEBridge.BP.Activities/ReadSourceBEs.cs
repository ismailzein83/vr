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
    }

    public sealed class ReadSourceBEs : DependentAsyncActivity<ReadSourceBEsInput>
    {
        [RequiredArgument]
        public InArgument<SourceBEReader> SourceReader { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<SourceBatches>> SourceBatches { get; set; }

        protected override void DoWork(ReadSourceBEsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBEBatchRetrieved = (sourceBEBatch, sourceRetrievedContext) =>
            {
                inputArgument.SourceBatches.Enqueue(new SourceBatches() { SorceBEBatches = new List<SourceBEBatch> { sourceBEBatch } });
            };
            SourceBEReaderRetrieveUpdatedBEsContext sourceBEReaderContext = new SourceBEReaderRetrieveUpdatedBEsContext(onSourceBEBatchRetrieved);
            inputArgument.SourceReader.RetrieveUpdatedBEs(sourceBEReaderContext);
        }

        protected override ReadSourceBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ReadSourceBEsInput
            {
                SourceBatches = this.SourceBatches.Get(context),
                SourceReader = this.SourceReader.Get(context)
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
