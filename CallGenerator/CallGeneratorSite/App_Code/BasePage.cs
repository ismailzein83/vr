using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using CallGeneratorLibrary;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
    #region Members
    private static OfficeUser _CurrentUser;
    private bool _HasPermissionChecking = true;
    private string _JavaScriptMessage = string.Empty;
    #endregion

    #region Properties

    public bool HasPermissionChecking
    {
        get
        {
            return _HasPermissionChecking;
        }
        set
        {
            _HasPermissionChecking = value;
        }
    }
    #endregion

    #region Constructor
    public BasePage()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    #endregion

    #region Events
    protected override void OnInit(EventArgs e)
    {
        SetCulture();
        base.OnInit(e);
    }
    protected override void OnLoad(EventArgs e)
    {
        try
        {
            Response.Cache.SetMaxAge(TimeSpan.Zero);
            Response.Expires = 0;
            Response.ExpiresAbsolute = DateTime.UtcNow.AddYears(-1);
            Response.Cache.SetNoStore();

            base.OnLoad(e);
        }
        catch (System.Exception ex)
        {
            Logger.LogException(ex);
        }
    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        //this is used to bind automatically the entries selected from the resources
        //this.DataBind();
    }

    protected override void Render(HtmlTextWriter writer)
    {
        try
        {
            // If the page contains a javascript alert, show it.
            if (_JavaScriptMessage != "")
            {
                Literal jsAlert = new Literal();
                jsAlert.Text = @"<script language=""javascript"">if(CustomAlert)CustomAlert(""" + _JavaScriptMessage + @"""); else alert(""" + _JavaScriptMessage + @""");</script>";
                this.Controls.Add(jsAlert);
            }

            // Render the page, then..
            base.Render(writer);
        }
        catch (System.Exception ex)
        {
            Logger.LogException(ex);
        }
    }
    protected void SendResponse(string sResponse)
    {
        System.IO.StreamWriter sw;

        sw = SetResponse();

        sw.Write(sResponse);

        sw.Flush();
        sw.Close();
    }

    protected System.IO.StreamWriter SetResponse()
    {
        Response.Cache.SetMaxAge(TimeSpan.Zero);
        Response.Expires = 0;
        Response.ExpiresAbsolute = DateTime.UtcNow.AddYears(-1);
        Response.Cache.SetNoStore();

        Response.Clear();

        return new System.IO.StreamWriter(Response.OutputStream);
    }
    #endregion

    #region Methods
    private void SetCulture()
    {
        //System.Threading.Thread.CurrentThread.CurrentCulture = CallGeneratorLibrary.Utilities.Context.Current.Language.Culture;
        //System.Threading.Thread.CurrentThread.CurrentUICulture = CallGeneratorLibrary.Utilities.Context.Current.Language.Culture;
    }
    public string GetEntry(string key)
    {
        string value = "";
        try
        {
            //switch (CurrentLanguage.Code)
            //{
            //    case "ar":
            //        value = GetGlobalResourceObject("Arabic", key) as string;
            //        break;
            //    case "en":
            //        value = GetGlobalResourceObject("English", key) as string;
            //        break;
            //    case "fr":
            //        value = GetGlobalResourceObject("French", key) as string;
            //        break;
            //}
        }
        catch { }
        return value;
    }

    public void JavaScriptAlert(string message)
    {
        if (_JavaScriptMessage != "") _JavaScriptMessage += "\\n";
        _JavaScriptMessage += message
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
    public void ConstructPageTitle(string entry)
    {
        this.Title = GetEntry("WebsiteName") + " :: " + GetEntry(entry);
    }
    public void ConstructPageTitle(params string[] fields)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(GetEntry("WebsiteName"));

        for (int i = 0; i < fields.Length; i++)
        {
            sb.Append(" :: ");
            sb.Append(fields[i]);
        }

        this.Title = sb.ToString();
    }
    public void ConstructPageTitleWithoutResources(string entry)
    {
        this.Title = GetEntry("WebsiteName") + " :: " + entry;

    }
    #endregion

    #region ViewState Manager
    //Setup the name of the hidden field on the client for storing the viewstate key
    public const string VIEW_STATE_FIELD_NAME = "__Custom_VIEWSTATE";

    //Setup a formatter for the viewstate information
    private LosFormatter _formatter = null;


    //overriding method of Page class
    protected override object LoadPageStateFromPersistenceMedium()
    {
        //If server side enabled use it, otherwise use original base class implementation
        if (true == ViewStateManager.GetViewStateManager().ServerSideEnabled)
        {
            return LoadViewState();
        }
        else
        {
            return base.LoadPageStateFromPersistenceMedium();
        }
    }

    //overriding method of Page class
    protected override void SavePageStateToPersistenceMedium(object viewState)
    {
        if (true == ViewStateManager.GetViewStateManager().ServerSideEnabled)
        {
            SaveViewState(viewState);
        }
        else
        {
            base.SavePageStateToPersistenceMedium(viewState);
        }
    }

    //implementation of method
    private object LoadViewState()
    {
        if (_formatter == null)
        {
            _formatter = new LosFormatter();
        }

        //Check if the client has form field that stores request key
        if (null == Request.Form[VIEW_STATE_FIELD_NAME])
        {
            //Did not see form field for viewstate, return null to try to continue (could log event...)
            return null;
        }

        //Make sure it can be converted to request number (in case of corruption)
        long lRequestNumber = 0;
        try
        {
            lRequestNumber = Convert.ToInt64(Request.Form[VIEW_STATE_FIELD_NAME]);
        }
        catch
        {
            //Could not covert to request number, return null (could log event...)
            return null;
        }

        //Get the viewstate for this page
        string _viewState = ViewStateManager.GetViewStateManager().GetViewState(lRequestNumber);

        //If find the viewstate on the server, convert it so ASP.Net can use it
        if (_viewState == null)
            return null;
        else
            return _formatter.Deserialize(_viewState);
    }


    //implementation of method
    private void SaveViewState(object viewState)
    {
        if (_formatter == null)
        {
            _formatter = new LosFormatter();
        }

        //Save the viewstate information
        StringBuilder _viewState = new StringBuilder();
        StringWriter _writer = new StringWriter(_viewState);
        _formatter.Serialize(_writer, viewState);
        long lRequestNumber = ViewStateManager.GetViewStateManager().SaveViewState(_viewState.ToString());

        //Need to register the viewstate hidden field (must be present or postback things don't 
        // work, we use in our case to store the request number)
        ClientScript.RegisterHiddenField(VIEW_STATE_FIELD_NAME, lRequestNumber.ToString());
    }
    #endregion

}