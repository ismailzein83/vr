using System;
using Vanrise.Analytic.Data.RDB;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RecordStorageRDBAnalyticDataProviderTable : RDBAnalyticDataProviderTable
    {
        public override Guid ConfigId => new Guid("6623B11A-333D-487C-8906-B07F1784F944");

        public Guid RecordStorageId { get; set; }

        static DataRecordStorageManager s_recordStorageManager = new DataRecordStorageManager();
        static DataStoreManager s_dataStoreManager = new DataStoreManager();

        public static string GetRDBTableNameByRecordStorageId(Guid recordStorageId)
        {
            var recordStorageRDBAnalyticDataProviderTable = new RecordStorageRDBAnalyticDataProviderTable();
            recordStorageRDBAnalyticDataProviderTable.RecordStorageId = recordStorageId;
            return recordStorageRDBAnalyticDataProviderTable.GetRDBTableName();
        }

        public override string GetRDBTableName()
        {
            var recordStorage = s_recordStorageManager.GetDataRecordStorage(this.RecordStorageId);
            recordStorage.ThrowIfNull("recordStorage", this.RecordStorageId);
            var dataStore = s_dataStoreManager.GetDataStore(recordStorage.DataStoreId);
            dataStore.ThrowIfNull("dataStore", recordStorage.DataStoreId);
            dataStore.Settings.ThrowIfNull("dataStore.Settings", dataStore.DataStoreId);
            var dataManager = dataStore.Settings.GetDataRecordDataManager(new GetRecordStorageDataManagerContext { DataStore = dataStore, DataRecordStorage = recordStorage });
            dataManager.ThrowIfNull("dataManager", this.RecordStorageId);
            RDBRecordStorageDataManager rdbRecordStorageDataManager = dataManager.CastWithValidate<RDBRecordStorageDataManager>("dataManager", this.RecordStorageId);
            return rdbRecordStorageDataManager.GetMainStorageRDBTableName();
        }
    }
}