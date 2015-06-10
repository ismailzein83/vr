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
                if(item.Parent == 0)
                {
                    retVal.Add(GetModuleMenu(item, modules, views, effectivePermissions));
                }
            }

            return retVal;
        }

        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views, Dictionary<string, List<string>> effectivePermissions)
        {
            MenuItem menu = new MenuItem() { Name = module.Name, Location = module.Url, Icon = module.Icon };

            List<Module> subModules = modules.FindAll(x => x.Parent == module.ModuleId);

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
                string result = "";
                foreach (BusinessEntityNode node in businessEntityHierarchy)
                {
                    bool found = false;
                    result = this.GetPathAggregation(item, node, businessEntityHierarchy, out found).ToString();
                    if (found)
                    {
                        break;
                    }
                    else
                    {
                        result = "";
                    }
                }

                List<string> allowedFlags = new List<string>();

                foreach (PermissionFlag flag in item.PermissionFlags)
                {
                    if (flag.Value == Flag.ALLOW)
                        allowedFlags.Add(flag.Name);
                }

                if (allowedFlags.Count > 0)
                {
                    effectivePermissions.Add(result, allowedFlags);
                }
            }

            return effectivePermissions;
        }

        private StringBuilder GetPathAggregation(Permission permission, BusinessEntityNode node, List<BusinessEntityNode> businessEntityHierarchy, out bool found)
        {
            found = false;

            StringBuilder path = new StringBuilder();

            path.Append(node.Name);
            path.Append("/");

            if(node.EntType == permission.EntityType && node.EntityId.ToString() == permission.EntityId)
            {
                found = true;
            }
            else if(node.Children != null && node.Children.Count > 0)
            {
                foreach(BusinessEntityNode subNode in node.Children)
                {
                   if(found)
                       break;
                    
                    path.Append(this.GetPathAggregation(permission, subNode, node.Children, out found));
                }
            }

            return path;
        }

        private bool isAllowed(Dictionary<string, List<string>> requiredPermissions, Dictionary<string, List<string>> allowedPermissions)
        {
            bool result = true;

            if (requiredPermissions != null)
            {
                foreach (KeyValuePair<string, List<string>> kvp in requiredPermissions)
                {
                    if (allowedPermissions.ContainsKey(kvp.Key))
                    {
                        foreach (string requiredFlag in kvp.Value)
                        {
                            string found = allowedPermissions[kvp.Key].Find(x => x == requiredFlag);
                            if (found == null)
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
