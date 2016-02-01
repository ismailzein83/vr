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
        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitions()
        {
            return GetItemsSP("genericdata.GetGenericRuleDefinitions", GenericRuleDefinitionMapper);
        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericRuleDefinition", ref updateHandle);
        }

        #region Mappers

        GenericRuleDefinition GenericRuleDefinitionMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<GenericRuleDefinition>((string)reader["Details"]);
        }

        #endregion
    }
}
