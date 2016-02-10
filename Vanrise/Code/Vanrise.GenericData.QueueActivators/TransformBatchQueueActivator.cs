using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation;

namespace Vanrise.GenericData.QueueActivators
{
    public class TransformBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator
    {
        DataTransformer _dataTransformer = new DataTransformer();

        public int DataTransformationDefinitionId { get; set; }

        public string SourceRecordName { get; set; }

        public List<TransformBatchNextStageRecord> NextStagesRecords { get; set; }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
            
        }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;            
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            var transformationOutput = _dataTransformer.ExecuteDataTransformation(this.DataTransformationDefinitionId,
                (transformationContext) =>
                {
                    transformationContext.SetRecordValue(this.SourceRecordName, batchRecords);
                });
            foreach (var nextStageRecord in this.NextStagesRecords)
            {
                var transformedList = transformationOutput.GetRecordValue(nextStageRecord.RecordName);
                foreach (var stageName in nextStageRecord.NextStages)
                {
                    var stage = context.GetStage(stageName);
                    var stageQueueItemType = stage.QueueItemType as DataRecordBatchQueueItemType;
                    if (stageQueueItemType == null)
                        throw new Exception(String.Format("stage '{0}' QueueItemType is not of type DataRecordBatchQueueItemType", stageName));
                    DataRecordBatch transformedBatch = DataRecordBatch.CreateBatchFromRecords(transformedList, stageQueueItemType.BatchDescription);
                    context.OutputItems.Add(stageName, transformedBatch);
                }
                
            }
        }

        public override void OnDisposed()
        {

        }
    }

    public class TransformBatchNextStageRecord
    {
        public string RecordName { get; set; }

        public List<string> NextStages { get; set; }
    }
}
