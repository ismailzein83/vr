using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public interface IRequiredPermissionSetManager : IBEManager
    {
        bool IsUserGrantedAllModulePermissionSets(int userId, string moduleName, out List<int> grantedPermissionSetIds);

        bool IsCurrentUserGrantedAllModulePermissionSets(string moduleName, out List<int> grantedPermissionSetIds);
    }
}
