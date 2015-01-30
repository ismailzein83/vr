using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CallGeneratorLibrary;

/// <summary>
/// Summary description for BaseUserControl
/// </summary>
public class BaseUserControl : System.Web.UI.UserControl
{
    #region Members

    #endregion

    #region Properties
    public BasePage BasePage
    {
        get { return Page as BasePage; }
    }
    //public OfficeUser OfficeUser
    //{
    //    get
    //    {
    //        return BasePage.OfficeUser;
    //    }
    //}
    //public Language CurrentLanguage
    //{
    //    get
    //    {
    //        return CallGeneratorLibrary.Utilities.Context.Current.Language;
    //    }
    //}
    #endregion

    #region Constructor
    public BaseUserControl()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    #endregion

    #region Events
    protected override void OnLoad(EventArgs e)
    {
        this.DataBind();
        base.OnLoad(e);
    }
    #endregion

    #region Methods
    public string GetEntry(string key)
    {
        return BasePage.GetEntry(key);
    }
    #endregion
}
