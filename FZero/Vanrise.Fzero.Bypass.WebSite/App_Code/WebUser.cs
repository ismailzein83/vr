using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Fzero.Bypass;

public class WebUser
{
    #region Members
    private User user = new User();
    private DateTime lastLoginDate;
    private bool isAuthenticated = false;
    private string previousPage = "Default.aspx";
    private string currentPage = "Default.aspx";
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

    public int ApplicationUserID
    {
        //get { return ApplicationUserId; }
        //set { ApplicationUserId = value; }
        get
        {
            if (user.ApplicationUser != null)
                return user.ApplicationUser.ID;
            return 0;
        }
    }

    public int MobileOperatorID
    {
        get
        {
            if (user.MobileOperator != null)
                return user.MobileOperator.ID;
            return 0;
        }
      
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

    public int portalType
    {
        get 
        {
            return user.AppTypeID.HasValue ? user.AppTypeID.Value : 0; //None
        }
    }


    #endregion

    #region Methods
    public WebUser()
    {
        isAuthenticated = false;
        previousPage = "Default.aspx";
        currentPage = "Default.aspx";
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
        if ( user!=null)
        if ((bool)this.user.IsSuperUser)
            return true;

        bool has = false;
        if (user != null)
        has = this.user.UserPermissions.Where(up => up.PermissionID == (int)permission).Count() > 0;
        return has;
    }
    #endregion


    public void SetMobileOperator(MobileOperator MobileOperator)
    {
        user.MobileOperators.Clear();
        user.MobileOperators.Add(MobileOperator);
    }

    public void SetUser(User user)
    {
        this.user = user;
    }
}