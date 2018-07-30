using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class DeleteRecordsFromStorageInput
    {
        public BaseQueue<DeleteRecordsBatch> InputQueue { get; set; }

        public Guid DataRecordStorageId { get; set; }

        public string IdFieldName { get; set; }

        public string DateTimeFieldName { get; set; }
    }

    public class DeleteRecordsFromStorageOutput
    {

    }

    #endregion

    public sealed class DeleteRecordsFromStorage : DependentAsyncActivity<DeleteRecordsFromStorageInput, DeleteRecordsFromStorageOutput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<DeleteRecordsBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<Guid> DataRecordStorageId { get; set; }

        [RequiredArgument]
        public InArgument<string> IdFieldName { get; set; }

        [RequiredArgument]
        public InArgument<string> DateTimeFieldName { get; set; }

        protected override DeleteRecordsFromStorageOutput DoWorkWithResult(DeleteRecordsFromStorageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(inputArgument.DataRecordStorageId);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((deleteRecordBatch) =>
                    {
                        DateTime batchStartTime = DateTime.Now;
                        recordStorageDataManager.DeleteRecords(deleteRecordBatch.DateTimeRange.From, deleteRecordBatch.DateTimeRange.To, deleteRecordBatch.IdsToDelete, 
                            inputArgument.IdFieldName, inputArgument.DateTimeFieldName);

                        double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Delete batch CDRs is done. Events Count: {0}. ElapsedTime: {1} (s)",
                            deleteRecordBatch.IdsToDelete.Count, elapsedTime.ToString());
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Delete Initial CDRs is done.");
            return new DeleteRecordsFromStorageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, DeleteRecordsFromStorageOutput result)
        {

        }

        protected override DeleteRecordsFromStorageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new DeleteRecordsFromStorageInput()
            {
                DataRecordStorageId = this.DataRecordStorageId.Get(context),
                InputQueue = this.InputQueue.Get(context),
                DateTimeFieldName = this.DateTimeFieldName.Get(context),
                IdFieldName = this.IdFieldName.Get(context)
            };
        }
    }
}