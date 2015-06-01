using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class RoleManager
    {
        public List<Role> GetRoles()
        {
            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            return dataManager.GetRoles();
        }
    }
}
