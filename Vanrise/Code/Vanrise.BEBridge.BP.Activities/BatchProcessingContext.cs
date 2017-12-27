using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Queueing;

namespace Vanrise.BEBridge.BP.Activities
{
    public class BatchProcessingContext
    {
        SourceBEBatch _sourceBEBatch;
        SourceBEReader _sourceBEReader;
        Queue<BaseQueue<BatchProcessingContext>> _qOutputQueues = new Queue<BaseQueue<BatchProcessingContext>>();
        bool _isComplete;

        List<ITargetBE> _targetBEsToInsert;
        List<ITargetBE> _targetBEsToUpdate;
        Guid _BEDefinitionId;

        internal BatchProcessingContext(SourceBEBatch sourceBEBatch, SourceBEReader sourceBEReader, List<BaseQueue<BatchProcessingContext>> outputQueues, Guid BEDefinitionId)
        {
            if (sourceBEBatch == null)
                throw new ArgumentNullException("sourceBEBatch");
            if (sourceBEReader == null)
                throw new ArgumentNullException("sourceBEReader");
            if (outputQueues == null || outputQueues.Count == 0)
                throw new ArgumentNullException("outputQueues");
            _sourceBEBatch = sourceBEBatch;
            _sourceBEReader = sourceBEReader;
            _BEDefinitionId = BEDefinitionId;
            foreach (var outputQueue in outputQueues)
            {
                _qOutputQueues.Enqueue(outputQueue);
            }
            var firstOutputQueue = _qOutputQueues.Dequeue();
            firstOutputQueue.Enqueue(this);
        }


        internal SourceBEBatch SourceBEBatch
        {
            get
            {
                return _sourceBEBatch;
            }
        }

        internal bool IsComplete
        {
            get
            {
                return _isComplete;
            }
        }

        internal void SetTargetBEs(List<ITargetBE> targetBEsToInsert, List<ITargetBE> targetBEsToUpdate)
        {
            if (targetBEsToInsert != null && targetBEsToInsert.Count > 0)
                _targetBEsToInsert = targetBEsToInsert;
            if (targetBEsToUpdate != null && targetBEsToUpdate.Count > 0)
                _targetBEsToUpdate = targetBEsToUpdate;
        }

        internal void SaveTargetBEs(Action<List<ITargetBE>> insertTargetBEs, Action<List<ITargetBE>> updateTargetBEs)
        {
            if (_targetBEsToInsert != null)
                insertTargetBEs(_targetBEsToInsert);
            if (_targetBEsToUpdate != null)
                updateTargetBEs(_targetBEsToUpdate);
            _targetBEsToInsert = null;
            _targetBEsToUpdate = null;
            CheckCompletion();
        }

        private void CheckCompletion()
        {
            if (_qOutputQueues.Count > 0)
            {
                var nextOutputQueue = _qOutputQueues.Dequeue();
                nextOutputQueue.Enqueue(this);
            }
            else
            {
                _sourceBEReader.SetBatchCompleted(new SourceBEReaderSetBatchImportedContext { Batch = _sourceBEBatch, BEReceiveDefinitionId = _BEDefinitionId });
                _isComplete = true;
            }
        }
    }

    public class SourceBEReaderSetBatchImportedContext : ISourceBEReaderSetBatchImportedContext
    {

        public SourceBEBatch Batch
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
}
