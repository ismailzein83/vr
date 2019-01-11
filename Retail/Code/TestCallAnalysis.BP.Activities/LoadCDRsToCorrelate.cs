using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        protected override LoadCDRsToCorrelateOutput DoWorkWithResult(LoadCDRsToCorrelateInput inputArgument, AsyncActivityHandle handle)
        {
            int maximumOutputQueueSize = 20;
            //if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_MaxCorrelateQueueSize"], out maximumOutputQueueSize))
            //    maximumOutputQueueSize = 20;

            int batchSize = 100000;
            //if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_BatchSize"], out batchSize))
            //    batchSize = 100000;

            if (inputArgument.OutputQueue == null)
                throw new NullReferenceException("inputArgument.OutputQueue");   

            LoadCDRsToCorrelateOutput output = new LoadCDRsToCorrelateOutput() { };
            long totalRecordsCount = 0;

            RecordBatch recordBatch = new RecordBatch() { Records = new List<dynamic>() };

            List<Entities.CDRCorrelationFilterGroup> cdrRCorrelationFilterGroups = inputArgument.CDRCorrelationFilterGroups;
            string orderColumnName = "";
            bool isOrderAscending =true;

            foreach (Entities.CDRCorrelationFilterGroup cdrCorrelationFilterGroup in cdrRCorrelationFilterGroups)
            {
                string rangeMessage;
                if (cdrCorrelationFilterGroup.To.HasValue)
                    rangeMessage = string.Format("Range From {0}, To {1}", cdrCorrelationFilterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"), cdrCorrelationFilterGroup.To.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    rangeMessage = string.Format("From {0}", cdrCorrelationFilterGroup.From.ToString("yyyy-MM-dd HH:mm:ss"));

                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, string.Format("Loading {0} has started.", rangeMessage));

                long batchRecordsCount = 0;
                DateTime batchStartTime = DateTime.Now;
                var dataRecordStorageId = new Guid();
                new DataRecordStorageManager().GetDataRecords(dataRecordStorageId, null, null, cdrCorrelationFilterGroup.RecordFilterGroup, () => ShouldStop(handle), ((itm) =>
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
                CDRCorrelationFilterGroups = this.CDRCorrelationFilterGroups.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsToCorrelateOutput result)
        {
        }
    }
}
