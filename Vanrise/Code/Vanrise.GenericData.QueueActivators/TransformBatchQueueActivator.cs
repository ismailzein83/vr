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
        DataTransformationDefinitionManager dataTransformationDefinitionManager = new DataTransformationDefinitionManager();
        DataTransformer dataTransformer = new DataTransformer();

        DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

        public int DataTransformationDefinitionId { get; set; }

        public string SourceRecordName { get; set; }

        public List<TransformBatchNextStageRecord> NextStagesRecords { get; set; }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
            
        }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;

            var recordTypeId = (context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType).DataRecordTypeId;
            Type recordTypeRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(recordTypeId);

            Type genericListType = typeof(List<>);
            Type recordListType = genericListType.MakeGenericType(recordTypeRuntimeType);

            dynamic batchRecords = ProtoBufSerializer.Deserialize(dataRecordBatch.SerializedRecordsList, recordListType);

            var transformationOutput = dataTransformer.ExecuteDataTransformation(this.DataTransformationDefinitionId,
                (transformationContext) =>
                {
                    transformationContext.SetRecordValue(this.SourceRecordName, batchRecords);
                });
            foreach (var nextStageRecord in this.NextStagesRecords)
            {
                var transformedList = transformationOutput.GetRecordValue(nextStageRecord.RecordName);
                foreach (var stage in nextStageRecord.NextStages)
                {
                    DataRecordBatch transformedBatch = new DataRecordBatch
                    {
                        SerializedRecordsList = ProtoBufSerializer.Serialize(transformedList)
                    };
                    context.OutputItems.Add(stage, transformedBatch);
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

    public class DataRecordBatchQueueItemType : Vanrise.Queueing.Entities.QueueExecutionFlowStageItemType
    {
        public int DataRecordTypeId { get; set; }

        public override Type GetQueueItemType()
        {
            return typeof(DataRecordBatch);
        }
    }


    public class DataRecordBatch : Vanrise.Queueing.Entities.PersistentQueueItem
    {
        public byte[] SerializedRecordsList { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Data Record Batch");
        }

        public override byte[] Serialize()
        {
            return this.SerializedRecordsList;
        }

        public override T Deserialize<T>(byte[] serializedBytes)
        {
            return new DataRecordBatch
            {
                SerializedRecordsList = serializedBytes
            } as T;
        }
    }
}
