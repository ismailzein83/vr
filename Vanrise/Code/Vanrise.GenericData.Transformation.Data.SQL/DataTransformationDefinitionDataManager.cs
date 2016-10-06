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
    public class DataTransformationDefinitionDataManager : BaseSQLDataManager, IDataTransformationDefinitionDataManager
    {
        public DataTransformationDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
   
        #region Public Methods
        public List<DataTransformationDefinition> GetDataTransformationDefinitions()
        {
            return GetItemsSP("genericdata.sp_DataTransformationDefinition_GetAll", DataTransformationDefinitionMapper);
        }
        public bool AreDataTransformationDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataTransformationDefinition", ref updateHandle);
        }
        public bool UpdateDataTransformationDefinition(Entities.DataTransformationDefinition dataTransformationDefinition)
        {
            string serializedObj = null;
            if (dataTransformationDefinition != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(dataTransformationDefinition);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_DataTransformationDefinition_Update", dataTransformationDefinition.DataTransformationDefinitionId, dataTransformationDefinition.Name, dataTransformationDefinition.Title, serializedObj);
            return (recordesEffected > 0);
        }
        public bool AddDataTransformationDefinition(Entities.DataTransformationDefinition dataTransformationDefinition)
        {
            string serializedObj = null;
            if (dataTransformationDefinition != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(dataTransformationDefinition);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_DataTransformationDefinition_Insert", dataTransformationDefinition.DataTransformationDefinitionId, dataTransformationDefinition.Name, dataTransformationDefinition.Title, serializedObj);

            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers
        DataTransformationDefinition DataTransformationDefinitionMapper(IDataReader reader)
        {
            DataTransformationDefinition dataTransformationDefinition =  Vanrise.Common.Serializer.Deserialize<DataTransformationDefinition>(reader["Details"] as string);
            if(dataTransformationDefinition != null)
            {
                dataTransformationDefinition.DataTransformationDefinitionId = GetReaderValue<Guid>(reader,"ID");
                dataTransformationDefinition.Name = reader["Name"] as string;
                dataTransformationDefinition.Title = reader["Title"] as string;
            }
            return dataTransformationDefinition;
        }

        #endregion
    }
}
