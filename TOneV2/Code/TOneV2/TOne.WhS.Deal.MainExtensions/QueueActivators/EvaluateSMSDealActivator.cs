using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;
using Vanrise.GenericData.QueueActivators;

namespace TOne.WhS.Deal.MainExtensions.QueueActivators
{
    public class EvaluateSMSDealActivator : QueueActivator, IReprocessStageActivator
    {
        private List<int> cdrTypesToEvaluate = new List<int>() { 1, 4 };

        public List<string> MainOutputStages { get; set; }
        public List<string> PartialPricedOutputStages { get; set; }
        public List<string> BillingOutputStages { get; set; }
        public List<string> TrafficOutputStages { get; set; }


        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            List<dynamic> mainCDRs = new List<dynamic>();
            List<dynamic> partialPricedCDRs = new List<dynamic>();
            List<dynamic> billingRecords = new List<dynamic>(); //for billing stats and traffic stats
            List<dynamic> trafficRecords = new List<dynamic>(); //for traffic stats

            foreach (var record in batchRecords)
                this.ClassifyRecord(record, mainCDRs, partialPricedCDRs, billingRecords, trafficRecords);

            DataRecordBatch mainTransformedBatch = DataRecordBatch.CreateBatchFromRecords(mainCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch partialPricedTransformedBatch = DataRecordBatch.CreateBatchFromRecords(partialPricedCDRs, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch billingTransformedBatch = DataRecordBatch.CreateBatchFromRecords(billingRecords, queueItemType.BatchDescription, recordTypeId);
            DataRecordBatch trafficTransformedBatch = DataRecordBatch.CreateBatchFromRecords(billingRecords.Union(trafficRecords).ToList(), queueItemType.BatchDescription, recordTypeId);

            SendOutputData(context, mainTransformedBatch, partialPricedTransformedBatch, billingTransformedBatch, trafficTransformedBatch);
        }

        private void SendOutputData(IQueueActivatorExecutionContext context, DataRecordBatch mainTransformedBatch, DataRecordBatch partialPricedTransformedBatch,
            DataRecordBatch billingTransformedBatch, DataRecordBatch trafficTransformedBatch)
        {
            if (mainTransformedBatch != null && mainTransformedBatch.GetRecordCount() > 0 && this.MainOutputStages != null)
            {
                foreach (var mainOutputStage in this.MainOutputStages)
                    context.OutputItems.Add(mainOutputStage, mainTransformedBatch);
            }

            if (partialPricedTransformedBatch != null && partialPricedTransformedBatch.GetRecordCount() > 0 && this.PartialPricedOutputStages != null)
            {
                foreach (var partialPricedOutputStage in this.PartialPricedOutputStages)
                    context.OutputItems.Add(partialPricedOutputStage, partialPricedTransformedBatch);
            }

            if (billingTransformedBatch != null && billingTransformedBatch.GetRecordCount() > 0 && this.BillingOutputStages != null)
            {
                foreach (var billingOutputStage in this.BillingOutputStages)
                    context.OutputItems.Add(billingOutputStage, billingTransformedBatch);
            }

            if (trafficTransformedBatch != null && trafficTransformedBatch.GetRecordCount() > 0 && this.TrafficOutputStages != null)
            {
                foreach (var trafficOutputStage in this.TrafficOutputStages)
                    context.OutputItems.Add(trafficOutputStage, trafficTransformedBatch);
            }
        }

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(IReprocessStageActivatorInitializingContext context)
        {
            return null;
        }

        public void ExecuteStage(IReprocessStageActivatorExecutionContext context)
        {
            QueueExecutionFlowStage queueExecutionFlowStage = context.QueueExecutionFlowStage;
            var queueItemType = queueExecutionFlowStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;

            List<string> validMainOutputStages = BuildValidOutputStage(context.StageNames, this.MainOutputStages);
            List<string> validPartialPricedOutputStages = BuildValidOutputStage(context.StageNames, this.PartialPricedOutputStages);
            List<string> validBillingOutputStages = BuildValidOutputStage(context.StageNames, this.BillingOutputStages);
            List<string> validTrafficOutputStages = BuildValidOutputStage(context.StageNames, this.TrafficOutputStages);

            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                    {
                        GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as GenericDataRecordBatch;
                        if (genericDataRecordBatch == null)
                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                        List<dynamic> mainCDRs = new List<dynamic>();
                        List<dynamic> partialPricedCDRs = new List<dynamic>();
                        List<dynamic> billingRecords = new List<dynamic>(); //for billing stats and traffic stats
                        List<dynamic> trafficRecords = new List<dynamic>(); //for traffic stats

                        foreach (var record in genericDataRecordBatch.Records)
                            this.ClassifyRecord(record, mainCDRs, partialPricedCDRs, billingRecords, trafficRecords);

                        if (validMainOutputStages != null && mainCDRs.Count > 0)
                        {
                            var mainGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = mainCDRs };
                            foreach (var mainOutputStageName in validMainOutputStages)
                            {
                                context.EnqueueBatch(mainOutputStageName, mainGenericDataRecordBatch);
                            }
                        }

                        if (validPartialPricedOutputStages != null && partialPricedCDRs.Count > 0)
                        {
                            var partialPricedGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = partialPricedCDRs };
                            foreach (var partialPricedOutputStageName in validPartialPricedOutputStages)
                            {
                                context.EnqueueBatch(partialPricedOutputStageName, partialPricedGenericDataRecordBatch);
                            }
                        }

                        if (validBillingOutputStages != null && billingRecords.Count > 0)
                        {
                            var billingGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = billingRecords };
                            foreach (var billingOutputStageName in validBillingOutputStages)
                            {
                                context.EnqueueBatch(billingOutputStageName, billingGenericDataRecordBatch);
                            }
                        }

                        if (validTrafficOutputStages != null && (billingRecords.Count > 0 || trafficRecords.Count > 0))
                        {
                            var trafficGenericDataRecordBatch = new Vanrise.Reprocess.Entities.GenericDataRecordBatch() { Records = billingRecords.Union(trafficRecords).ToList() };
                            foreach (var trafficOutputStageName in validTrafficOutputStages)
                            {
                                context.EnqueueBatch(trafficOutputStageName, trafficGenericDataRecordBatch);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });
        }

        private List<string> BuildValidOutputStage(List<string> currentStages, List<string> outputStagesToCheck)
        {
            if (outputStagesToCheck != null && currentStages != null)
            {
                List<string> validOutputStages = new List<string>();
                foreach (var stageToCheck in outputStagesToCheck)
                {
                    if (currentStages.Contains(stageToCheck))
                        validOutputStages.Add(stageToCheck);
                }
                return validOutputStages.Count > 0 ? validOutputStages : null;
            }
            return null;
        }

        public void FinalizeStage(IReprocessStageActivatorFinalizingContext context)
        {
        }

        public int? GetStorageRowCount(IReprocessStageActivatorGetStorageRowCountContext context)
        {
            return null;
        }

        public void CommitChanges(IReprocessStageActivatorCommitChangesContext context)
        {
        }

        public void DropStorage(IReprocessStageActivatorDropStorageContext context)
        {
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            if (MainOutputStages == null && PartialPricedOutputStages == null && BillingOutputStages == null && TrafficOutputStages == null)
                return null;

            if (stageNames == null)
                return null;

            List<string> allStages = new List<string>();
            if (MainOutputStages != null)
                allStages.AddRange(MainOutputStages);

            if (PartialPricedOutputStages != null)
                allStages.AddRange(PartialPricedOutputStages);

            if (BillingOutputStages != null)
                allStages.AddRange(BillingOutputStages);

            if (TrafficOutputStages != null)
                allStages.AddRange(TrafficOutputStages);

            Func<string, bool> filterExpression = (itemObject) => stageNames.Contains(itemObject);

            IEnumerable<string> filteredStages = allStages.FindAllRecords(filterExpression);
            return filteredStages != null ? filteredStages.ToList() : null;
        }

        public Vanrise.Queueing.BaseQueue<IReprocessBatch> GetQueue()
        {
            return new Vanrise.Queueing.MemoryQueue<Vanrise.Reprocess.Entities.IReprocessBatch>();
        }

        public List<BatchRecord> GetStageBatchRecords(IReprocessStageActivatorPreparingContext context)
        {
            return null;
        }

        #endregion

        private void ClassifyRecord(dynamic record, List<dynamic> mainCDRs, List<dynamic> partialPricedCDRs, List<dynamic> billingRecords, List<dynamic> trafficRecords)
        {
            int cdrType = record.Type;
            if (!cdrTypesToEvaluate.Contains(cdrType))
            {
                trafficRecords.Add(record);
                return;
            }

            if (cdrType == 1)
                mainCDRs.Add(record);
            else
                partialPricedCDRs.Add(record);

            billingRecords.Add(record);
        }
    }
}
