using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.FileDataStorage
{
    public class FileDataStoreSettings : DataStoreSettings
    {
        public override Guid ConfigId => new Guid("DB1224D0-CC3D-481D-9163-E4E7C5231D00");

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
            throw new NotImplementedException();
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
        }
    }
}
