using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    public class SQLDataStoreSettings : DataStoreSettings
    {
        public string ConnectionString { get; set; }

        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;
            var sqlRecordStorageSettings = context.RecordStorage.Settings as SQLDataRecordStorageSettings;
            var existingRecordStorageSettings = context.ExistingRecordSettings as SQLDataRecordStorageSettings;

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlRecordStorageSettings);

            if (existingRecordStorageSettings == null)
                dataManager.CreateSQLRecordStorageTable();
            else
                dataManager.AlterSQLRecordStorageTable(existingRecordStorageSettings);
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            return new SQLRecordStorageDataManager(context.DataStore.Settings as SQLDataStoreSettings, context.DataRecordStorage.Settings as SQLDataRecordStorageSettings);
        }
    }
}
