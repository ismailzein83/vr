using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.Data.SQL
{
    public class DataTransformationStepConfigDataManager:BaseSQLDataManager,IDataTransformationStepConfigDataManager
    {
        //public DataTransformationStepConfigDataManager()
        //    : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        //{

        //}
        //#region Public Methods
        //public List<Entities.DataTransformationStepConfig> GetDataTransformationSteps()
        //{
        //    return GetItemsSP("genericdata.sp_DataTransformationStepConfig_GetAll", DataTransformationStepConfigMapper);
        //}
        //public bool AreDataTransformationStepConfigUpdated(ref object updateHandle)
        //{
        //    return base.IsDataUpdated("genericdata.DataTransformationStepConfig", ref updateHandle);
        //}
        //#endregion

        //#region Mappers

        //DataTransformationStepConfig DataTransformationStepConfigMapper(IDataReader reader)
        //{
        //    DataTransformationStepConfig dataTransformationStepConfig = Vanrise.Common.Serializer.Deserialize<DataTransformationStepConfig>(reader["Details"] as string);
        //    if (dataTransformationStepConfig != null)
        //    {
        //        dataTransformationStepConfig.DataTransformationStepConfigId = Convert.ToInt32(reader["ID"]);
        //        dataTransformationStepConfig.Name = reader["Name"] as string;
        //    }
        //    return dataTransformationStepConfig;
        //}

        //#endregion


       
    }
}
