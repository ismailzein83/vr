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

    public class LoadCDRsToCorrelateInput
    {
        public Guid RecordStorageId { get; set; }

        public string IdFieldName { get; set; }

        public BaseQueue<RecordBatch> OutputQueue { get; set; }

        public RecordFilterGroup RecordFilterGroup { get; set; }

        public string OrderColumnName { get; set; }

        public bool IsOrderAscending { get; set; }
    }

    public class LoadCDRsToCorrelateOutput
    {
        public long LastImportedId { get; set; }
    }

    #endregion

    public sealed class LoadCDRsToCorrelate : BaseAsyncActivity<LoadCDRsToCorrelateInput, LoadCDRsToCorrelateOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> RecordStorageId { get; set; }

        [RequiredArgument]
        public InArgument<string> IdFieldName { get; set; }

        [RequiredArgument]
        public InArgument<RecordFilterGroup> RecordFilterGroup { get; set; }

        [RequiredArgument]
        public InArgument<string> OrderColumnName { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsOrderAscending { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RecordBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public OutArgument<long> LastImportedId { get; set; }

        protected override LoadCDRsToCorrelateOutput DoWorkWithResult(LoadCDRsToCorrelateInput inputArgument, AsyncActivityHandle handle)
        {
            if (inputArgument.OutputQueue == null)
                throw new NullReferenceException("inputArgument.OutputQueue");

            LoadCDRsToCorrelateOutput output = new LoadCDRsToCorrelateOutput() { };
            long eventCount = 0;

            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            RecordFilterGroup recordFilterGroup = inputArgument.RecordFilterGroup;
            string orderColumnName = inputArgument.OrderColumnName;
            bool isOrderAscending = inputArgument.IsOrderAscending;

            long lastRecordId = 0;

            new DataRecordStorageManager().GetDataRecords(inputArgument.RecordStorageId, null, null, recordFilterGroup, () => ShouldStop(handle), ((itm) =>
            {
                eventCount++;
                recordBatch.Records.Add(itm);
                long itmRecordId = itm.GetFieldValue(inputArgument.IdFieldName);
                if (itmRecordId > lastRecordId)
                    lastRecordId = itmRecordId;

                if (recordBatch.Records.Count >= 100000)
                {
                    inputArgument.OutputQueue.Enqueue(recordBatch);
                    recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                }
            }), orderColumnName, isOrderAscending);

            if (recordBatch.Records.Count > 0)
                inputArgument.OutputQueue.Enqueue(recordBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Loading Source Records is done. Events Count: {0}", eventCount);
            output.LastImportedId = lastRecordId;
            return output;
        }

        protected override LoadCDRsToCorrelateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsToCorrelateInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                IdFieldName = this.IdFieldName.Get(context),
                RecordStorageId = this.RecordStorageId.Get(context),
                RecordFilterGroup = this.RecordFilterGroup.Get(context),
                OrderColumnName = this.OrderColumnName.Get(context),
                IsOrderAscending = this.IsOrderAscending.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsToCorrelateOutput result)
        {
            this.LastImportedId.Set(context, result.LastImportedId);
        }
    }
}