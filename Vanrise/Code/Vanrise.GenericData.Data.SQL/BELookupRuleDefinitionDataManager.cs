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
    public class BELookupRuleDefinitionDataManager : BaseSQLDataManager, IBELookupRuleDefinitionDataManager
    {
        #region Constructors

        public BELookupRuleDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
        
        #endregion

        #region Public Methods

        public IEnumerable<BELookupRuleDefinition> GetBELookupRuleDefinitions()
        {
            return GetItemsSP("genericdata.sp_BELookupRuleDefinition_GetAll", BELookupRuleDefinitionMapper);
        }

        public bool AreBELookupRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[genericdata].[BELookupRuleDefinition]", ref updateHandle);
        }

        public bool InsertBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition, out int insertedId)
        {
            object beLookupRuleDefinitionId;
            string details = Vanrise.Common.Serializer.Serialize(beLookupRuleDefinition, true);

            int affectedRows = ExecuteNonQuerySP("genericdata.sp_BELookupRuleDefinition_Insert", out beLookupRuleDefinitionId, beLookupRuleDefinition.Name, details);

            if (affectedRows > 0)
            {
                insertedId = (int)beLookupRuleDefinitionId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool UpdateBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            string details = Vanrise.Common.Serializer.Serialize(beLookupRuleDefinition, true);
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_BELookupRuleDefinition_Update", beLookupRuleDefinition.BELookupRuleDefinitionId, beLookupRuleDefinition.Name, details);
            return (affectedRows > 0);
        }

        #endregion

        #region Mappers

        BELookupRuleDefinition BELookupRuleDefinitionMapper(IDataReader reader)
        {
            var beLookupRuleDefinition = Vanrise.Common.Serializer.Deserialize<BELookupRuleDefinition>(reader["Details"] as string);
            beLookupRuleDefinition.BELookupRuleDefinitionId = (int)reader["ID"];
            return beLookupRuleDefinition;
        }
        
        #endregion
    }
}
