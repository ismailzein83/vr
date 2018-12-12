using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Analytic.Data.RDB;
using Vanrise.GenericData.Business;
using Vanrise.Common;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RecordStorageRDBAnalyticDataProviderTable : RDBAnalyticDataProviderTable
    {
        public override Guid ConfigId => new Guid("6623B11A-333D-487C-8906-B07F1784F944");

        public Guid RecordStorageId { get; set; }

        static DataRecordStorageManager s_recordStorageManager = new DataRecordStorageManager();
        static DataStoreManager s_dataStoreManager = new DataStoreManager();

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
