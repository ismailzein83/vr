using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments

    public class InsertCorrelatedCDRsInput
    {
        public BaseQueue<CDRCorrelationBatch> InputQueueToInsert { get; set; }
    }
    public class InsertCorrelatedCDRsOutput
    {
    }

    #endregion

    public class InsertCorrelatedCDRs : DependentAsyncActivity<InsertCorrelatedCDRsInput, InsertCorrelatedCDRsOutput>
    {

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRCorrelationBatch>> InputQueueToInsert { get; set; }

        protected override InsertCorrelatedCDRsOutput DoWorkWithResult(InsertCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Guid dataRecordStorage = new Guid();
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);
            int maxDBNumberQuery = recordStorageDataManager.GetDBQueryMaxParameterNumber();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueueToInsert.TryDequeue((recordBatch) =>
                    {
                        DateTime batchStartTime = DateTime.Now;
                        recordStorageDataManager.InsertRecords(recordBatch.OutputRecordsToInsert);
                        double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                            recordBatch.OutputRecordsToInsert.Count, elapsedTime.ToString());
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs is done.");
            return new InsertCorrelatedCDRsOutput();
        }

        protected override InsertCorrelatedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new InsertCorrelatedCDRsInput()
            {
                InputQueueToInsert = this.InputQueueToInsert.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertCorrelatedCDRsOutput result)
        {
        }
    }
}
