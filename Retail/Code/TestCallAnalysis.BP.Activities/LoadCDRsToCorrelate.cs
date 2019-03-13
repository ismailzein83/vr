using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using TestCallAnalysis.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    #region Arguments

    public class LoadCDRsToCorrelateInput
    {
        public MemoryQueue<RecordBatch> OutputQueue { get; set; }

        public List<Entities.CDRCorrelationFilterGroup> CDRCorrelationFilterGroups { get; set; }

    }
    public class LoadCDRsToCorrelateOutput
    {
    }

    #endregion

    public sealed class LoadCDRsToCorrelate : BaseAsyncActivity<LoadCDRsToCorrelateInput, LoadCDRsToCorrelateOutput>
    {

        [RequiredArgument]
        public InArgument<List<Entities.CDRCorrelationFilterGroup>> CDRCorrelationFilterGroups { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<RecordBatch>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<RecordBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override LoadCDRsToCorrelateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsToCorrelateInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                CDRCorrelationFilterGroups = this.CDRCorrelationFilterGroups.Get(context),
            };
        }

        protected override LoadCDRsToCorrelateOutput DoWorkWithResult(LoadCDRsToCorrelateInput inputArgument, AsyncActivityHandle handle)
        {
            int maximumOutputQueueSize = 20;
            long totalRecordsCount = 0;
            Guid mappedCDRDataRecordStorageId = MappedCDRManager.dataRecordStorage;

            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            List<Entities.CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = inputArgument.CDRCorrelationFilterGroups;

            if (cdrCorrelationFilterGroups != null && cdrCorrelationFilterGroups.Count > 0)
            {
                foreach (var filterGroup in cdrCorrelationFilterGroups)
                {
                    string rangeMessage;
                    if (filterGroup.To.HasValue)
                        rangeMessage = string.Format("Range From {0}, To {1}", filterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"), filterGroup.To.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    else
                        rangeMessage = string.Format("From {0}", filterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"));

                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has started.", rangeMessage));

                    long recordsCount = 0;
                    DateTime startTime = DateTime.Now;
                    new DataRecordStorageManager().GetDataRecords(mappedCDRDataRecordStorageId, filterGroup.From, filterGroup.To, filterGroup.RecordFilterGroup, () => ShouldStop(handle), ((itm) =>
                    {
                        totalRecordsCount++;
                        recordsCount++;
                        recordBatch.Records.Add(itm);
                    }), null, true);

                    inputArgument.OutputQueue.Enqueue(recordBatch);

                    double elapsedTime = Math.Round((DateTime.Now - startTime).TotalSeconds);
                    handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has finished. Events Count: {1}. Time Elapsed: {2} (s)", rangeMessage, recordsCount, elapsedTime));

                    while (inputArgument.OutputQueue.Count >= maximumOutputQueueSize)
                        Thread.Sleep(1000);
                }
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Loading Source Records is done. Events Count: {0}", totalRecordsCount);
            return new LoadCDRsToCorrelateOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsToCorrelateOutput result)
        {
        }
    }
}
