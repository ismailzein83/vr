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
    public class BusinessEntityDefinitionDataManager : BaseSQLDataManager, IBusinessEntityDefinitionDataManager
    {
        public BusinessEntityDefinitionDataManager() : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey")) { }

        #region Public Methods
        public IEnumerable<BusinessEntityDefinition> GetBusinessEntityDefinitions()
        {
            return GetItemsSP("genericdata.sp_BusinessEntityDefinition_GetAll", BusinessEntityDefinitionMapper);
        }
        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.BusinessEntityDefinition", ref updateHandle);
        }
        public bool UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            string serializedObj = null;
            if (businessEntityDefinition != null && businessEntityDefinition.Settings != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(businessEntityDefinition.Settings);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityDefinition_Update", businessEntityDefinition.BusinessEntityDefinitionId, businessEntityDefinition.Name, businessEntityDefinition.Title, serializedObj);
            return (recordesEffected > 0);
        }

        public bool AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            string serializedObj = null;
            if (businessEntityDefinition != null && businessEntityDefinition.Settings != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(businessEntityDefinition.Settings);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_BusinessEntityDefinition_Insert", businessEntityDefinition.BusinessEntityDefinitionId, businessEntityDefinition.Name, businessEntityDefinition.Title, serializedObj);


            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers

        BusinessEntityDefinition BusinessEntityDefinitionMapper(IDataReader reader)
        {
            return new BusinessEntityDefinition()
            {
                BusinessEntityDefinitionId = GetReaderValue<Guid>( reader,"ID"),
                Name = (string)reader["Name"],
                Title = (string)reader["Title"],
                Settings = Vanrise.Common.Serializer.Deserialize<BusinessEntityDefinitionSettings>((string)reader["Settings"])
            };
        }
        
        #endregion


       
    }
}
