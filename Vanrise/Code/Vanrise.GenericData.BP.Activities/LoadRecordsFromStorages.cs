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

    public class LoadRecordsFromStoragesInput
    {
        public List<Guid> RecordStorageIds { get; set; }

        public DateTime? FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        public BaseQueue<RecordBatch> OutputQueue { get; set; }

        public RecordFilterGroup RecordFilterGroup { get; set; }

        public string OrderColumnName { get; set; }

        public bool IsOrderAscending { get; set; }
    }

    public class LoadRecordsFromStoragesOutput
    {
        public long EventCount { get; set; }
    }

    #endregion

    public sealed class LoadRecordsFromStorages : BaseAsyncActivity<LoadRecordsFromStoragesInput, LoadRecordsFromStoragesOutput>
    {
        [RequiredArgument]
        public InArgument<List<Guid>> RecordStorageIds { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> ToTime { get; set; }

        public InArgument<RecordFilterGroup> RecordFilterGroup { get; set; }

        public InArgument<string> OrderColumnName { get; set; }

        public InArgument<bool> IsOrderAscending { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RecordBatch>> OutputQueue { get; set; }

        public OutArgument<long> EventCount { get; set; }

        protected override LoadRecordsFromStoragesOutput DoWorkWithResult(LoadRecordsFromStoragesInput inputArgument, AsyncActivityHandle handle)
        {
            if (inputArgument.OutputQueue == null)
                throw new NullReferenceException("inputArgument.OutputQueue");

            if (inputArgument.RecordStorageIds == null)
                throw new NullReferenceException("inputArgument.RecordStorageIds");

            LoadRecordsFromStoragesOutput output = new LoadRecordsFromStoragesOutput() { };
            long eventCount = 0;

            DataRecordStorageManager manager = new DataRecordStorageManager();
            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            RecordFilterGroup recordFilterGroup = inputArgument.RecordFilterGroup;
            string orderColumnName = inputArgument.OrderColumnName;
            bool isOrderAscending = inputArgument.IsOrderAscending;

            foreach (Guid recordStorageId in inputArgument.RecordStorageIds)
            {
                manager.GetDataRecords(recordStorageId, inputArgument.FromTime, inputArgument.ToTime, recordFilterGroup, () => ShouldStop(handle), ((itm) =>
                {
                    eventCount++;
                    recordBatch.Records.Add(itm);
                    if (recordBatch.Records.Count >= 10000)
                    {
                        inputArgument.OutputQueue.Enqueue(recordBatch);

                        recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                    }
                }), orderColumnName, isOrderAscending);
            }

            if (recordBatch.Records.Count > 0)
                inputArgument.OutputQueue.Enqueue(recordBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Loading Source Records is done. Events Count: {0}", eventCount);

            output.EventCount = eventCount;
            return output;
        }

        protected override LoadRecordsFromStoragesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadRecordsFromStoragesInput()
            {
                FromTime = this.FromTime.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                RecordStorageIds = this.RecordStorageIds.Get(context),
                ToTime = this.ToTime.Get(context),
                RecordFilterGroup = this.RecordFilterGroup.Get(context),
                OrderColumnName = this.OrderColumnName.Get(context),
                IsOrderAscending = this.IsOrderAscending.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadRecordsFromStoragesOutput result)
        {
            this.EventCount.Set(context, result.EventCount);
        }
    }
}