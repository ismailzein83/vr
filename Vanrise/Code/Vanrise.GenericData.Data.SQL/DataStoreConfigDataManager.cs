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
    public class DataStoreConfigDataManager : BaseSQLDataManager, IDataStoreConfigDataManager
    {
        //public DataStoreConfigDataManager()
        //    : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        //{

        //}

        //#region Public Methods
        //public List<DataStoreConfig> GetDataStoreConfigs()
        //{
        //    return GetItemsSP("genericdata.sp_DataStoreConfig_GetAll", DataStoreConfigMapper);
        //}
        //public bool AreDataStoreConfigUpdated(ref object updateHandle)
        //{
        //    return base.IsDataUpdated("genericdata.DataStoreConfig", ref updateHandle);
        //}

        //#endregion

        //#region Mappers

        //DataStoreConfig DataStoreConfigMapper(IDataReader reader)
        //{
        //    DataStoreConfig dataStoreConfig = Vanrise.Common.Serializer.Deserialize<DataStoreConfig>(reader["Details"] as string);
        //    if (dataStoreConfig != null)
        //    {
        //        dataStoreConfig.DataStoreConfigId = Convert.ToInt32(reader["ID"]);
        //        dataStoreConfig.Name = reader["Name"] as string;
        //    }
        //    return dataStoreConfig;
        //}

        //#endregion
    }
}
