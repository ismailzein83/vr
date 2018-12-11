using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBDataStoreSettings : DataStoreSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F8DD5B05-7F69-4F16-BFC6-D65EF9B65BF8"); }
        }

        public string ModuleName { get; set; }

        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }

        /// <summary>
        /// either ConnectionString or ConnectionStringName or ConnectionStringAppSettingName should have value.
        /// </summary>
        public string ConnectionStringAppSettingName { get; set; }

        public override void CreateTempStorage(ICreateTempStorageContext context)
        {
            var rdbDataStoreSettings = context.DataStore.Settings.CastWithValidate<RDBDataStoreSettings>("context.DataStore.Settings", context.DataStore.DataStoreId);
            var rdbDataRecordStorageSettings = context.DataRecordStorage.Settings.CastWithValidate<RDBDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId);

            RDBRecordStorageDataManager dataManager = new RDBRecordStorageDataManager(rdbDataStoreSettings, rdbDataRecordStorageSettings, context.DataRecordStorage, null);
            context.TempStorageInformation = dataManager.CreateTempRDBRecordStorageTable(context.ProcessId);
        }

        public override void DropStorage(IDropStorageContext context)
        {
            var rdbDataStoreSettings = context.DataStore.Settings.CastWithValidate<RDBDataStoreSettings>("context.DataStore.Settings", context.DataStore.DataStoreId);
            var rdbDataRecordStorageSettings = context.DataRecordStorage.Settings.CastWithValidate<RDBDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId);

            RDBTempStorageInformation rdbTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                rdbTempStorageInformation = context.TempStorageInformation.CastWithValidate<RDBTempStorageInformation>("context.TempStorageInformation");

            RDBRecordStorageDataManager dataManager = new RDBRecordStorageDataManager(rdbDataStoreSettings, rdbDataRecordStorageSettings, context.DataRecordStorage, rdbTempStorageInformation);
            dataManager.DropStorage();
        }

        public override void FillDataRecordStorageFromTempStorage(IFillDataRecordStorageFromTempStorageContext context)
        {
            var rdbDataStoreSettings = context.DataStore.Settings.CastWithValidate<RDBDataStoreSettings>("context.DataStore.Settings", context.DataStore.DataStoreId);
            var rdbDataRecordStorageSettings = context.DataRecordStorage.Settings.CastWithValidate<RDBDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId);
            var rdbTempStorageInformation = context.TempStorageInformation.CastWithValidate<RDBTempStorageInformation>("context.TempStorageInformation", context.DataRecordStorage.DataRecordStorageId);

            RDBRecordStorageDataManager dataManager = new RDBRecordStorageDataManager(rdbDataStoreSettings, rdbDataRecordStorageSettings, context.DataRecordStorage, rdbTempStorageInformation);
            dataManager.FillDataRecordStorageFromTempStorage(context.From, context.To, context.RecordFilterGroup);
        }

        public override IDataRecordDataManager GetDataRecordDataManager(IGetRecordStorageDataManagerContext context)
        {
            RDBTempStorageInformation rdbTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                rdbTempStorageInformation = context.TempStorageInformation.CastWithValidate<RDBTempStorageInformation>("rdbTempStorageInformation");

            return new RDBRecordStorageDataManager(context.DataStore.Settings.CastWithValidate<RDBDataStoreSettings>("context.DataStore.Settings", context.DataStore.DataStoreId), context.DataRecordStorage.Settings.CastWithValidate<RDBDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId),
                                                    context.DataRecordStorage, rdbTempStorageInformation);
        }

        public override int GetStorageRowCount(IGetStorageRowCountContext context)
        {
            var rdbDataStoreSettings = context.DataStore.Settings.CastWithValidate<RDBDataStoreSettings>("context.DataStore.Settings", context.DataStore.DataStoreId);
            var rdbDataRecordStorageSettings = context.DataRecordStorage.Settings.CastWithValidate<RDBDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId);


            RDBTempStorageInformation rdbTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                rdbTempStorageInformation = context.TempStorageInformation.CastWithValidate<RDBTempStorageInformation>("context.TempStorageInformation");

            RDBRecordStorageDataManager dataManager = new RDBRecordStorageDataManager(rdbDataStoreSettings, rdbDataRecordStorageSettings, context.DataRecordStorage, rdbTempStorageInformation);
            return dataManager.GetStorageRowCount();
        }

        public override ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context)
        {
            var rdbDataStoreSettings = context.DataStore.Settings.CastWithValidate<RDBDataStoreSettings>("context.DataStore.Settings", context.DataStore.DataStoreId);
            var rdbDataRecordStorageSettings = context.DataRecordStorage.Settings.CastWithValidate<RDBDataRecordStorageSettings>("context.DataRecordStorage.Settings", context.DataRecordStorage.DataRecordStorageId);
            
            RDBTempStorageInformation rdbTempStorageInformation = null;
            if (context.TempStorageInformation != null)
                rdbTempStorageInformation = context.TempStorageInformation.CastWithValidate<RDBTempStorageInformation>("context.TempStorageInformation", context.DataRecordStorage.DataRecordStorageId);

            return new RDBRecordStorageDataManager(rdbDataStoreSettings, rdbDataRecordStorageSettings,
                                                    context.DataRecordStorage, context.SummaryTransformationDefinition, rdbTempStorageInformation);
        }

        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
