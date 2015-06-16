using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class RoleManager
    {
        public List<Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            return dataManager.GetFilteredRoles(fromRow, toRow, name);
        }

        public Role GetRole(int roleId)
        {
            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            return dataManager.GetRole(roleId);
        }

        public List<Role> GetRoles()
        {
            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            return dataManager.GetRoles();
        }

        public List<int> GetUserRoles(int userId)
        {
            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            return dataManager.GetUserRoles(userId);
        }

        public Vanrise.Entities.InsertOperationOutput<Role> AddRole(Role roleObject, int[] members)
        {
            InsertOperationOutput<Role> insertOperationOutput = new InsertOperationOutput<Role>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int roleId = -1;

            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            bool insertActionSucc = dataManager.AddRole(roleObject, out roleId);

            if(insertActionSucc)
            {
                dataManager.AssignMembers(roleId, members);
                
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                roleObject.RoleId = roleId;
                insertOperationOutput.InsertedObject = roleObject;
            }
            
            return insertOperationOutput;
        }


        public Vanrise.Entities.UpdateOperationOutput<Role> UpdateRole(Role roleObject, int[] members)
        {
            IRoleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRoleDataManager>();
            bool updateActionSucc = dataManager.UpdateRole(roleObject);
            UpdateOperationOutput<Role> updateOperationOutput = new UpdateOperationOutput<Role>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                dataManager.AssignMembers(roleObject.RoleId, members);

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = roleObject;
            }
            return updateOperationOutput;
        }

    }
}
