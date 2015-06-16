using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Fzero.MobileCDRAnalysis;

public class WebUser
{
    #region Members
    private DateTime lastLoginDate;
    private bool isAuthenticated = false;
    private string previousPage = "Login.aspx";
    private string currentPage = "Login.aspx";
    private string random = string.Empty;
    #endregion

    #region Properties
   
   
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
   


    #endregion

    #region Methods
    public WebUser()
    {
        isAuthenticated = true;
        previousPage = "Login.aspx";
        currentPage = "Login.aspx";
    }

   
    #endregion
    
}