using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.SQLDataStorage
{
    public class SQLDataStoreSettings : DataStoreSettings
    {
        public override Guid ConfigId { get { return new Guid("2AEEC2DE-EC44-4698-AAEF-8E9DBF669D1E"); } }

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

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlRecordStorageSettings, context.RecordStorage, null);

            if (existingRecordStorageSettings == null)
                dataManager.CreateSQLRecordStorageTable();
            else
                dataManager.AlterSQLRecordStorageTable(existingRecordStorageSettings);
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            SQLTempStorageInformation sqlTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                sqlTempStorageInformation = context.TempStorageInformation.CastWithValidate<SQLTempStorageInformation>("sqlTempStorageInformation");

            return new SQLRecordStorageDataManager(context.DataStore.Settings as SQLDataStoreSettings, context.DataRecordStorage.Settings as SQLDataRecordStorageSettings,
                                                    context.DataRecordStorage, sqlTempStorageInformation);
        }

        public override ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context)
        {
            var sqlDataRecordStorageSettings = context.DataRecordStorage.Settings as SQLDataRecordStorageSettings;
            if (sqlDataRecordStorageSettings == null)
                return null;

            SQLTempStorageInformation sqlTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                sqlTempStorageInformation = context.TempStorageInformation as SQLTempStorageInformation;

            return new SQLRecordStorageDataManager(context.DataStore.Settings as SQLDataStoreSettings, context.DataRecordStorage.Settings as SQLDataRecordStorageSettings,
                                                    context.DataRecordStorage, context.SummaryTransformationDefinition, sqlTempStorageInformation);
        }

        public override void CreateTempStorage(ICreateTempStorageContext context)
        {
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;
            var sqlDataRecordStorageSettings = context.DataRecordStorage.Settings as SQLDataRecordStorageSettings;

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlDataRecordStorageSettings, context.DataRecordStorage, null);
            context.TempStorageInformation = dataManager.CreateTempSQLRecordStorageTable(context.ProcessId);
        }

        public override void FillDataRecordStorageFromTempStorage(IFillDataRecordStorageFromTempStorageContext context)
        {
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;
            var sqlDataRecordStorageSettings = context.DataRecordStorage.Settings as SQLDataRecordStorageSettings;
            var sqlTempStorageInformation = context.TempStorageInformation as SQLTempStorageInformation;

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlDataRecordStorageSettings, context.DataRecordStorage, sqlTempStorageInformation);
            dataManager.FillDataRecordStorageFromTempStorage(context.From, context.To, context.RecordFilterGroup);
        }

        public override void DropStorage(IDropStorageContext context)
        {
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;
            var sqlDataRecordStorageSettings = context.DataRecordStorage.Settings as SQLDataRecordStorageSettings;

            SQLTempStorageInformation sqlTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                sqlTempStorageInformation = context.TempStorageInformation as SQLTempStorageInformation;

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlDataRecordStorageSettings, context.DataRecordStorage, sqlTempStorageInformation);
            dataManager.DropStorage();
        }

        public override int GetStorageRowCount(IGetStorageRowCountContext context)
        {
            var sqlDataStoreSettings = context.DataStore.Settings as SQLDataStoreSettings;
            var sqlDataRecordStorageSettings = context.DataRecordStorage.Settings as SQLDataRecordStorageSettings;

            SQLTempStorageInformation sqlTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                sqlTempStorageInformation = context.TempStorageInformation as SQLTempStorageInformation;

            SQLRecordStorageDataManager dataManager = new SQLRecordStorageDataManager(sqlDataStoreSettings, sqlDataRecordStorageSettings, context.DataRecordStorage, sqlTempStorageInformation);
            return dataManager.GetStorageRowCount();
        }
    }
}
