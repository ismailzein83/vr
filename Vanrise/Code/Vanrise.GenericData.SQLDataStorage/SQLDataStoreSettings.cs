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

        /// <summary>
        /// either ConnectionString or ConnectionStringName should have value. ConnectionString has more priority than ConnectionStringName
        /// </summary>
        public string ConnectionStringName { get; set; }

        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;
            var sqlRecordStorageSettings = context.RecordStorage.Settings as SQLDataRecordStorageSettings;
            var existingRecordStorageSettings = context.ExistingRecordSettings as SQLDataRecordStorageSettings;

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlRecordStorageSettings, context.RecordStorage);

            if (existingRecordStorageSettings == null)
                dataManager.CreateSQLRecordStorageTable();
            else
                dataManager.AlterSQLRecordStorageTable(existingRecordStorageSettings);
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            return new SQLRecordStorageDataManager(context.DataStore.Settings as SQLDataStoreSettings, context.DataRecordStorage.Settings as SQLDataRecordStorageSettings, context.DataRecordStorage);
        }

        public override ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context)
        {
            var sqlDataRecordStorageSettings = context.DataRecordStorage.Settings as SQLDataRecordStorageSettings;
            if (sqlDataRecordStorageSettings == null)
                return null;
            return new SQLRecordStorageDataManager(context.DataStore.Settings as SQLDataStoreSettings, 
                context.DataRecordStorage.Settings as SQLDataRecordStorageSettings, 
                context.DataRecordStorage,
                context.SummaryTransformationDefinition);
        }

        public override Guid ConfigId
        {
            get { return new Guid("2AEEC2DE-EC44-4698-AAEF-8E9DBF669D1E"); }
        }
    }
}
