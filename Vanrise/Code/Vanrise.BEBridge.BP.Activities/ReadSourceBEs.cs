using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.BEBridge.BP.Activities
{

    public class ReadSourceBEsInput
    {
        public SourceBEReader SourceReader { get; set; }

        public SourceBEBatch SourceBatch { get; set; }
    }

    public sealed class ReadSourceBEs : BaseAsyncActivity<ReadSourceBEsInput>
    {
        [RequiredArgument]
        public InArgument<SourceBEReader> SourceReader { get; set; }
        [RequiredArgument]
        public InOutArgument<SourceBEBatch> SourceBatch { get; set; }
        protected override void DoWork(ReadSourceBEsInput inputArgument, AsyncActivityHandle handle)
        {
            Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBEBatchRetrieved = (sourceBEBatch, sourceRetrievedContext) =>
            {
                inputArgument.SourceBatch = sourceBEBatch;
            };
            SourceBEReaderRetrieveUpdatedBEsContext context = new SourceBEReaderRetrieveUpdatedBEsContext(onSourceBEBatchRetrieved);
            inputArgument.SourceReader.RetrieveUpdatedBEs(context);
        }

        protected override ReadSourceBEsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReadSourceBEsInput
            {
                SourceBatch = this.SourceBatch.Get(context),
                SourceReader = this.SourceReader.Get(context)
            };
        }

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

    }


}
