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
    public class DataRecordFieldTypeConfigDataManager:BaseSQLDataManager,IDataRecordFieldTypeConfigDataManager
    {
        //public DataRecordFieldTypeConfigDataManager()
        //    : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        //{

        //}

        //#region Public Methods
        //public List<DataRecordFieldTypeConfig> GetDataRecordFieldTypes()
        //{
        //    return GetItemsSP("genericdata.sp_DataRecordFieldTypeConfig_GetAll", DataRecordFieldTypeConfigMapper);
        //}
        //public bool AreDataRecordFieldTypeConfigUpdated(ref object updateHandle)
        //{
        //    return base.IsDataUpdated("genericdata.DataRecordFieldTypeConfig", ref updateHandle);
        //}
       
        //#endregion

        //#region Mappers

        //DataRecordFieldTypeConfig DataRecordFieldTypeConfigMapper(IDataReader reader)
        //{
        //    DataRecordFieldTypeConfig dataRecordFieldTypeConfig = Vanrise.Common.Serializer.Deserialize<DataRecordFieldTypeConfig>(reader["Details"] as string);
        //    if(dataRecordFieldTypeConfig !=null)
        //    {
        //        dataRecordFieldTypeConfig.DataRecordFieldTypeConfigId = Convert.ToInt32(reader["ID"]);
        //        dataRecordFieldTypeConfig.Name = reader["Name"] as string;
        //    }  
        //    return dataRecordFieldTypeConfig;
        ////}

        //#endregion
  

       
    }
}
