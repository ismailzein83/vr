using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using TOne.Data;

namespace TOne.Business
{
    public class SecurityManager
    {
        public AuthenticationOutput Authenticate(string Email, string password)
        {
            SecurityEssentials.User user = SecurityEssentials.User.Authenticate(Email, SecurityEssentials.PasswordEncryption.Encode(password));

            return new AuthenticationOutput
            {
                Result = user.IsAuthenticated ? AuthenticationResult.Succeeded : AuthenticationResult.Failed,
                User = user
            };
        }

        public IEnumerable<UserRoleMember> GetUserRoleMembers(int userId)
        {
            GenericEntityManager genericEntityManager = new GenericEntityManager();
            return genericEntityManager.GetEntitiesByLinkedEntity<UserRoleMember>(false, userId);
        }

        public IEnumerable<RolePermission> GetRolePermissions(int roleId)
        {
            GenericEntityManager genericEntityManager = new GenericEntityManager();
            return genericEntityManager.GetEntitiesByOwner<RolePermission>(true, roleId);
        }

        public IEnumerable<RolePermission> GetRolesPermissions(IEnumerable<int> roleIds)
        {
            GenericEntityManager genericEntityManager = new GenericEntityManager();
            return genericEntityManager.GetEntitiesByOwners<RolePermission>(true, roleIds);
        }

        public IEnumerable<T> GetRolesPermissions<T>(IEnumerable<int> roleIds) where T : RolePermission
        {
            GenericEntityManager genericEntityManager = new GenericEntityManager();
            return genericEntityManager.GetEntitiesByOwners<T>(false, roleIds);
        }

        public IEnumerable<RolePermission> GetUserPermissions(int userId)
        {
            var userRoles = GetUserRoleMembers(userId);
            if (userRoles != null)
                return GetRolesPermissions(userRoles.Select(itm => itm.RoleId));
            else
                return null;
        }

        public IEnumerable<T> GetUserPermissions<T>(int userId) where T : RolePermission
        {
            var userRoles = GetUserRoleMembers(userId);
            if (userRoles != null)
                return GetRolesPermissions<T>(userRoles.Select(itm => itm.RoleId));
            else
                return null;
        }

 
    }
}
