using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Fzero.MobileCDRAnalysis;

public class WebUser
{
    #region Members
    private User user = new User();
    private DateTime lastLoginDate;
    private bool isAuthenticated = false;
    private string previousPage = "Login.aspx";
    private string currentPage = "Login.aspx";
    private string random = string.Empty;
    #endregion

    #region Properties
    public User User
    {
        get { return user; }
        set { user = value; }
    }
    public DateTime LastLoginDate
    {
        get { return lastLoginDate; }
        set { lastLoginDate = value; }
    }


    public bool IsAuthenticated
    {
        get { return isAuthenticated; }
        set { isAuthenticated = value; }
    }
    public string PreviousPage
    {
        get { return previousPage; }
        set { previousPage = value; }
    }
    public string CurrentPage
    {
        get { return currentPage; }
        set { currentPage = value; }
    }
    public string Random
    {
        get
        {
            return random;
        }
        set
        {
            random = value;
        }
    }

    public Parameters Params { get; set; }

    #endregion

    #region Methods
    public WebUser()
    {
        isAuthenticated = false;
        previousPage = "Login.aspx";
        currentPage = "Login.aspx";
    }

    public void Login()
    {
        LastLoginDate = user.LastLoginTime.HasValue ? user.LastLoginTime.Value : DateTime.MinValue;//DateTime.MinValue;
        user.LastLoginTime = DateTime.Now;
        User.SaveLastLoginTime(user);
        isAuthenticated = true;
    }

    public void Logout()
    {
        user = new User();
        LastLoginDate = DateTime.MinValue;
        isAuthenticated = false;
        HttpContext.Current.Session.Clear();

    }


    public bool HasPermission(Enums.SystemPermissions permission)
    {
        if (user != null)
            if ((bool)this.user.IsSuperUser)
                return true;

        bool has = false;
        if (user != null)
            has = this.user.UserPermissions.Where(up => up.PermissionID == (int)permission).Count() > 0;
        return has;
    }
    #endregion

}