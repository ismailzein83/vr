using System;
using System.Activities;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments

    public class InsertCorrelatedCDRsInput
    {
        public MemoryQueue<CDRCorrelationBatch> InputQueueToInsert { get; set; }
    }
    public class InsertCorrelatedCDRsOutput
    {
    }

    #endregion

    public class InsertCorrelatedCDRs : DependentAsyncActivity<InsertCorrelatedCDRsInput, InsertCorrelatedCDRsOutput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCorrelationBatch>> InputQueueToInsert { get; set; }

        protected override InsertCorrelatedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new InsertCorrelatedCDRsInput()
            {
                InputQueueToInsert = this.InputQueueToInsert.Get(context),
            };
        }
   
        protected override InsertCorrelatedCDRsOutput DoWorkWithResult(InsertCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CorrelatedCDRManager correlatedCDRManager = new CorrelatedCDRManager();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    if (inputArgument.InputQueueToInsert != null && inputArgument.InputQueueToInsert.Count > 0)
                    {
                        hasItem = inputArgument.InputQueueToInsert.TryDequeue((recordBatch) =>
                        {
                            DateTime batchStartTime = DateTime.Now;
                            correlatedCDRManager.InsertCorrelatedCDRs(recordBatch.OutputRecordsToInsert);
                            double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                recordBatch.OutputRecordsToInsert.Count, elapsedTime.ToString());
                        });
                    }

                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs is done.");
            return new InsertCorrelatedCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertCorrelatedCDRsOutput result)
        {
        }
    }
}
