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
    public class DataStoreDataManager : BaseSQLDataManager, IDataStoreDataManager
    {
        public DataStoreDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods

        public IEnumerable<DataStore> GetDataStores()
        {
            return GetItemsSP("genericdata.sp_DataStore_GetAll", DataStoreMapper);
        }

        public bool AddDataStore(DataStore dataStore) 
        {

            int affectedRows = ExecuteNonQuerySP("genericdata.sp_DataStore_Insert", dataStore.DataStoreId, dataStore.Name, Vanrise.Common.Serializer.Serialize(dataStore.Settings));
            return (affectedRows == 1);
        }
        public bool UpdateDataStore(DataStore dataStore)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_DataStore_Update", dataStore.DataStoreId, dataStore.Name, Vanrise.Common.Serializer.Serialize(dataStore.Settings));
            return (affectedRows == 1);
        }
        public bool AreDataStoresUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataStore", ref updateHandle);
        }

        #endregion

        #region Mappers

        DataStore DataStoreMapper(IDataReader reader)
        {
            return new DataStore()
            {
                DataStoreId =  GetReaderValue<Guid>(reader,"ID"),
                Name = (string)reader["Name"],
                Settings = Vanrise.Common.Serializer.Deserialize<DataStoreSettings>((string)reader["Settings"])
            };
        }
        
        #endregion
    }
}
