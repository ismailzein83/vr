using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.Entities;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class InsertCorrelatedCDRsInput
    {
        public CDRCorrelationDefinition CDRCorrelationDefinition { get; set; }

        public BaseQueue<CDRCorrelationBatch> InputQueueToInsert { get; set; }

        public BaseQueue<DeleteRecordsBatch> OutputQueueToDelete { get; set; }
    }

    public class InsertCorrelatedCDRsOutput
    {
    }

    #endregion

    public sealed class InsertCorrelatedCDRs : DependentAsyncActivity<InsertCorrelatedCDRsInput, InsertCorrelatedCDRsOutput>
    {
        [RequiredArgument]
        public InArgument<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRCorrelationBatch>> InputQueueToInsert { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<DeleteRecordsBatch>> OutputQueueToDelete { get; set; }

        protected override InsertCorrelatedCDRsOutput DoWorkWithResult(InsertCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(inputArgument.CDRCorrelationDefinition.Settings.OutputDataRecordStorageId);
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

                        inputArgument.OutputQueueToDelete.Enqueue(new DeleteRecordsBatch() { IdsToDelete = recordBatch.InputIdsToDelete, DateTimeRange = recordBatch.DateTimeRange });
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
                CDRCorrelationDefinition = this.CDRCorrelationDefinition.Get(context),
                InputQueueToInsert = this.InputQueueToInsert.Get(context),
                OutputQueueToDelete = this.OutputQueueToDelete.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertCorrelatedCDRsOutput result)
        {
        }
    }
}