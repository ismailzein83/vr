using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class MenuManager
    {
        #region ctor

        ModuleManager _moduleManager;
        ViewManager _viewManager;
        public MenuManager()
        {
            _moduleManager = new ModuleManager();
            _viewManager = new ViewManager();
        }

        #endregion

        #region Public Methods

        public List<MenuItem> GetMenuItems(bool getOnlyAllowDynamic,bool withEmptyChilds)
        {
            List<Module> modules = _moduleManager.GetModules();
            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if (item.ParentId == null)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules);

                    if (withEmptyChilds)
                        retVal.Add(rootItem);
                    else if (!withEmptyChilds && rootItem.Childs != null)
                        retVal.Add(rootItem);
                }
            }

            return retVal;
        }

        public List<MenuItem> GetAllMenuItems(int userId, bool withEmptyChilds)
        {
            List<Module> modules = _moduleManager.GetModules();

            List<View> views = _viewManager.GetViews();
            views = this.FilterViewsPerAudience(views, userId);

            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if (item.ParentId == null)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules, views, withEmptyChilds);
                    if (withEmptyChilds)
                        retVal.Add(rootItem);
                    else if (!CheckIfEmpty(rootItem))
                        retVal.Add(rootItem);
                }
            }

            return SortedMenuItems(retVal); ;
        }

        public bool CheckIfEmpty(MenuItem rootItem)
        {
            bool result = true;
            if (rootItem.MenuType == MenuType.View)
                return false;
          
            else if (rootItem.MenuType == MenuType.Module && rootItem.Childs !=null && rootItem.Childs.Count > 0)
            {
                foreach (var item in rootItem.Childs)
                {
                       result =  CheckIfEmpty(item);
                      if(!result)
                          return result;
                }
            }
            return result;
        }
        public List<MenuItem> SortedMenuItems(List<MenuItem> menuItems)
        {

            if (menuItems == null || menuItems.Count == 0)
                return menuItems;
            menuItems = menuItems.OrderBy(m => m.Rank).ToList();
            foreach (MenuItem menuItem in menuItems)
                if (menuItem.Childs != null && menuItem.Childs.Count != 0)
                    menuItem.Childs = SortedMenuItems(menuItem.Childs);
            return menuItems;


        }

        #endregion

        #region Private Methods

        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views, bool withEmptyChilds)
        {
             VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            MenuItem menu = new MenuItem() { Id = module.ModuleId, Location = module.Url, Icon = module.Icon, Rank = module.Rank, MenuType = MenuType.Module};
                    if (module.Settings != null && module.Settings.LocalizedName != null)
                        menu.Name = vrLocalizationManager.GetTranslatedTextResourceValue(module.Settings.LocalizedName, module.Name);
            if (menu.Name == null)
                menu.Name = module.Name;
            AddDefaultViewUrl(module.DefaultViewId, menu);
            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);

            IViewUserAccessContext viewUserAccessContext = new ViewUserAccessContext { UserId = SecurityContext.Current.GetLoggedInUserId() };

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if ((viewItem.ActionNames == null || SecurityContext.Current.HasPermissionToActions(viewItem.ActionNames))
                        &&
                        (viewItem.Settings == null || viewItem.Settings.DoesUserHaveAccess(viewUserAccessContext))
                        )
                    {
                        MenuItem viewMenu = new MenuItem() { Id = viewItem.ViewId, Location = viewItem.Url, Type = viewItem.Type, Rank = viewItem.Rank, MenuType = MenuType.View };
                                if (viewItem.Settings != null && viewItem.Settings.ViewNameResourceKey != null)
                                    viewMenu.Name = vrLocalizationManager.GetTranslatedTextResourceValue(viewItem.Settings.ViewNameResourceKey, viewItem.Name);

                                if (viewItem.Settings != null && viewItem.Settings.ViewTitleResourceKey != null)
                                    viewMenu.Title = vrLocalizationManager.GetTranslatedTextResourceValue(viewItem.Settings.ViewTitleResourceKey,viewItem.Title);
                        if (viewMenu.Name == null)
                            viewMenu.Name = viewItem.Name;
                        if (viewMenu.Title == null)
                            viewMenu.Title = viewItem.Title;

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
                    List<View> viewsOfSubModules = views.FindAll(x => x.ModuleId == item.ModuleId);
                    if ((viewsOfSubModules != null && viewsOfSubModules.Count > 0) || withEmptyChilds)
                        menu.Childs.Add(GetModuleMenu(item, modules, views, withEmptyChilds));
                }
            }

            return menu;
        }

        private List<View> FilterViewsPerAudience(List<View> views, int userId)
        {
            List<View> filteredResults = new List<View>();
            GroupManager groupManager = new GroupManager();

            foreach (View item in views)
            {
                if (item.Audience != null)
                {
                    //Check if the user is an audience then add the view; otherwise the view will not be in the filtered results
                    if ((item.Audience.Users != null && item.Audience.Users.Find(x => x == userId) != 0) ||
                        item.Audience.Groups != null && groupManager.IsUserMemberInGroups(userId, item.Audience.Groups))
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

        private MenuItem GetModuleMenu(Module module, List<Module> modules)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            MenuItem menu = new MenuItem() { Id = module.ModuleId,  Location = module.Url, Icon = module.Icon, AllowDynamic = module.AllowDynamic };
                    if (module.Settings != null && module.Settings.LocalizedName != null)
                        menu.Name = vrLocalizationManager.GetTranslatedTextResourceValue(module.Settings.LocalizedName, module.Name);
            if (menu.Name == null)
                menu.Name = module.Name;

            AddDefaultViewUrl(module.DefaultViewId, menu);   
            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

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
        private void AddDefaultViewUrl(Guid? defaultViewId, MenuItem menu)
        {
            if (defaultViewId.HasValue)
            {
                View defaultView = new ViewManager().GetView(defaultViewId.Value);
                menu.Title = defaultView.Title;
                if (defaultView.Settings != null)
                    menu.DefaultURL = defaultView.Settings.GetURL(defaultView);
                else
                    menu.DefaultURL = defaultView.Url;

            }
        }

        #endregion
    }
}
