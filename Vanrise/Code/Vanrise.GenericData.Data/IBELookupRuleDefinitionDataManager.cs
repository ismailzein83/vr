using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IBELookupRuleDefinitionDataManager : IDataManager
    {
        IEnumerable<BELookupRuleDefinition> GetBELookupRuleDefinitions();

        bool AreBELookupRuleDefinitionsUpdated(ref object updateHandle);

        bool InsertBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition);

        bool UpdateBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition);
    }
}
