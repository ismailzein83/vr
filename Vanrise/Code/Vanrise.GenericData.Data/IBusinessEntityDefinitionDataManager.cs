using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IBusinessEntityDefinitionDataManager : IDataManager
    {
        IEnumerable<BusinessEntityDefinition> GetBusinessEntityDefinitions();
        bool UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition);
        bool AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition);
        bool AreGenericRuleDefinitionsUpdated(ref object updateHandle);
    }
}
