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
        public List<MenuItem> GetMenuItems(SecurityToken secToken)
        {
            RoleManager roleManager = new RoleManager();
            List<int> groups = roleManager.GetUserRoles(secToken.UserId);

            PermissionManager permmissionManager = new PermissionManager();
            EffectivePermissionsWrapper effectivePermissionsWrapper = permmissionManager.GetEffectivePermissions(secToken, groups);

            IModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            List<Module> modules = moduleDataManager.GetModules();


            List<View> views = GetViews();

            views = this.FilterViewsPerAudience(views, secToken, groups);

            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if(item.ParentId == 0)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules, views, effectivePermissionsWrapper.EffectivePermissions, effectivePermissionsWrapper.BreakInheritanceEntities);
                    if(rootItem.Childs != null && rootItem.Childs.Count > 0)
                        retVal.Add(rootItem);
                }
            } 

            return retVal;
        }
        private List<View> GetViews()
        {
            IViewDataManager viewDataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            List<View> views = viewDataManager.GetViews();
            for (int i = 0; i < views.Count; i++)
            {
                if (views[i].Type == ViewType.Dynamic)
                    views[i].Url =string.Format("{0}/{{\"viewId\":\"{1}\"}}",views[i].Url,views[i].ViewId);
            }
            return views;

        } 


        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views, Dictionary<string, Dictionary<string, Flag>> effectivePermissions, HashSet<string> breakInheritanceEntities)
        {
            MenuItem menu = new MenuItem() { Id = module.ModuleId, Name = module.Name, Title = module.Title, Location = module.Url, Icon = module.Icon };

            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if(viewItem.RequiredPermissions == null || isAllowed(viewItem.RequiredPermissions, effectivePermissions, breakInheritanceEntities))
                    {
                        MenuItem viewMenu = new MenuItem() { Id = viewItem.ViewId, Name = viewItem.Name, Title = viewItem.Title, Location = viewItem.Url, Type = viewItem.Type };
                        menu.Childs.Add(viewMenu);
                    }
                }
            }

            if (subModules.Count > 0)
            {
                if (menu.Childs == null)
                    menu.Childs = new List<MenuItem>();
                foreach (Module item in subModules)
                {
                    menu.Childs.Add(GetModuleMenu(item, modules, views, effectivePermissions, breakInheritanceEntities));
                }
            }

            return menu;
        }

        private bool isAllowed(Dictionary<string, List<string>> requiredPermissions, Dictionary<string, Dictionary<string, Flag>> effectivePermissions, HashSet<string> breakInheritanceEntities)
        {
            //Assume that the view is allowed, and start looping until you find an exception that prevents the user from seeing this view
            bool result = true;

            foreach (KeyValuePair<string, List<string>> kvp in requiredPermissions)
            {
                result = CheckPermissions(kvp.Key, kvp.Value, effectivePermissions, breakInheritanceEntities, new HashSet<string>());
                if (!result)
                    break;
            }

            return result;
        }

        private bool CheckPermissions(string requiredPath, List<string> requiredFlags, Dictionary<string, Dictionary<string, Flag>> effectivePermissions,
            HashSet<string> breakInheritanceEntities, HashSet<string> allowedFlags)
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
            if (index > 0 && !breakInheritanceEntities.Contains(requiredPath))
            {
                //Keep looping recursively until you finish trimming the whole string requiredPath
                string oneLevelUp = requiredPath.Remove(index);
                result = CheckPermissions(oneLevelUp, requiredFlags, effectivePermissions, breakInheritanceEntities, allowedFlags);
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

        private List<View> FilterViewsPerAudience(List<View> views, SecurityToken secToken, List<int> subscribedGroups)
        {
            List<View> filteredResults = new List<View>();
            
            foreach (View item in views)
            {
                if(item.Audience != null)
                {
                    //Check if the user is an audience then add the view; otherwise the view will not be in the filtered results
                   if((item.Audience.Users != null && item.Audience.Users.Find(x => x == secToken.UserId) != 0) || isAudienceInSubscribedGroups(item, subscribedGroups))
                       filteredResults.Add(item);
                }
                else
                {
                    //No audience rule then add the view direclty
                    filteredResults.Add(item);
                }
            }

            return filteredResults;
        }

        private bool isAudienceInSubscribedGroups(View view, List<int> subscribedGroups)
        {
            if (view.Audience.Groups != null)
            {
                foreach (int groupId in subscribedGroups)
                {
                    if (view.Audience.Groups.Find(x => x == groupId) != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public List<MenuItem> GetMenuItems()
        {

            IModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            List<Module> modules = moduleDataManager.GetModules();
            List<MenuItem> retVal = new List<MenuItem>();
          
            foreach (Module item in modules)
            {
                if (item.ParentId == 0)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules);
                    if (rootItem.AllowDynamic  || rootItem.Childs!=null)
                        retVal.Add(rootItem);
                }
            }

            return retVal;
        }
        private MenuItem GetModuleMenu(Module module, List<Module> modules)
        {
            
            
            MenuItem menu = new MenuItem() { Id = module.ModuleId, Name = module.Name, Location = module.Url, Icon = module.Icon, AllowDynamic=module.AllowDynamic };
            
            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId && x.AllowDynamic);

            if (subModules.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (Module item in subModules)
                {
                        menu.Childs.Add(GetModuleMenu(item, modules));
                }
            }

            return menu;
        }
        
    }
}
