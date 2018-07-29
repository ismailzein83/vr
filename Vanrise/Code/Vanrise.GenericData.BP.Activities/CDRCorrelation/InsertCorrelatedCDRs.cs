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

    public class InsertCorrelatedCDRsInput
    {
        public CDRCorrelationDefinition CDRCorrelationDefinition { get; set; }

        public BaseQueue<CDRCorrelationBatch> InputQueueToInsert { get; set; }

        public BaseQueue<DeleteRecordsBatch> OutputQueueToDelete { get; set; }
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
        public InOutArgument<BaseQueue<DeleteRecordsBatch>> OutputQueueToDelete { get; set; }

        protected override InsertCorrelatedCDRsOutput DoWorkWithResult(InsertCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(inputArgument.CDRCorrelationDefinition.Settings.OutputDataRecordStorageId);
            int maxDBNumberQuery = recordStorageDataManager.GetDBQueryMaxParameterNumber();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueueToInsert.TryDequeue((recordBatch) =>
                    {
                        DateTime batchStartTime = DateTime.Now;
                        recordStorageDataManager.InsertRecords(recordBatch.OutputRecordsToInsert);

                        double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs Batch is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                            recordBatch.OutputRecordsToInsert.Count, elapsedTime.ToString());

                        inputArgument.OutputQueueToDelete.Enqueue(new DeleteRecordsBatch() { IdsToDelete = recordBatch.InputIdsToDelete, DateTimeRange = recordBatch.DateTimeRange });

                        //if (recordBatch.InputIdsToDelete.Count <= maxDBNumberQuery)
                        //{
                        //    EnqueuItemsToDelete(inputArgument, recordBatch, recordBatch.InputIdsToDelete);
                        //}
                        //else
                        //{
                        //    List<long> idsToDelete = new List<long>();
                        //    foreach (var id in recordBatch.InputIdsToDelete)
                        //    {
                        //        idsToDelete.Add(id);
                        //        if (idsToDelete.Count == maxDBNumberQuery)
                        //        {
                        //            EnqueuItemsToDelete(inputArgument, recordBatch, idsToDelete);
                        //            idsToDelete = new List<long>();
                        //        }
                        //    }
                        //    if (idsToDelete.Count > 0)
                        //    {
                        //        EnqueuItemsToDelete(inputArgument, recordBatch, idsToDelete);
                        //    }
                        //}
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Insert Correlated CDRs is done.");
            return new InsertCorrelatedCDRsOutput();
        }

        //private void EnqueuItemsToDelete(InsertCorrelatedCDRsInput inputArgument, CDRCorrelationBatch recordBatch, List<long> idsToDelete)
        //{
        //    RecordFilterGroup recordFilterGroup = GetRecordFilterGroup(idsToDelete, inputArgument.CDRCorrelationDefinition.Settings.IdFieldName);
        //    DeleteRecordsBatch deleteRecordsBatch = new DeleteRecordsBatch() { DateTimeRange = recordBatch.DateTimeRange, RecordFilterGroup = recordFilterGroup, EventsCount = idsToDelete.Count };
        //    inputArgument.OutputQueueToDelete.Enqueue(deleteRecordsBatch);
        //}

        private RecordFilterGroup GetRecordFilterGroup(List<long> idsToDelete, string idFieldName)
        {
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = new List<RecordFilter>() };
            foreach (long id in idsToDelete)
                recordFilterGroup.Filters.Add(new NumberRecordFilter { FieldName = idFieldName, CompareOperator = NumberRecordFilterOperator.Equals, Value = id });

            return recordFilterGroup;
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