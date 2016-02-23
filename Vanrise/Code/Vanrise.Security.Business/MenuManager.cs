﻿using System;
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
        public List<MenuItem> GetMenuItems(bool getOnlyAllowDynamic)
        {
            List<Module> modules = _moduleManager.GetModules();
            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if (item.ParentId == 0)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules, getOnlyAllowDynamic);
                    if ((rootItem.AllowDynamic || !getOnlyAllowDynamic) || rootItem.Childs != null)
                        retVal.Add(rootItem);
                }
            }

            return retVal;
        }
        public List<MenuItem> GetMenuItems(int userId)
        {
            List<Module> modules = _moduleManager.GetModules();

            GroupManager groupManager = new GroupManager();
            List<int> groups = groupManager.GetUserGroups(userId);

            List<View> views = _viewManager.GetViews();
            views = this.FilterViewsPerAudience(views, userId, groups);

            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if (item.ParentId == 0)
                {
                    MenuItem rootItem = GetModuleMenuV2(item, modules, views);
                    if (rootItem.Childs != null && rootItem.Childs.Count > 0)
                        retVal.Add(rootItem);
                }
            }

            return SortedMenuItems(retVal); ;
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
        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views)
        {
            MenuItem menu = new MenuItem() { Id = module.ModuleId, Name = module.Name, Title = module.Title, Location = module.Url, Icon = module.Icon, Rank = module.Rank };

            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if (viewItem.RequiredPermissions == null || SecurityContext.Current.IsAllowed(viewItem.RequiredPermissions))
                    {
                        MenuItem viewMenu = new MenuItem() { Id = viewItem.ViewId, Name = viewItem.Name, Title = viewItem.Title, Location = viewItem.Url, Type = viewItem.Type, Rank = viewItem.Rank };
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
                    menu.Childs.Add(GetModuleMenu(item, modules, views));
                }
            }

            return menu;
        }
        private List<View> FilterViewsPerAudience(List<View> views, int userId, List<int> subscribedGroups)
        {
            List<View> filteredResults = new List<View>();

            foreach (View item in views)
            {
                if (item.Audience != null)
                {
                    //Check if the user is an audience then add the view; otherwise the view will not be in the filtered results
                    if ((item.Audience.Users != null && item.Audience.Users.Find(x => x == userId) != 0) || isAudienceInSubscribedGroups(item, subscribedGroups))
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
        private MenuItem GetModuleMenu(Module module, List<Module> modules, bool getOnlyAllowDynamic )
        {


            MenuItem menu = new MenuItem() { Id = module.ModuleId, Name = module.Name, Title = module.Title, Location = module.Url, Icon = module.Icon, AllowDynamic = module.AllowDynamic };

            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId && (x.AllowDynamic || !getOnlyAllowDynamic));

            if (subModules.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (Module item in subModules)
                {
                    menu.Childs.Add(GetModuleMenu(item, modules, getOnlyAllowDynamic));
                }
            }

            return menu;
        }
        #endregion

        #region Pending Methods

        MenuItem GetModuleMenuV2(Module module, List<Module> modules, List<View> views)
        {
            MenuItem menu = new MenuItem() { Id = module.ModuleId, Name = module.Name, Title = module.Title, Location = module.Url, Icon = module.Icon, Rank = module.Rank };

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);
            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if (viewItem.ActionNames == null || SecurityContext.Current.IsAllowedV2(viewItem.ActionNames))
                    {
                        MenuItem viewMenu = new MenuItem() { Id = viewItem.ViewId, Name = viewItem.Name, Title = viewItem.Title, Location = viewItem.Url, Type = viewItem.Type, Rank = viewItem.Rank };
                        menu.Childs.Add(viewMenu);
                    }
                }
            }

            if (subModules.Count > 0)
            {
                if (menu.Childs == null)
                    menu.Childs = new List<MenuItem>();
                foreach (Module item in subModules)
                    menu.Childs.Add(GetModuleMenuV2(item, modules, views));
            }

            return menu;
        }

        #endregion
    }
}
