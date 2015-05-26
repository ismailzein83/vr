using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using TOne.Data;

namespace TOne.Business
{
    public class RoleManager
    {
        public List<TOne.Entities.Role> GetRoles(int fromRow, int toRow)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            return datamanager.GetRoles(fromRow, toRow);
        }

        public void DeleteRole(int Id)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            datamanager.DeleteRole(Id);
        }

        public Role AddRole(Role Role)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            return datamanager.AddRole(Role);
        }

        public bool UpdateRole(Role Role)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            return datamanager.UpdateRole(Role);
        }


        public List<TOne.Entities.Role> SearchRole(string name)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            return datamanager.SearchRole(name);
        }
    }
}
