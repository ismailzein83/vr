using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public interface ISecurityManager : IBEManager
    {
        string RequiredPermissionsToString(List<RequiredPermissionEntry> requirePermissions);
    }
}
