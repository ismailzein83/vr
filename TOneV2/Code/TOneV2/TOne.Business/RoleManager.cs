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

        public List<TOne.Entities.Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            return datamanager.GetFilteredRoles(fromRow, toRow, name);
        }

        public Role GetRole(int roleId)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            return datamanager.GetRole(roleId);
        }

        public TOne.Entities.OperationResults.InsertOperationOutput<Role> AddRole(Role roleObject)
        {
            TOne.Entities.OperationResults.InsertOperationOutput<Role> insertOperationOutput = new TOne.Entities.OperationResults.InsertOperationOutput<Role>();

            insertOperationOutput.Result = TOne.Entities.OperationResults.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int RoleId = -1;

            IRoleDataManager dataManager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            bool insertActionSucc = dataManager.AddRole(roleObject, out RoleId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = TOne.Entities.OperationResults.InsertOperationResult.Succeeded;
                roleObject.RoleId = RoleId;
                insertOperationOutput.InsertedObject = roleObject;
            }
            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<Role> UpdateRole(Role roleObject)
        {
            IRoleDataManager dataManager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            bool updateActionSucc = dataManager.UpdateRole(roleObject);
            TOne.Entities.UpdateOperationOutput<Role> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<Role>();

            updateOperationOutput.Result = TOne.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = TOne.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = roleObject;
            }
            return updateOperationOutput;
        }

        public void DeleteRole(int id)
        {
            IRoleDataManager datamanager = DataManagerFactory.GetDataManager<IRoleDataManager>();
            datamanager.DeleteRole(id);
        }
    }
}
