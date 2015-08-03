using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Data
{
    public interface IModuleDataManager : IDataManager
    {
        List<Vanrise.Security.Entities.Module> GetModules();
        bool UpdateModuleRank(int moduleId, int rank);
    }
}
