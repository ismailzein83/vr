using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class WebsiteMenu
    {
        public static List<WebsiteMenu> GetMenus(int? appTypeID)
        {
            List<WebsiteMenu> menus = new List<WebsiteMenu>();
            try
            {
                using (Entities context = new Entities())
                {
                    if (appTypeID == 2)//Admin
                        menus = context.WebsiteMenus
                            .Include(m => m.WebsiteMenus1)
                            .Where(m =>
                                m.IsActive == true
                                && m.MainMenuID == null
                                && (!m.AppType.HasValue || m.AppType == 2))
                            .OrderBy(m => m.Ordering)
                            .ToList();
                    else if (appTypeID == 1)//Public
                        menus = context.WebsiteMenus
                           .Include(m => m.WebsiteMenus1)
                           .Where(m =>
                               m.IsActive == true
                               && m.MainMenuID == null
                               && (!m.AppType.HasValue || m.AppType == 1))
                           .OrderBy(m => m.Ordering)
                           .ToList();
                    else
                        menus = context.WebsiteMenus
                           .Include(m => m.WebsiteMenus1)
                           .Where(m =>
                               m.IsActive == true
                               && m.MainMenuID == null
                               && (!m.AppType.HasValue || m.AppType == appTypeID.Value))
                           .OrderBy(m => m.Ordering)
                           .ToList();
                }
                foreach (WebsiteMenu menu in menus)
                {
                    menu.WebsiteMenus1 = menu.WebsiteMenus1//SubMenus
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.Ordering).ToList();
                }
            }
            catch(Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.WebsiteMenu.GetMenus(" + appTypeID +")", err);
            }
            return menus;

        }
    }
}
