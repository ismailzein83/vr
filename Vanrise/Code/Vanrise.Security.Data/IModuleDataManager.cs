using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IModuleDataManager : IDataManager
    {
        List<Vanrise.Security.Entities.Module> GetModules();
        bool UpdateModuleRank(Guid moduleId, Guid? parentId, int rank);

        bool AddModule(Module moduleObject);
        bool UpdateModule(Module moduleObject);
        bool AreModulesUpdated(ref object _updateHandle);
    }
}
