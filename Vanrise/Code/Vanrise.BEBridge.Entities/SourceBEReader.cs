using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public abstract class SourceBEReader
    {
        public int ConfigId { get; set; }

        public abstract void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context);
    }

    public interface ISourceBEReaderRetrieveUpdatedBEsContext
    {
        void OnSourceBEBatchRetrieved(SourceBEBatch sourceBEs, SourceBEBatchRetrievedContext context);
    }

    public class SourceBEBatchRetrievedContext
    {

    }
}
