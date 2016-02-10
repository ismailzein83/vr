using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.QueueActivators
{
    public class StoreBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator
    {
        public int DataRecordStorageId { get; set; }

        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        DataStoreManager _dataStoreManager = new DataStoreManager();

        public override void OnDisposed()
        {
            throw new NotImplementedException();
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();
        }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            var dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(this.DataRecordStorageId);

            if (dataRecordStorage == null)
                throw new NullReferenceException(String.Format("dataRecordStorage. Id '{0}'", this.DataRecordStorageId));
            if (dataRecordStorage.Settings == null)
                throw new NullReferenceException("dataRecordStorage.Settings");

            var dataStore = _dataStoreManager.GeDataStore(dataRecordStorage.Settings.ConfigId);
            if (dataStore == null)
                throw new NullReferenceException(String.Format("dataStore. dataStore Id '{0}' dataRecordStorage Id '{1}'", dataRecordStorage.Settings.ConfigId, this.DataRecordStorageId));
            var getRecordStorageDataManagerContext = new GetRecordStorageDataManagerContext
            {
                DataStore = dataStore,
                DataRecordStorage = dataRecordStorage
            };
            var dataManager = dataStore.Settings.GetDataRecordDataManager(getRecordStorageDataManagerContext);
            var dbApplyStream = dataManager.InitialiazeStreamForDBApply();
            foreach(var record in batchRecords)
            {
                dataManager.WriteRecordToStream(record, dbApplyStream);
            }
            var streamReadyToApply = dataManager.FinishDBApplyStream(dbApplyStream);
            dataManager.ApplyStreamToDB(streamReadyToApply);
        }
    }
}
