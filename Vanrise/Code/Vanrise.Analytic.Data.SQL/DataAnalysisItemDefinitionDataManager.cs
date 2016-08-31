using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class DataAnalysisItemDefinitionDataManager : BaseSQLDataManager, IDataAnalysisItemDefinitionDataManager
    {
        #region ctor/Local Variables
        public DataAnalysisItemDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion


        #region Public Methods

        public List<DataAnalysisItemDefinition> GetDataAnalysisItemDefinitions()
        {
            return GetItemsSP("Analytic.sp_DataAnalysisItemDefinition_GetAll", DataAnalysisItemDefinitionMapper);
        }

        public bool AreDataAnalysisItemDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Analytic.DataAnalysisItemDefinition", ref updateHandle);
        }

        public bool Insert(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
        {
            string serializedSettings = dataAnalysisItemDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(dataAnalysisItemDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Analytic.sp_DataAnalysisItemDefinition_Insert", dataAnalysisItemDefinitionItem.DataAnalysisItemDefinitionId, dataAnalysisItemDefinitionItem.DataAnalysisDefinitionId, dataAnalysisItemDefinitionItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(DataAnalysisItemDefinition dataAnalysisItemDefinitionItem)
        {
            string serializedSettings = dataAnalysisItemDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(dataAnalysisItemDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Analytic.sp_DataAnalysisItemDefinition_Update", dataAnalysisItemDefinitionItem.DataAnalysisItemDefinitionId, dataAnalysisItemDefinitionItem.DataAnalysisDefinitionId, dataAnalysisItemDefinitionItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        DataAnalysisItemDefinition DataAnalysisItemDefinitionMapper(IDataReader reader)
        {
            DataAnalysisItemDefinition dataAnalysisItemDefinition = new DataAnalysisItemDefinition
            {
                DataAnalysisItemDefinitionId = (Guid) reader["ID"],
                DataAnalysisDefinitionId = (Guid)reader["DataAnalysisDefinitionId"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<DataAnalysisItemDefinitionSettings>(reader["Settings"] as string) 
            };
            return dataAnalysisItemDefinition;
        }

        #endregion
    }
}
