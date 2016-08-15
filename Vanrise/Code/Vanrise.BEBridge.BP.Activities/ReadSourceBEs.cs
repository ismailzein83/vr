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

    public sealed class ReadSourceBEs : CodeActivity
    {
        [RequiredArgument]
        public InArgument<SourceBEReader> SourceReader { get; set; }
        [RequiredArgument]
        public InOutArgument<SourceBEBatch> SourceBatch { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Action<SourceBEBatch, SourceBEBatchRetrievedContext> onSourceBEBatchRetrieved = (sourceBEBatch, sourceRetrievedContext) =>
            {
                SourceBatch.Set(context, sourceBEBatch);
            };
            SourceBEReaderRetrieveUpdatedBEsContext sourceBEReaderContext = new SourceBEReaderRetrieveUpdatedBEsContext(onSourceBEBatchRetrieved);
            SourceReader.Get(context).RetrieveUpdatedBEs(sourceBEReaderContext);

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
