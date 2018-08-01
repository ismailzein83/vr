using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.Entities;
using System.Threading;
using System.Configuration;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class LoadCDRsToCorrelateInput
    {
        public Guid RecordStorageId { get; set; }

        public string IdFieldName { get; set; }

        public MemoryQueue<RecordBatch> OutputQueue { get; set; }

        public List<RecordFilterGroup> RecordFilterGroups { get; set; }

        public string OrderColumnName { get; set; }

        public bool IsOrderAscending { get; set; }
    }

    public class LoadCDRsToCorrelateOutput
    {
    }

    #endregion

    public sealed class LoadCDRsToCorrelate : BaseAsyncActivity<LoadCDRsToCorrelateInput, LoadCDRsToCorrelateOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> RecordStorageId { get; set; }

        [RequiredArgument]
        public InArgument<string> IdFieldName { get; set; }

        [RequiredArgument]
        public InArgument<List<RecordFilterGroup>> RecordFilterGroups { get; set; }

        [RequiredArgument]
        public InArgument<string> OrderColumnName { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsOrderAscending { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<RecordBatch>> OutputQueue { get; set; }

        protected override LoadCDRsToCorrelateOutput DoWorkWithResult(LoadCDRsToCorrelateInput inputArgument, AsyncActivityHandle handle)
        {
            int maximumOutputQueueSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_MaxCorrelateQueueSize"], out maximumOutputQueueSize))
                maximumOutputQueueSize = 20;

            int batchSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_BatchSize"], out batchSize))
                batchSize = 100000;

            if (inputArgument.OutputQueue == null)
                throw new NullReferenceException("inputArgument.OutputQueue");

            LoadCDRsToCorrelateOutput output = new LoadCDRsToCorrelateOutput() { };
            long eventCount = 0;

            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            List<RecordFilterGroup> recordFilterGroups = inputArgument.RecordFilterGroups;
            string orderColumnName = inputArgument.OrderColumnName;
            bool isOrderAscending = inputArgument.IsOrderAscending;


            foreach (RecordFilterGroup recordFilterGroup in recordFilterGroups)
            {
                new DataRecordStorageManager().GetDataRecords(inputArgument.RecordStorageId, null, null, recordFilterGroup, () => ShouldStop(handle), ((itm) =>
                {
                    eventCount++;
                    recordBatch.Records.Add(itm);

                    if (recordBatch.Records.Count >= batchSize)
                    {
                        inputArgument.OutputQueue.Enqueue(recordBatch);
                        recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                    }
                }), orderColumnName, isOrderAscending);

                if (recordBatch.Records.Count > 0)
                    inputArgument.OutputQueue.Enqueue(recordBatch);

                while (inputArgument.OutputQueue.Count >= maximumOutputQueueSize)
                    Thread.Sleep(1000);
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Loading Source Records is done. Events Count: {0}", eventCount);
            return output;
        }

        protected override LoadCDRsToCorrelateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsToCorrelateInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                IdFieldName = this.IdFieldName.Get(context),
                RecordStorageId = this.RecordStorageId.Get(context),
                RecordFilterGroups = this.RecordFilterGroups.Get(context),
                OrderColumnName = this.OrderColumnName.Get(context),
                IsOrderAscending = this.IsOrderAscending.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsToCorrelateOutput result)
        {
        }
    }
}