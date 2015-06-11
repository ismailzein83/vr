using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class MenuManager
    {
        public List<MenuItem> GetMenuItems(string token)
        {
            SecurityToken secToken = Common.Serializer.Deserialize<SecurityToken>(Common.TempEncryptionHelper.Decrypt(token));

            Dictionary<string, List<string>> effectivePermissions = this.GetEffectivePermissions(secToken.UserId);
            string x = Common.Serializer.Serialize(effectivePermissions);

            IModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            List<Module> modules = moduleDataManager.GetModules();
            
            IViewDataManager viewDataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            List<View> views = viewDataManager.GetViews();

            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if(item.ParentId == 0)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules, views, effectivePermissions);
                    if(rootItem.Childs.Count > 0)
                        retVal.Add(rootItem);
                }
            }

            return retVal;
        }

        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views, Dictionary<string, List<string>> effectivePermissions)
        {
            MenuItem menu = new MenuItem() { Name = module.Name, Location = module.Url, Icon = module.Icon };

            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if(isAllowed(viewItem.RequiredPermissions, effectivePermissions))
                    {
                        MenuItem viewMenu = new MenuItem() { Name = viewItem.Name, Location = viewItem.Url };
                        menu.Childs.Add(viewMenu);
                    }
                }
            }

            if (subModules.Count > 0)
            {
                foreach (Module item in subModules)
                {
                    menu.Childs.Add(GetModuleMenu(item, modules, views, effectivePermissions));
                }
            }

            return menu;
        }


        private Dictionary<string, List<string>> GetEffectivePermissions(int userId)
        {
            Dictionary<string, List<string>> effectivePermissions = new Dictionary<string, List<string>>();

            PermissionManager permissionManager = new PermissionManager();
            
            //TODO: consider these are read from the cache
            List<Permission> allPermissions = permissionManager.GetPermissions();
            List<BusinessEntityNode> businessEntityHierarchy = permissionManager.GetEntityNodes();

            //TODO: consider here to get also the aggregation from User Groups
            List<Permission> userPermissions = allPermissions.FindAll(x => x.HolderType == HolderType.USER && x.HolderId == userId.ToString());

            foreach (Permission item in userPermissions)
            {
                BusinessEntityNode result = null;

                //Get the node that matches the permission item in the business entity hierarchy tree
                foreach (BusinessEntityNode node in businessEntityHierarchy)
                {
                    result = node.Descendants().Where(x => x.EntType == item.EntityType && x.EntityId.ToString() == item.EntityId).FirstOrDefault();
                    if (result != null)
                    {
                        break;
                    }
                }

                //Just prepare the allowed flags in a list of strings, only add the flags that marked as Allow to the list
                List<string> allowedFlags = new List<string>();

                foreach (PermissionFlag flag in item.PermissionFlags)
                {
                    if (flag.Value == Flag.ALLOW)
                        allowedFlags.Add(flag.Name);
                }

                if (allowedFlags.Count > 0)
                {
                    //The get relative path will convert the business entity node into a path string
                    //Add a new record to the dictionary of effective permissions that is about the path as a key and the list of allowed permissions as a value
                    //TODO: this is to be cached later and moved to the base API Controller and done once in a user session
                    effectivePermissions.Add(result.GetRelativePath(), allowedFlags);
                }
            }

            return effectivePermissions;
        }

        private BusinessEntityNode GetBusinessEntityNode(Permission permission, BusinessEntityNode node, List<BusinessEntityNode> businessEntityHierarchy)
        {
            if(node.EntType == permission.EntityType && node.EntityId.ToString() == permission.EntityId)
            {
                return node;
            }
            else if(node.Children != null && node.Children.Count > 0)
            {
                foreach(BusinessEntityNode subNode in node.Children)
                {
                    return GetBusinessEntityNode(permission, subNode, node.Children);
                }
            }

            return null;
        }

        private bool isAllowed(Dictionary<string, List<string>> requiredPermissions, Dictionary<string, List<string>> allowedPermissions)
        {
            bool result = true;

            if (requiredPermissions != null)
            {
                foreach (KeyValuePair<string, List<string>> kvp in requiredPermissions)
                {
                    if (!(result = CheckPermissions(kvp.Key, kvp.Value, allowedPermissions)))
                        break;
                }
            }

            return result;
        }

        private bool CheckPermissions(string requiredPath, List<string> requiredFlags, Dictionary<string, List<string>> allowedPermissions)
        {
            bool result = true;

            if (allowedPermissions.ContainsKey(requiredPath))
            {
                foreach (string requiredFlag in requiredFlags)
                {
                    if(allowedPermissions[requiredPath].Find(x => x == requiredFlag) == null)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
        
    }
}
