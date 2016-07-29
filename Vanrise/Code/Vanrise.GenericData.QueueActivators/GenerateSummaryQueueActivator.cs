using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class GenerateSummaryQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public int SummaryTransformationDefinitionId { get; set; }

        public string NextStageName { get; set; }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            

            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
            var summaryRecordTypeId = transformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId;
            var recortTypeManager = new DataRecordTypeManager();
            var summaryRecordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(summaryRecordTypeId);
            if (summaryRecordRuntimeType == null)
                throw new NullReferenceException(String.Format("summaryRecordRuntimeType {0}", recordTypeId));

            var summaryBatches = transformationManager.ConvertRawItemsToBatches(batchRecords, () => new GenericSummaryRecordBatch());
            if (summaryBatches != null)
            {
                foreach(var b in summaryBatches)
                {
                    b.SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId;
                    b.DescriptionTemplate = queueItemType.BatchDescription;
                    context.OutputItems.Add(this.NextStageName, b);
                }
            }
        }

        void Reprocess.Entities.IReprocessStageActivator.ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
            var allSummaryBatches = new Dictionary<DateTime, Dictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>>();
            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                    {
                        Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
                        if (genericDataRecordBatch == null)
                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                        var summaryRecordBatches = transformationManager.ConvertRawItemsToBatches(genericDataRecordBatch.Records, () => new GenericSummaryRecordBatch());
                        if (summaryRecordBatches != null)
                        {
                            foreach (var summaryBatch in summaryRecordBatches)
                            {
                                Dictionary<string, Vanrise.Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>> matchBatch;
                                if (allSummaryBatches.TryGetValue(summaryBatch.BatchStart, out matchBatch))
                                {
                                    matchBatch = new Dictionary<string, Common.Business.SummaryTransformation.SummaryItemInProcess<GenericSummaryItem>>();
                                    allSummaryBatches.Add(summaryBatch.BatchStart, matchBatch);
                                }
                                transformationManager.UpdateExistingFromNew(matchBatch, summaryBatch);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });

            //Store Summary Batches for finalization step
            foreach (var summaryBatchEntry in allSummaryBatches)
            {
                var genericSummaryBatch = new GenericSummaryRecordBatch
                {
                    BatchStart = summaryBatchEntry.Key,
                    Items = summaryBatchEntry.Value.Values.Select(summaryItemInProcess => summaryItemInProcess.SummaryItem).ToList(),
                    SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId
                };
                byte[] serializedBatch = genericSummaryBatch.Serialize();
            }
        }

        void Reprocess.Entities.IReprocessStageActivator.FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            throw new NotImplementedException();
        }

        List<string> Reprocess.Entities.IReprocessStageActivator.GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> Reprocess.Entities.IReprocessStageActivator.GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }
    }
}
