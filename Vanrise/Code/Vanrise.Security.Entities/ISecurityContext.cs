using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public interface ISecurityContext
    {
        int GetLoggedInUserId();

        bool TryGetLoggedInUserId(out int? userId);

        bool IsAllowed(string requiredPermissions);

        bool IsAllowed(string requiredPermissions,int userId);

        bool IsAllowed(RequiredPermissionSettings requiredPermissions ,int userId);

        bool HasPermissionToActions(string systemActionNames);

        void SetContextUserId(int userId);
    }
}
