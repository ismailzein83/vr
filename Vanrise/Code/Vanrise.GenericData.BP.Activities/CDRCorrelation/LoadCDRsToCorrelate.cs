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
using Vanrise.Common;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class LoadCDRsToCorrelateInput
    {
        public Guid RecordTypeId { get; set; }

        public Guid RecordStorageId { get; set; }

        public MemoryQueue<RecordBatch> OutputQueue { get; set; }

        public List<CDRCorrelationFilterGroup> CDRCorrelationFilterGroups { get; set; }

        public bool IsDateTimeOrderAscending { get; set; }
    }

    public class LoadCDRsToCorrelateOutput
    {
    }

    #endregion

    public sealed class LoadCDRsToCorrelate : BaseAsyncActivity<LoadCDRsToCorrelateInput, LoadCDRsToCorrelateOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> RecordTypeId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> RecordStorageId { get; set; }

        [RequiredArgument]
        public InArgument<List<CDRCorrelationFilterGroup>> CDRCorrelationFilterGroups { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsDateTimeOrderAscending { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<RecordBatch>> OutputQueue { get; set; }

        protected override LoadCDRsToCorrelateOutput DoWorkWithResult(LoadCDRsToCorrelateInput inputArgument, AsyncActivityHandle handle)
        {
            int maximumOutputQueueSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_MaxCorrelateQueueSize"], out maximumOutputQueueSize))
                maximumOutputQueueSize = 200;

            int batchSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_BatchSize"], out batchSize))
                batchSize = 10000;

            if (inputArgument.OutputQueue == null)
                throw new NullReferenceException("inputArgument.OutputQueue");

            DataRecordType inputDataRecordType = new DataRecordTypeManager().GetDataRecordType(inputArgument.RecordTypeId);
            string datetimeFieldName = inputDataRecordType.Settings.DateTimeField;

            LoadCDRsToCorrelateOutput output = new LoadCDRsToCorrelateOutput() { };
            long totalRecordsCount = 0;

            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            List<CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = inputArgument.CDRCorrelationFilterGroups;
            cdrCorrelationFilterGroups.ThrowIfNull("cdrCorrelationFilterGroups");

            foreach (CDRCorrelationFilterGroup cdrCorrelationFilterGroup in cdrCorrelationFilterGroups)
            {
                string rangeMessage;
                if (cdrCorrelationFilterGroup.To.HasValue)
                    rangeMessage = string.Format("Range From {0}, To {1}", cdrCorrelationFilterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"), cdrCorrelationFilterGroup.To.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    rangeMessage = string.Format("From {0}", cdrCorrelationFilterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"));

                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has started.", rangeMessage));

                long batchRecordsCount = 0;
                DateTime batchStartTime = DateTime.Now;

                new DataRecordStorageManager().GetDataRecords(inputArgument.RecordStorageId, cdrCorrelationFilterGroup.From, cdrCorrelationFilterGroup.To, cdrCorrelationFilterGroup.RecordFilterGroup, () => ShouldStop(handle), ((itm) =>
                {
                    totalRecordsCount++;
                    batchRecordsCount++;
                    recordBatch.Records.Add(itm);

                    if (recordBatch.Records.Count >= batchSize)
                    {
                        inputArgument.OutputQueue.Enqueue(recordBatch);
                        recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                    }
                }), datetimeFieldName, inputArgument.IsDateTimeOrderAscending);

                if (recordBatch.Records.Count > 0)
                {
                    inputArgument.OutputQueue.Enqueue(recordBatch);
                    recordBatch = new RecordBatch() { Records = new List<dynamic>() };
                }

                TimeSpan elapsedTime = DateTime.Now - batchStartTime;
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has finished. Events Count: {1}. ElapsedTime: {2}", 
                    rangeMessage, batchRecordsCount, elapsedTime.ToString(@"hh\:mm\:ss\.fff")));

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
                RecordTypeId = this.RecordTypeId.Get(context),
                RecordStorageId = this.RecordStorageId.Get(context),
                CDRCorrelationFilterGroups = this.CDRCorrelationFilterGroups.Get(context),
                IsDateTimeOrderAscending = this.IsDateTimeOrderAscending.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsToCorrelateOutput result)
        {
        }
    }
}