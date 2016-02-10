using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    internal class RecordStorageDataManager : BaseSQLDataManager, IDataRecordDataManager
    {
        DataStore _dataStore;
        DataRecordStorage _dataRecordStorage;

        internal RecordStorageDataManager(DataStore dataStore, DataRecordStorage dataRecordStorage)             
        {
            this._dataStore = dataStore;
            this._dataRecordStorage = dataRecordStorage;
        }

        protected override string GetConnectionString()
        {
            var dataStoreSettings = _dataStore.Settings as SQLDataStoreSettings;
            if (dataStoreSettings == null)
                throw new ArgumentNullException("dataStoreSettings");
            return dataStoreSettings.ConnectionString;
        }

        public void ApplyStreamToDB(object stream)
        {
            base.InsertBulkToTable(stream as BaseBulkInsertInfo);
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            var dataRecordStorageSettings = this._dataRecordStorage.Settings as SQLDataRecordStorageSettings;
            if (dataRecordStorageSettings == null)
                throw new ArgumentNullException("dataRecordStorageSettings");
            return new StreamBulkInsertInfo
            {
                TableName = dataRecordStorageSettings.TableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
    }
}
