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

        public bool AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {

            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericRuleDefinition_Insert", genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition.Name, Vanrise.Common.Serializer.Serialize(genericRuleDefinition));

            return (affectedRows == 1);
        }

        public bool UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericRuleDefinition_Update", genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition.Name, Vanrise.Common.Serializer.Serialize(genericRuleDefinition));
            return (affectedRows == 1);
        }

        #endregion

        #region Mappers

        GenericRuleDefinition GenericRuleDefinitionMapper(IDataReader reader)
        {
            // The Details column doesn't allow null values
            GenericRuleDefinition details = Vanrise.Common.Serializer.Deserialize<GenericRuleDefinition>(reader["Details"] as string);
            return new GenericRuleDefinition()
            {
                GenericRuleDefinitionId =  GetReaderValue<Guid>(reader,"ID"),
                Name = (string)reader["Name"],
                Title = details.Title,
                CriteriaDefinition = details.CriteriaDefinition,
                SettingsDefinition = details.SettingsDefinition,
                Objects = details.Objects,
                Security = details.Security
            };
        }

        #endregion
    }
}
