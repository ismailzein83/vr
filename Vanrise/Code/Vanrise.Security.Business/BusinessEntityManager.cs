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
    public class BusinessEntityManager
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
                if (item.ParentId == 0)
                {
                    retVal.Add(GetModuleNode(item, modules, entities, null));
                }
            }

            return retVal;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ToggleBreakInheritance(EntityType entityType, string entityId)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();
            updateOperationOutput.UpdatedObject = null;

            bool result = false;

            if (entityType == EntityType.MODULE)
            {
                IBusinessEntityModuleDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
                result = manager.ToggleBreakInheritance(entityId);
            }
            else
            {
                IBusinessEntityDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
                result = manager.ToggleBreakInheritance(entityId);
            }

            updateOperationOutput.Result = (result) ? UpdateOperationResult.Succeeded : UpdateOperationResult.Failed;

            return updateOperationOutput;
        }

        private BusinessEntityNode GetModuleNode(BusinessEntityModule module, List<BusinessEntityModule> modules, List<BusinessEntity> entities, BusinessEntityNode parent)
        {
            BusinessEntityNode node = new BusinessEntityNode()
            {
                EntityId = module.ModuleId,
                Name = module.Name,
                EntType = EntityType.MODULE,
                BreakInheritance = module.BreakInheritance,
                PermissionOptions = module.PermissionOptions,
                Parent = parent
            };

            List<BusinessEntityModule> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

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
                        BreakInheritance = entityItem.BreakInheritance,
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
