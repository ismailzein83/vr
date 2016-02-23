using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class BusinessEntityNodeManager
    {
        #region Constructors/Fields

        BusinessEntityManager _beManager = new BusinessEntityManager();

        BusinessEntityModuleManager _beModuleManager = new BusinessEntityModuleManager();
        
        #endregion

        #region Public Methods

        public List<BusinessEntityNode> GetEntityNodes()
        {
            //TODO: pass the holder id to load the saved permissions
            IEnumerable<BusinessEntityModule> modules = _beModuleManager.GetBusinessEntityModules();
            IEnumerable<BusinessEntity> entities = _beManager.GetBusinessEntites();

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
            updateOperationOutput.Result = UpdateOperationResult.Failed;

            if (entityType == EntityType.MODULE)
            {
                IBusinessEntityModuleDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();

                if (manager.ToggleBreakInheritance(entityId))
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    _beModuleManager.SetCacheExpired();
                }
            }
            else
            {
                IBusinessEntityDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();

                if (manager.ToggleBreakInheritance(entityId))
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    _beManager.SetCacheExpired();
                }
            }

            return updateOperationOutput;
        }

        public BusinessEntityNode GetBusinessEntityNode(EntityType entityType, string entityId)
        {
            return GetBusinessEntityNodeRecursively(GetEntityNodes(), entityType, entityId);
        }

        public string GetBusinessEntityNodePath(EntityType entityType, string entityId)
        {
            BusinessEntityNode beNode = GetBusinessEntityNode(entityType, entityId);
            return (beNode != null) ? beNode.GetRelativePath() : null;
        }

        public string GetBusinessEntityNodePath(string nodeName)
        {
            return GetBusinessEntityNodePathRecursively(GetEntityNodes(), nodeName);
        }

        #endregion

        #region Private Methods

        private BusinessEntityNode GetModuleNode(BusinessEntityModule module, IEnumerable<BusinessEntityModule> modules, IEnumerable<BusinessEntity> entities, BusinessEntityNode parent)
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

            IEnumerable<BusinessEntityModule> subModules = modules.FindAllRecords(x => x.ParentId == module.ModuleId);

            IEnumerable<BusinessEntity> childEntities = entities.FindAllRecords(x => x.ModuleId == module.ModuleId);

            if (childEntities.Count() > 0)
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

            if (subModules.Count() > 0)
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

        BusinessEntityNode GetBusinessEntityNodeRecursively(IEnumerable<BusinessEntityNode> beNodes, EntityType targetType, string targetId)
        {
            foreach (var beNode in beNodes)
            {
                if (beNode.EntType == targetType && beNode.EntityId.ToString() == targetId)
                    return beNode;
                else if (beNode.Children != null)
                {
                    BusinessEntityNode targetNode = GetBusinessEntityNodeRecursively(beNode.Children, targetType, targetId);
                    if (targetNode != null) return targetNode;
                }
            }
            return null;
        }

        string GetBusinessEntityNodePathRecursively(IEnumerable<BusinessEntityNode> beNodes, string nodeName)
        {
            foreach (var beNode in beNodes)
            {
                if (beNode.Name == nodeName)
                    return beNode.GetRelativePath();
                else if (beNode.Children != null)
                {
                    string path = GetBusinessEntityNodePathRecursively(beNode.Children, nodeName);
                    if (path != null) return path;
                }
            }
            return null;
        }

        #endregion
    }
}
