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
            var sqlRecordStorageSettings = context.RecordStorage.Settings as SQLDataRecordStorageSettings;
            var sqlRecordStorageState = context.RecordStorageState as SQLDataRecordStorageState;
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;

            if (sqlRecordStorageState == null)
                CreateSQLTable(sqlDataStoreSettings, sqlRecordStorageSettings);
            else
                UpdateSQLTable(sqlRecordStorageSettings, sqlRecordStorageState);
        }

        private void CreateSQLTable(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings)
        {
            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(dataStoreSettings, dataRecordStorageSettings);
            dataManager.CreateSQLRecordStorageTable();
        }

        private static void UpdateSQLTable(SQLDataRecordStorageSettings sqlRecordStorageSettings, SQLDataRecordStorageState sqlRecordStorageState)
        {
            List<SQLDataRecordStorageColumn> columnsToAdd = new List<SQLDataRecordStorageColumn>();
            foreach (var columnSetting in sqlRecordStorageSettings.Columns)
            {
                var matchExistingColumnState = sqlRecordStorageState.ExistingSettings.Columns.FirstOrDefault(itm => itm.ColumnName == columnSetting.ColumnName);
                if (matchExistingColumnState == null)
                    columnsToAdd.Add(columnSetting);
                else if (columnSetting.SQLDataType != matchExistingColumnState.SQLDataType)
                    throw new Exception(String.Format("Cannot change column type of column '{0}'", columnSetting.ColumnName));
            }
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            return new SQLRecordStorageDataManager(context.DataStore.Settings as SQLDataStoreSettings, context.DataRecordStorage.Settings as SQLDataRecordStorageSettings);
        }
    }
}
