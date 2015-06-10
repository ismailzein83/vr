using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IPermissionDataManager : IDataManager
    {
        List<Permission> GetPermissions();

        List<Permission> GetPermissions(HolderType holderType, string holderId);

        bool UpdatePermission(Permission permission);

        bool DeletePermission(Permission permission);
    }
}
