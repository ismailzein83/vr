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
            throw new NotImplementedException();
        }

        public override void DropStorage(IDropStorageContext context)
        {
            throw new NotImplementedException();
        }

        public override void FillDataRecordStorageFromTempStorage(IFillDataRecordStorageFromTempStorageContext context)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override ISummaryRecordDataManager GetSummaryDataRecordDataManager(IGetSummaryRecordStorageDataManagerContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
