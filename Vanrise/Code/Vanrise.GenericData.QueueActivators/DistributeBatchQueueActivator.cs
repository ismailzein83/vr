//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.BusinessProcess;
//using Vanrise.GenericData.Business;
//using Vanrise.GenericData.Entities;
//using Vanrise.Common;

//namespace Vanrise.GenericData.QueueActivators
//{
//    public class DistributeBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
//    {
//        public List<string> OutputStages { get; set; }

//        public override void OnDisposed()
//        {
//            throw new NotImplementedException();
//        }

//        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
//        {
//            throw new NotImplementedException();
//        }



//        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
//        {
//            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;

//            if (this.OutputStages != null)
//            {
//                foreach (var stageName in this.OutputStages)
//                {
//                    context.OutputItems.Add(stageName, dataRecordBatch);
//                }
//            }
//        }

//        void Reprocess.Entities.IReprocessStageActivator.ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
//        {
//            context.DoWhilePreviousRunning(() =>
//            {
//                bool hasItem = false;
//                List<string> validStages = GetOutputStages(context.StageNames);
//                do
//                {
//                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
//                    {
//                        Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
//                        if (genericDataRecordBatch == null)
//                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

//                        if (this.OutputStages != null)
//                        {
//                            foreach (var stageName in this.OutputStages)
//                            {
//                                if (validStages != null && validStages.Contains(stageName))
//                                    context.EnqueueBatch(stageName, genericDataRecordBatch);
//                            }
//                        }
//                    });
//                } while (!context.ShouldStop() && hasItem);
//            });
//        }

//        private List<string> GetOutputStages(List<string> stageNames)
//        {
//            if (OutputStages == null)
//                return null;

//            if (stageNames == null)
//                return null;

//            Func<string, bool> filterExpression = (itemObject) => stageNames.Contains(itemObject);

//            IEnumerable<string> filteredStages = OutputStages.FindAllRecords(filterExpression);
//            return filteredStages != null ? filteredStages.ToList() : null;
//        }

//        void Reprocess.Entities.IReprocessStageActivator.FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
//        {

//        }

//        List<string> Reprocess.Entities.IReprocessStageActivator.GetOutputStages(List<string> stageNames)
//        {
//            return null;
//        }

//        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> Reprocess.Entities.IReprocessStageActivator.GetQueue()
//        {
//            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
//        }

//        public List<Reprocess.Entities.BatchRecord> GetStageBatchRecords(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
//        {
//            return null;
//        }
//    }
//}
