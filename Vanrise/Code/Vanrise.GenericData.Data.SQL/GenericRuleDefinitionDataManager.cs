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
    public class GenericRuleDefinitionDataManager : BaseSQLDataManager, IGenericRuleDefinitionDataManager
    {
        public GenericRuleDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        
        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitions()
        {
            return GetItemsSP("genericdata.sp_GenericRuleDefinition_GetAll", GenericRuleDefinitionMapper);
        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericRuleDefinition", ref updateHandle);
        }
        
        #endregion

        #region Mappers

        GenericRuleDefinition GenericRuleDefinitionMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<GenericRuleDefinition>((string)reader["Details"]);
        }

        #endregion
    }
}
