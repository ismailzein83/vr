using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Collections.Specialized;
using System.Text;

/// <summary>
/// Summary description for OfficeUser
/// </summary>
public class OfficeUser : System.Security.Principal.IIdentity
{
    #region Members
    //protected List<Page> pages;
    //protected List<Menus> menus;
    //private static List<Role> roles;
    protected User _User = new User();
    protected bool isAutheticated = false;
    protected StringCollection accessiblePages;
    //protected List<Permission> permissions;
    //protected List<ProjectsLibrary.ProjectNotification> projectNotifications;
    #endregion

    #region Properties
    /// <summary>
    /// Get User Accessible Pages
    /// </summary>
    //public List<Page> Pages
    //{
    //    get
    //    {
    //        if (pages == null)
    //        {
    //            if (_User.IsSuperAdmin.HasValue)
    //            {
    //                if (_User.IsSuperAdmin.Value)
    //                {

    //                    return pages = CallGeneratorLibrary.Repositories.UserRepository.GetPages();
    //                }
    //                else
    //                    return pages = RoleRepository.GetPages(roles);
    //            }
    //            else
    //                return pages = RoleRepository.GetPages(roles);
    //        }
    //        else
    //            return pages;
    //    }
    //}
    ///// <summary>
    ///// Get User Permissions
    ///// </summary>
    //public List<Permission> Permissions
    //{
    //    get
    //    {
    //        if (permissions == null)
    //            return permissions = RoleRepository.GetPermissions(roles);
    //        return permissions;
    //    }
    //}
    /// <summary>
    /// Get User Menus
    /// </summary>
    //public List<Menus> Menus
    //{
    //    get
    //    {
    //        if (menus == null)
    //        {
    //            if (_User.IsSuperAdmin.HasValue)
    //            {
    //                if (_User.IsSuperAdmin.Value)
    //                {
    //                    CallGeneratorLibrary.CallGeneratorModelDataContext context = new CallGeneratorLibrary.CallGeneratorModelDataContext();
    //                    return menus = context.Menus.ToList<Menus>();
    //                }
    //                else
    //                    return menus = RoleRepository.GetMenus(roles);
    //            }
    //            else
    //                return menus = RoleRepository.GetMenus(roles);
    //        }
    //        else
    //            return menus;
    //    }
    //}
    /// <summary>
    /// 
    /// </summary>
    //public StringCollection AccessiblePages
    //{
    //    get
    //    {
    //        if (Pages != null)
    //        {
    //            foreach (Page p in Pages)
    //                accessiblePages.Add(p.Name);
    //        }
    //        return accessiblePages;
    //    }
    //}
    /// <summary>
    /// Project Notifications
    /// </summary>
    //public List<ProjectsLibrary.ProjectNotification> ProjectNotifications
    //{
    //    get
    //    {
    //        if (projectNotifications == null)
    //            if (this._User.ProjectNotifications != null)
    //                return projectNotifications = this._User.ProjectNotifications.ToList<ProjectsLibrary.ProjectNotification>();
    //        return projectNotifications;
    //    }
    //    set
    //    {
    //        projectNotifications = value;
    //    }
    //}
    public int Id { get { return _User.Id; } }
    public User User { get { return _User; } }
    public string Password { get { return _User.Password; } set { _User.Password = value; } }
    public DateTime LastLoginDate { get { return _User.LastLoginDate.Value; } }
    public string Email { get { return _User.Email; } set { _User.Email = value; } }
    public string Username { get { return _User.UserName; } set { _User.UserName = value; } }
    public string IpSwitch { get { return _User.IpSwitch; } set { _User.IpSwitch = value; } }
    public string CallerId { get { return _User.CallerId; } set { _User.CallerId = value; } }
    //public int CompanyId { get { return _User.Company.Id; } set { _User.Company.Id = value; } }
    //public string CompanyName { get { return _User.Company.Name; } set { _User.Company.Name = value; } }

    public string CreationDateF { get { return _User.CreationDate.HasValue ? _User.CreationDate.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty; } }
    public string LastLoginDateF { get { return _User.LastLoginDate.HasValue ? _User.LastLoginDate.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty; } }
    public bool IsAuthenticated
    {
        get
        {
            if (!isAutheticated)
            {
                // check the cookie existance
                HttpCookie myCookie = HttpContext.Current.Request.Cookies["AutoLogin"];
                if (myCookie != null)
                {
                    string username = myCookie.Values[0];
                    string password = myCookie.Values[1];
                    _User = new User();
                    _User = UserRepository.Login(username, password);
                    if (_User.Id > 0)
                        isAutheticated = true;
                }
            }
            return isAutheticated;
        }
    }
    ///// <summary>
    ///// Returns True if the Current User has the specified Permission
    ///// </summary>
    ///// <param name="Permission">The Permission to look for.</param>
    ///// <returns>True or False.</returns>
    //public bool HasPermission(Permission permission)
    //{
    //    return Permissions.Contains(permission);
    //}
    /// <summary>
    /// Returns true if the user permissions allow him to access the page (ASPX).
    /// </summary>
    /// <param name="pageName">The Page Name (ASPX)</param>
    /// <returns>Boolean true or false</returns>
    //public bool CanAccessPage(Page page)
    //{
    //    return Pages.Contains(page);
    //}
    /// <summary>
    /// Returns true if the user permissions allow him to access the page (ASPX).
    /// </summary>
    /// <param name="pageName">The Page Name (ASPX)</param>
    /// <returns>Boolean true or false</returns>
    //public bool CanAccessPage(string page)
    //{
    //    return AccessiblePages.Contains(page);
    //}
    public string Name
    {
        get
        {
            return _User.Name;
        }
    }
    public string AuthenticationType
    {
        get
        {
            return "Custom";
        }
    }
    public bool IsSuperAdministrator
    {
        get
        {
            if (_User.IsSuperAdmin.HasValue)
                if (_User.IsSuperAdmin.Value)
                    return true;
            return false;
        }
    }
    #endregion

    #region Constructor
    public OfficeUser()
    {
        accessiblePages = new StringCollection();
    }
    public OfficeUser(int userId)
    {
        _User = UserRepository.Load(userId);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Logs the current User out (de-authenticates him), Clears the session 
    /// and the auto-login cookie.
    /// </summary>
    public void Logout()
    {
        isAutheticated = false;
        HttpContext.Current.Session.RemoveAll();

        //roles = null;
        //this.pages = null;
        //this.menus = null;
        //this.permissions = null;
        this.accessiblePages = null;

        // remove the cookie from the response
        HttpCookie myCookie = HttpContext.Current.Request.Cookies["AutoLogin"];
        if (myCookie != null)
        {
            myCookie.Expires = DateTime.Parse("1990-01-01");
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
    }
    /// <summary>
    /// Gets the Hashed password.
    /// </summary>
    /// <param name="Username">The Username</param>
    /// <returns>The hashed password for the given username in the given country.</returns>
    public string GetHashedPassword(string username)
    {
        return UserRepository.GetPassword(username);
    }
    /// <summary>
    ///  Used to perform a Login in General
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="preserveCookie"></param>
    /// <returns></returns>
    //public bool Login(string username, string password, bool preserveCookie)
    //{
    //    _User = UserRepository.Login(username, password);

    //    //System.Web.HttpContext.Current.Session["CurrentUserEntity"] = _User;

    //    if (_User != null)
    //    {
    //        if (_User.Id > 0)
    //        {
    //            if (preserveCookie)
    //            {
    //                HttpContext context = HttpContext.Current;
    //                HttpCookie cookie = new HttpCookie("AutoLogin");
    //                cookie.Values.Add("SGBL_Username", username);
    //                cookie.Values.Add("SGBL_Pwd", password);
    //                cookie.Expires = DateTime.MaxValue;
    //                context.Response.Cookies.Add(cookie);
    //            }
    //            _User.LastLoginDate = DateTime.Now;
    //            UserRepository.Save(_User);
    //            isAutheticated = true;
    //        }
    //        else
    //            isAutheticated = false;
    //    }

    //    return isAutheticated;
    //}
    /// <summary>
    ///  Used to perform a Login in General
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="preserveCookie"></param>
    /// <returns></returns>
    public bool Login(CallGeneratorLibrary.User user, bool preserveCookie)
    {
        _User = user;//UserRepository.Login(username, password);

        //System.Web.HttpContext.Current.Session["CurrentUserEntity"] = _User;

        if (_User != null)
        {
            if (_User.Id > 0)
            {
                if (preserveCookie)
                {
                    HttpContext context = HttpContext.Current;
                    HttpCookie cookie = new HttpCookie("AutoLogin");
                    cookie.Values.Add("Vanrise_Username", user.UserName);
                    cookie.Values.Add("Vanrise_Pwd", user.Password);
                    cookie.Expires = DateTime.MaxValue;
                    context.Response.Cookies.Add(cookie);
                }
                _User.LastLoginDate = DateTime.Now;
                UserRepository.Save(_User);
                isAutheticated = true;
            }
            else
                isAutheticated = false;
        }
        return isAutheticated;
    }
    /// <summary>
    /// Get User Roles
    /// </summary>
    //public void ManipulateRoles()
    //{
    //    if (roles == null && _User.Id > 0)
    //        roles = RoleRepository.GetUserRoles(_User.Id);
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageName"></param>
    /// <returns></returns>
    //public List<RolePermission> GetRolePermissions(string pageName)
    //{
    //    if (roles != null)
    //    {
    //        var ids = from r in roles
    //                  select r.Id;

    //        List<int> roleIds = ids.ToList<int>();

    //        string rolesIds = string.Empty;
    //        foreach (int role in roleIds)
    //            rolesIds += role.ToString() + ",";
    //        rolesIds = rolesIds.TrimEnd(',');

    //        return RoleRepository.GetRolePermissions(pageName, rolesIds);
    //    }

    //    return null;
    //}
    //public RolePermission GetRolePermission(string pageName)
    //{
    //    if (roles != null)
    //    {
    //        var ids = from r in roles
    //                  select r.Id;

    //        List<int> roleIds = ids.ToList<int>();

    //        string rolesIds = string.Empty;
    //        foreach (int role in roleIds)
    //            rolesIds += role.ToString() + ",";
    //        rolesIds = rolesIds.TrimEnd(',');

    //        return null;//RoleRepository.GetRolePermissions(pageName, rolesIds);
    //    }

    //    return null;
    //}
    #endregion

}
