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

        public List<CDRCorrelationFilterGroup> CDRCorrelationFilterGroups { get; set; }

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
        public InArgument<List<CDRCorrelationFilterGroup>> CDRCorrelationFilterGroups { get; set; }

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
            long totalRecordsCount = 0;

            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            List<CDRCorrelationFilterGroup> cdrRCorrelationFilterGroups = inputArgument.CDRCorrelationFilterGroups;
            string orderColumnName = inputArgument.OrderColumnName;
            bool isOrderAscending = inputArgument.IsOrderAscending;

            foreach (CDRCorrelationFilterGroup cdrCorrelationFilterGroup in cdrRCorrelationFilterGroups)
            {
                string rangeMessage;
                if (cdrCorrelationFilterGroup.To.HasValue)
                    rangeMessage = string.Format("Range From {0}, To {1}", cdrCorrelationFilterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"), cdrCorrelationFilterGroup.To.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    rangeMessage = string.Format("From {0}", cdrCorrelationFilterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"));

                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has started.", rangeMessage));

                long batchRecordsCount = 0;
                DateTime batchStartTime = DateTime.Now;

                new DataRecordStorageManager().GetDataRecords(inputArgument.RecordStorageId, null, null, cdrCorrelationFilterGroup.RecordFilterGroup, () => ShouldStop(handle), ((itm) =>
                {
                    totalRecordsCount++;
                    batchRecordsCount++;
                    recordBatch.Records.Add(itm);

                    if (recordBatch.Records.Count >= batchSize)
                    {
                        inputArgument.OutputQueue.Enqueue(recordBatch);
                        recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                    }
                }), orderColumnName, isOrderAscending);

                if (recordBatch.Records.Count > 0)
                {
                    inputArgument.OutputQueue.Enqueue(recordBatch);
                    recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                }

                double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has finished. Events Count: {1}. Time Elapsed: {2} (s)", rangeMessage, batchRecordsCount, elapsedTime));

                while (inputArgument.OutputQueue.Count >= maximumOutputQueueSize)
                    Thread.Sleep(1000);
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Loading Source Records is done. Events Count: {0}", totalRecordsCount);
            return output;
        }

        protected override LoadCDRsToCorrelateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsToCorrelateInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                IdFieldName = this.IdFieldName.Get(context),
                RecordStorageId = this.RecordStorageId.Get(context),
                CDRCorrelationFilterGroups = this.CDRCorrelationFilterGroups.Get(context),
                OrderColumnName = this.OrderColumnName.Get(context),
                IsOrderAscending = this.IsOrderAscending.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsToCorrelateOutput result)
        {
        }
    }
}