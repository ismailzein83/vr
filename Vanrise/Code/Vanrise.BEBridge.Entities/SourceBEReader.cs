using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public abstract class SourceBEReader
    {
        public Guid ConfigId { get; set; }

        public abstract void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context);

        public virtual void SetBatchCompleted(ISourceBEReaderSetBatchImportedContext context)
        {

        }
    }

    public interface ISourceBEReaderRetrieveUpdatedBEsContext
    {
        void OnSourceBEBatchRetrieved(SourceBEBatch sourceBEs, SourceBEBatchRetrievedContext context);

        object ReaderState { get; set; }

        Guid BEReceiveDefinitionId { get; set; }
    }

    public interface ISourceBEReaderSetBatchImportedContext
    {
        SourceBEBatch Batch { get; }
        Guid BEReceiveDefinitionId { get; set; }
    }

    public class SourceBEBatchRetrievedContext
    {

    }
}
