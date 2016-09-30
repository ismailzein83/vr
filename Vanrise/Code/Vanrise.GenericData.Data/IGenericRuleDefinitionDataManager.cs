using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IGenericRuleDefinitionDataManager : IDataManager
    {
        IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitions();
        bool AreGenericRuleDefinitionsUpdated(ref object updateHandle);
        bool AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition);
        bool UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition);
    }
}
