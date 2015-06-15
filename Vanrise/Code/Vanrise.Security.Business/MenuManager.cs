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
            PermissionManager permmissionManager = new PermissionManager();
            Dictionary<string, Dictionary<string, Flag>> effectivePermissions = permmissionManager.GetEffectivePermissions(token);

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

        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views, Dictionary<string, Dictionary<string, Flag>> effectivePermissions)
        {
            MenuItem menu = new MenuItem() { Name = module.Name, Location = module.Url, Icon = module.Icon };

            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if(viewItem.RequiredPermissions == null || isAllowed(viewItem.RequiredPermissions, effectivePermissions))
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

        private bool isAllowed(Dictionary<string, List<string>> requiredPermissions, Dictionary<string, Dictionary<string, Flag>> effectivePermissions)
        {
            //Assume that the view is allowed, and start looping until you find an exception that prevents the user from seeing this view
            bool result = true;

            foreach (KeyValuePair<string, List<string>> kvp in requiredPermissions)
            {
                result = CheckPermissions(kvp.Key, kvp.Value, effectivePermissions, new HashSet<string>());
                if (!result)
                    break;
            }

            return result;
        }

        private bool CheckPermissions(string requiredPath, List<string> requiredFlags, Dictionary<string, Dictionary<string, Flag>> effectivePermissions, HashSet<string> allowedFlags)
        {
            bool result = true;

            Dictionary<string, Flag> effectivePermissionFlags;
            if (effectivePermissions.TryGetValue(requiredPath, out effectivePermissionFlags))
            {
                Flag fullControlFlag;
                if (effectivePermissionFlags.TryGetValue("Full Control", out fullControlFlag))
                {
                    if (fullControlFlag == Flag.DENY)
                        return false;
                    else
                    {
                        foreach(var flag in requiredFlags)
                            allowedFlags.Add(flag);
                    }
                }
                else
                {
                    foreach (string requiredFlag in requiredFlags)
                    {
                        Flag effectivePermissionFlag;
                        if (effectivePermissionFlags.TryGetValue(requiredFlag, out effectivePermissionFlag))
                        {
                            if (effectivePermissionFlag == Flag.DENY)
                            {
                                return false;
                            }
                            else
                            {
                                allowedFlags.Add(requiredFlag);
                            }
                        }
                    }
                }
            }

            //The required path might be in one level up, then check it on that level recursively
            int index = requiredPath.LastIndexOf('/');
            if (index > 0)
            {
                //Keep looping recursively until you finish trimming the whole string requiredPath
                string oneLevelUp = requiredPath.Remove(index);
                result = CheckPermissions(oneLevelUp, requiredFlags, effectivePermissions, allowedFlags);
            }
            else
            {
                //Validation logic
                foreach (string item in requiredFlags)
                {
                    if (!allowedFlags.Contains(item))
                        return false;
                }
            }

            return result;
        }
        
    }
}
