using System;
using System.Collections.Generic;
using System.Linq;
using Mediation.Generic.Data;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;

namespace Mediation.Generic.QueueActivators
{
    public class StoreStagingRecordsQueueActivator : QueueActivator
    {
        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        DataStoreManager _dataStoreManager = new DataStoreManager();
        public string SessionRecordTypeId { get; set; }
        public string EventTimeRecordTypeId { get; set; }
        public List<StatusMapping> StatusMappings { get; set; }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            IStoreStagingRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IStoreStagingRecordsDataManager>();

            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            RecordFilterManager filterManager = new RecordFilterManager();

            List<StoreStagingRecord> storeStagingRecords = new List<StoreStagingRecord>();
            foreach (var batchRecord in batchRecords)
            {
                DataRecordFilterGenericFieldMatchContext dataRecordFilterContext = new DataRecordFilterGenericFieldMatchContext(batchRecord, recordTypeId);
                StoreStagingRecord storeStagingRecord = new StoreStagingRecord();
                storeStagingRecord.SessionId = (long)GetPropertyValue(batchRecord, SessionRecordTypeId);
                storeStagingRecord.EventTime = (DateTime)GetPropertyValue(batchRecord, EventTimeRecordTypeId);
                foreach (var statusMapping in StatusMappings)
                {
                    if (filterManager.IsFilterGroupMatch(statusMapping.FilterGroup, dataRecordFilterContext))
                    {
                        storeStagingRecord.EventStatus = statusMapping.Status;
                        break;
                    }
                }
                storeStagingRecord.EventDetails = batchRecord;
                storeStagingRecords.Add(storeStagingRecord);
            }
            dataManager.SaveStoreStagingRecordsToDB(storeStagingRecords);
        }

        object GetPropertyValue(object batchRecord, string propertyName)
        {
            var reader = Vanrise.Common.Utilities.GetPropValueReader(propertyName);
            return reader.GetPropertyValue(batchRecord);
        }
    }
}
