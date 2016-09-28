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

        public bool AddDataRecordStorage(DataRecordStorage dataRecordStorage, out int insertedId)
        {
            object dataRecordStorageId;
            int affectedRows = ExecuteNonQuerySP
                (
                    "genericdata.sp_DataRecordStorage_Insert",
                    out dataRecordStorageId,
                    dataRecordStorage.Name,
                    dataRecordStorage.DataRecordTypeId,
                    dataRecordStorage.DataStoreId,
                    Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings),
                    DateTime.Now
                );
            
            if (affectedRows == 1) {
                insertedId = (int)dataRecordStorageId;
                return true;
            }
            else {
                insertedId = -1;
                return false;
            }
        }

        public bool UpdateDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            int affectedRows = ExecuteNonQuerySP
                (
                    "genericdata.sp_DataRecordStorage_Update",
                    dataRecordStorage.DataRecordStorageId,
                    dataRecordStorage.Name,
                    dataRecordStorage.DataRecordTypeId,
                    dataRecordStorage.DataStoreId,
                    Vanrise.Common.Serializer.Serialize(dataRecordStorage.Settings)
                );
            return (affectedRows == 1);
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
                DataRecordTypeId = GetReaderValue<Guid>(reader,"DataRecordTypeID"),
                DataStoreId = (int)reader["DataStoreID"],
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordStorageSettings>((string)reader["Settings"]),
                State = (GetReaderValue<string>(reader, "State") != null) ? Vanrise.Common.Serializer.Deserialize<DataRecordStorageState>((string)reader["State"]) : null
            };
        }

        #endregion
    }
}
