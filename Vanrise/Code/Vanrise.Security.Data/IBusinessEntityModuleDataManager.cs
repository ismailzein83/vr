using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IBusinessEntityModuleDataManager : IDataManager
    {
        IEnumerable<Vanrise.Security.Entities.BusinessEntityModule> GetModules();

        bool ToggleBreakInheritance(string entityId);
        bool AddBusinessEntityModule(BusinessEntityModule moduleObject, out int moduleId);
        bool UpdateBusinessEntityModule(BusinessEntityModule moduleObject);
        bool UpdateBusinessEntityModuleRank(int moduleId, int? parentId);
        bool AreBusinessEntityModulesUpdated(ref object updateHandle);
    }
}
