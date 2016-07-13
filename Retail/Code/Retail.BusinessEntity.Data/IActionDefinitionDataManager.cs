using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IActionDefinitionDataManager:IDataManager
    {
        bool AreActionDefinitionUpdated(ref object updateHandle);
        bool Insert(ActionDefinition actionDefinition);
        bool Update(ActionDefinition actionDefinition);
        IEnumerable<ActionDefinition> GetActionDefinitions();
    }
}
