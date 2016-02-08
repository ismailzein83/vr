using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class DataRecordStorageDataManager : BaseSQLDataManager, IDataRecordStorageDataManager
    {
        public DataRecordStorageDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods

        public IEnumerable<DataRecordStorage> GetDataRecordStorages()
        {
            return GetItemsSP("genericdata.sp_DataRecordStorage_GetALL", DataRecordStorageMapper);
        }

        public bool AreDataRecordStoragesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataRecordStorage", ref updateHandle);
        }

        #endregion

        #region Mappers

        DataRecordStorage DataRecordStorageMapper(IDataReader reader)
        {
            return new DataRecordStorage() {
                DataRecordStorageId = (int)reader["ID"],
                Name = (string)reader["Name"],
                DataRecordTypeId = (int)reader["DataRecordTypeID"],
                DataStoreId = (int)reader["DataStoreID"],
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordStorageSettings>((string)reader["Settings"])
            };
        }

        #endregion
    }
}
