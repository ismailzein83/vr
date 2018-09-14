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
                if (item.ParentId == Guid.Empty || item.ParentId == null)
                {
                    retVal.Add(GetModuleNode(item, modules, entities, null, true));
                }
            }

            return retVal;
        }
        public List<BusinessEntityNode> GetEntityModules()
        {
            //TODO: pass the holder id to load the saved permissions
            IEnumerable<BusinessEntityModule> modules = _beModuleManager.GetBusinessEntityModules();

            List<BusinessEntityNode> retVal = new List<BusinessEntityNode>();

            foreach (BusinessEntityModule item in modules)
            {
                if (item.ParentId == Guid.Empty || item.ParentId == null)
                {
                    retVal.Add(GetModuleNode(item, modules, null, null, true));
                }
            }

            return retVal;
        }
        public Vanrise.Entities.UpdateOperationOutput<object> ToggleBreakInheritance(EntityType entityType, string entityId)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();
            updateOperationOutput.UpdatedObject = null;
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            bool havePermission = DosesUserHaveToggleBreakInheritancePermission(entityType, entityId);
            if (!havePermission)
            {
                updateOperationOutput.Message = "you don't have permission to perform this action";
                updateOperationOutput.ShowExactMessage = true;
                return updateOperationOutput;
            }
            if (entityType == EntityType.MODULE)
            {
                IBusinessEntityModuleDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();

                if (manager.ToggleBreakInheritance(new Guid(entityId)))
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    _beModuleManager.SetCacheExpired();
                }
            }
            else
            {
                IBusinessEntityDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();

                if (manager.ToggleBreakInheritance(new Guid(entityId)))
                {
                    updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                    _beManager.SetCacheExpired();
                }
            }

            return updateOperationOutput;
        }

        bool DosesUserHaveToggleBreakInheritancePermission(EntityType entityType, string entityId)
        {
            string nodePath = GetBusinessEntityNodePath(entityType, entityId);
            BusinessEntityNode beNode = GetBusinessEntityNode(entityType, entityId);
            int userId = SecurityContext.Current.GetLoggedInUserId();
            PermissionManager manager = new PermissionManager();
            EffectivePermissionsWrapper effectivePermissionsWrapper = manager.GetEffectivePermissions(userId);
            Dictionary<string, Flag> effectivePermissionFlags;
            List<string> requiredPermissionsToCheck = entityType == EntityType.MODULE ? new List<string>(new string[] { "View", "Add", "Edit", "Delete" }) : beNode.PermissionOptions;

            if (effectivePermissionsWrapper.EffectivePermissions.TryGetValue(nodePath, out effectivePermissionFlags))
            {
                if (entityType == EntityType.MODULE)
                {
                    Flag fullControlFlag;
                    if (effectivePermissionFlags.TryGetValue("Full Control", out fullControlFlag))
                    {
                        if (fullControlFlag != Flag.ALLOW)
                            return false;
                        else
                        {
                            return CheckAllExistingPermissionFlags(effectivePermissionFlags, requiredPermissionsToCheck);
                        }
                    }
                    else return false;
                }
                else
                {
                    return CheckAllExistingPermissionFlags(effectivePermissionFlags, requiredPermissionsToCheck); ;
                }

            }

            else return false;
        }


        bool CheckAllExistingPermissionFlags(Dictionary<string, Flag> effectivePermissionFlags, List<string> requiredPermissionsToCheck)
        {
            foreach (string requiredFlag in requiredPermissionsToCheck)
            {
                Flag effectivePermissionFlag;
                if (effectivePermissionFlags.TryGetValue(requiredFlag, out effectivePermissionFlag))
                {
                    if (effectivePermissionFlag != Flag.ALLOW)
                    {
                        return false;
                    }
                }
                else return false;
            }
            return true;
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

        public Vanrise.Entities.UpdateOperationOutput<List<BusinessEntityNode>> UpdateEntityNodesRank(List<BusinessEntityNode> businessEntityNodes)
        {
            UpdateOperationOutput<List<BusinessEntityNode>> updateOperationOutput = new UpdateOperationOutput<List<BusinessEntityNode>>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc = false;

            updateActionSucc = UpdateEntityNodesChildren(businessEntityNodes);

            MenuManager menuManager = new MenuManager();
            if (updateActionSucc)
            {

                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                List<BusinessEntityNode> updatedBusinessEntityNodes = GetEntityModules();
                updateOperationOutput.UpdatedObject = updatedBusinessEntityNodes;
            }
            return updateOperationOutput;
        }
        public bool UpdateEntityNodesChildren(List<BusinessEntityNode> businessEntityNodes)
        {
            for (int i = 0; i < businessEntityNodes.Count; i++)
            {
                var businessEntityNode = businessEntityNodes[i];
                if (businessEntityNode.EntType == EntityType.MODULE)
                {
                    _beModuleManager.UpdateBusinessEntityModuleRank(businessEntityNode.EntityId, null);
                    PrepareEntitiesAndModulesObjects(businessEntityNode.Children, businessEntityNode);

                }
                else if (businessEntityNode.EntType == EntityType.ENTITY)
                {
                    _beManager.UpdateBusinessEntityRank(businessEntityNode.EntityId, Guid.Empty);
                }
            }
            return true;
        }
        public void PrepareEntitiesAndModulesObjects(List<BusinessEntityNode> children, BusinessEntityNode parent)
        {
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child.EntType == EntityType.ENTITY)
                    {
                        _beManager.UpdateBusinessEntityRank(child.EntityId, parent.EntityId);
                    }
                    else
                    {
                        _beModuleManager.UpdateBusinessEntityModuleRank(child.EntityId, parent.EntityId);
                        PrepareEntitiesAndModulesObjects(child.Children, child);
                    }
                }
            }
        }




        #endregion

        #region Private Methods

        private BusinessEntityNode GetModuleNode(BusinessEntityModule module, IEnumerable<BusinessEntityModule> modules, IEnumerable<BusinessEntity> entities, BusinessEntityNode parent, bool withchildEntities)
        {
            BusinessEntityNode node = new BusinessEntityNode()
            {
                EntityId = module.ModuleId,
                Name = module.Name,
                Title = module.Name,
                EntType = EntityType.MODULE,
                BreakInheritance = module.BreakInheritance,
                PermissionOptions = module.PermissionOptions,
                Parent = parent
            };


            IEnumerable<BusinessEntityModule> subModules = modules.FindAllRecords(x => x.ParentId == module.ModuleId);

            IEnumerable<BusinessEntity> childEntities = null;

            if (entities != null)
                childEntities = entities.FindAllRecords(x => x.ModuleId == module.ModuleId);

            if (childEntities != null && childEntities.Count() > 0 && withchildEntities)
            {
                node.Children = new List<BusinessEntityNode>();
                foreach (BusinessEntity entityItem in childEntities)
                {
                    BusinessEntityNode entityNode = new BusinessEntityNode()
                    {
                        EntityId = entityItem.EntityId,
                        Name = entityItem.Name,
                        Title = entityItem.Title,
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
                    node.Children.Add(GetModuleNode(item, modules, entities, node, withchildEntities));
                }
            }

            return node;
        }

        BusinessEntityNode GetBusinessEntityNodeRecursively(IEnumerable<BusinessEntityNode> beNodes, EntityType targetType, string targetId)
        {
            foreach (var beNode in beNodes)
            {
                if (beNode.EntType == targetType && targetId.Equals(beNode.EntityId.ToString(), StringComparison.InvariantCultureIgnoreCase))
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
