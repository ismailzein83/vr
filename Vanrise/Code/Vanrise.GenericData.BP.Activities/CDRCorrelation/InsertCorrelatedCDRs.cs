using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.Entities;
using System.Configuration;

namespace Vanrise.GenericData.BP.Activities
{
    #region Arguments

    public class InsertCorrelatedCDRsInput
    {
        public CDRCorrelationDefinition CDRCorrelationDefinition { get; set; }

        public BaseQueue<CDRCorrelationBatch> InputQueueToInsert { get; set; }

        public MemoryQueue<DeleteRecordsBatch> OutputQueueToDelete { get; set; }
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
        public InOutArgument<MemoryQueue<DeleteRecordsBatch>> OutputQueueToDelete { get; set; }

        protected override InsertCorrelatedCDRsOutput DoWorkWithResult(InsertCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(inputArgument.CDRCorrelationDefinition.Settings.OutputDataRecordStorageId);
            int maxDBNumberQuery = recordStorageDataManager.GetDBQueryMaxParameterNumber();

            int batchSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["CDRCorrelation_InsertCorrelatedCDRs_BatchSize"], out batchSize))
                batchSize = 50000;

            CDRCorrelationBatch batchToInsert = new CDRCorrelationBatch
            {
                OutputRecordsToInsert = new List<dynamic>(),
                InputIdsToDelete = new HashSet<long>(),
                DateTimeRange = new DateTimeRange { From = DateTime.MaxValue }
            };

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueueToInsert.TryDequeue((recordBatch) =>
                    {
                        batchToInsert.OutputRecordsToInsert.AddRange(recordBatch.OutputRecordsToInsert);
                        batchToInsert.InputIdsToDelete.UnionWith(recordBatch.InputIdsToDelete);
                        if (batchToInsert.DateTimeRange.From > recordBatch.DateTimeRange.From)
                            batchToInsert.DateTimeRange.From = recordBatch.DateTimeRange.From;
                        if (batchToInsert.DateTimeRange.To < recordBatch.DateTimeRange.To.AddSeconds(1))
                            batchToInsert.DateTimeRange.To = recordBatch.DateTimeRange.To.AddSeconds(1);
                        
                        if(batchToInsert.OutputRecordsToInsert.Count >= batchSize)
                        {
                            InsertRecords(batchToInsert, recordStorageDataManager, inputArgument, handle);
                            batchToInsert = new CDRCorrelationBatch
                            {
                                OutputRecordsToInsert = new List<dynamic>(),
                                InputIdsToDelete = new HashSet<long>(),
                                DateTimeRange = new DateTimeRange { From = DateTime.MaxValue }
                            };
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (batchToInsert.OutputRecordsToInsert.Count > 0)
                InsertRecords(batchToInsert, recordStorageDataManager, inputArgument, handle);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs is done.");
            return new InsertCorrelatedCDRsOutput();
        }

        private void InsertRecords(CDRCorrelationBatch recordBatch, IDataRecordDataManager recordStorageDataManager, InsertCorrelatedCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime batchStartTime = DateTime.Now;
            recordStorageDataManager.InsertRecords(recordBatch.OutputRecordsToInsert);

            TimeSpan elapsedTime = DateTime.Now - batchStartTime;
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1}",
                recordBatch.OutputRecordsToInsert.Count, elapsedTime.ToString(@"hh\:mm\:ss\.fff"));

            inputArgument.OutputQueueToDelete.Enqueue(new DeleteRecordsBatch() { IdsToDelete = recordBatch.InputIdsToDelete, DateTimeRange = recordBatch.DateTimeRange });
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