using System;
using System.Activities;
using Vanrise.BusinessProcess;
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

        protected override DeleteRecordsFromStorageOutput DoWorkWithResult(DeleteRecordsFromStorageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(inputArgument.DataRecordStorageId);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((deletRecordBatch) =>
                    {
                        recordStorageDataManager.DeleteRecords(deletRecordBatch.DateTimeRange.From, deletRecordBatch.DateTimeRange.To, deletRecordBatch.RecordFilterGroup);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

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
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}