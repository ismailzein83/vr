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
    public class PermissionManager
    {
        public List<BusinessEntityNode> GetEntityNodes()
        {
            //TODO: pass the holder id to load the saved permissions
            IBusinessEntityModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
            List<BusinessEntityModule> modules = moduleDataManager.GetModules();

            IBusinessEntityDataManager entityDataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            List<BusinessEntity> entities = entityDataManager.GetEntities();

            List<BusinessEntityNode> retVal = new List<BusinessEntityNode>();

            foreach (BusinessEntityModule item in modules)
            {
                if (item.Parent == 0)
                {
                    retVal.Add(GetModuleNode(item, modules, entities, null));
                }
            }

            return retVal;
        }

        public List<Permission> GetPermissions()
        {
            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            return dataManager.GetPermissions();
        }

        public List<Permission> GetPermissions(int holderType, string holderId)
        {
            HolderType paramHolderType = (holderType == 0) ? HolderType.USER : HolderType.ROLE;

            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            return dataManager.GetPermissions(paramHolderType, holderId);
        }

        public Vanrise.Entities.UpdateOperationOutput<object> UpdatePermissions(IEnumerable<Permission> permissions)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = null;

            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();

            foreach (Permission item in permissions)
            {
                if (item.PermissionFlags.Count == 0)
                {
                    if(!dataManager.DeletePermission(item))
                    {
                        updateOperationOutput.Result = UpdateOperationResult.Failed;
                    }
                }
                else if (!dataManager.UpdatePermission(item))
                {
                    updateOperationOutput.Result = UpdateOperationResult.Failed;
                }
            }

            return updateOperationOutput;
        }

        private BusinessEntityNode GetModuleNode(BusinessEntityModule module, List<BusinessEntityModule> modules, List<BusinessEntity> entities, BusinessEntityNode parent)
        {
            BusinessEntityNode node = new BusinessEntityNode()
            {
                EntityId = module.ModuleId,
                Name = module.Name,
                EntType = EntityType.MODULE,
                PermissionOptions = module.PermissionOptions,
                Parent = parent
            };

            List<BusinessEntityModule> subModules = modules.FindAll(x => x.Parent == module.ModuleId);

            List<BusinessEntity> childEntities = entities.FindAll(x => x.ModuleId == module.ModuleId);

            if (childEntities.Count > 0)
            {
                node.Children = new List<BusinessEntityNode>();
                foreach (BusinessEntity entityItem in childEntities)
                {
                    BusinessEntityNode entityNode = new BusinessEntityNode()
                    {
                        EntityId = entityItem.EntityId,
                        Name = entityItem.Name,
                        EntType = EntityType.ENTITY,
                        PermissionOptions = entityItem.PermissionOptions,
                        Parent = node
                    };

                    node.Children.Add(entityNode);
                }
            }

            if (subModules.Count > 0)
            {
                if (node.Children == null)
                    node.Children = new List<BusinessEntityNode>();

                foreach (BusinessEntityModule item in subModules)
                {
                    node.Children.Add(GetModuleNode(item, modules, entities, node));
                }
            }

            return node;
        }
    }
}
