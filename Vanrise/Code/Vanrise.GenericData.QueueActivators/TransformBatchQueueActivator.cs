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
    public class TransformBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
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
            if (this.NextStagesRecords != null)
            {
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
                        if (transformedBatch.GetRecordCount() > 0)
                            context.OutputItems.Add(stageName, transformedBatch);
                    }

                }
            }
        }

        public override void OnDisposed()
        {

        }

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                List<string> validStages = GetOutputStages(context.StageNames);
                do
                {
                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                    {
                        Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
                        if (genericDataRecordBatch == null)
                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                        var transformationOutput = _dataTransformer.ExecuteDataTransformation(this.DataTransformationDefinitionId,
                        (transformationContext) =>
                        {
                            transformationContext.SetRecordValue(this.SourceRecordName, genericDataRecordBatch.Records);
                        });

                        if (this.NextStagesRecords != null)
                        {
                            foreach (var nextStageRecord in this.NextStagesRecords)
                            {
                                var transformedList = transformationOutput.GetRecordValue(nextStageRecord.RecordName);
                                Reprocess.Entities.GenericDataRecordBatch item = new Reprocess.Entities.GenericDataRecordBatch() { Records = transformedList };
                                foreach (var stageName in nextStageRecord.NextStages)
                                {
                                    if (validStages != null && validStages.Contains(stageName))
                                        context.EnqueueBatch(stageName, item);
                                }
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {

        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            if (NextStagesRecords == null)
                return null;

            if (stageNames == null)
                return null;

            IEnumerable<string> stages = NextStagesRecords.SelectMany(itm => itm.NextStages).Distinct();
            Func<string, bool> filterExpression = (itemObject) => stageNames.Contains(itemObject);

            IEnumerable<string> filteredStages = stages.FindAllRecords(filterExpression);
            return filteredStages != null ? filteredStages.ToList() : null;
        }

        public Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }


        public List<Reprocess.Entities.StageRecordInfo> GetStageRecordInfo(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            return null;
        }
    }

    public class TransformBatchNextStageRecord
    {
        public string RecordName { get; set; }

        public List<string> NextStages { get; set; }
    }
}
