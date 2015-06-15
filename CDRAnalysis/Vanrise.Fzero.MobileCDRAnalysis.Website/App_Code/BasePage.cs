using CommonWebComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Telerik.Web.UI;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
    public static DateTime CurrentDate = DateTime.Today;


    public void ClearFilterAndSorting(RadGrid gv)
    {
        foreach (GridColumn column in gv.MasterTableView.Columns)
        {
            column.CurrentFilterFunction = GridKnownFunction.NoFilter;
            column.CurrentFilterValue = String.Empty;
            column.SortExpression = String.Empty;
        }
        gv.MasterTableView.FilterExpression = null;
        gv.MasterTableView.SortExpressions.Clear();
        gv.MasterTableView.ClearSelectedItems();
    }

    public static WebUser CurrentUser
    {
        get
        {
            if (!(HttpContext.Current.Session["currentUser"] is WebUser))
                HttpContext.Current.Session["currentUser"] = new WebUser();
            return (WebUser)HttpContext.Current.Session["currentUser"];
        }
        set
        {
            HttpContext.Current.Session["currentUser"] = value;
        }
    }
    public BasePage()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    protected override void OnPreInit(EventArgs e)
    {
        SetCulture();
        base.OnPreInit(e);
    }
    private void SetCulture()
    {
        //    System.Threading.Thread.CurrentThread.CurrentCulture = CurrentUser.Language.Culture;
        //    System.Threading.Thread.CurrentThread.CurrentUICulture = CurrentUser.Language.Culture;
    }

    #region Page Redirect Methods
    public void SelfRedirect()
    {
        Response.Redirect(this.GetType().BaseType.Name + ".aspx");
    }
    public void PreviousPageRedirect()
    {
        Response.Redirect(CurrentUser.PreviousPage);
    }

    public void RedirectTo(string page)
    {
        Response.Redirect(page);
    }

    private static string defaultPage = "~/SuspectionAnalysis.aspx";
    protected static string authenticationPage = "~/Login.aspx";
    public void DefaultPageRedirect()
    {
        Response.Redirect(defaultPage);
    }
    public void RedirectToAuthenticationPage()
    {
        string url = authenticationPage + "?returnto=" + Server.UrlEncode(Request.Url.AbsoluteUri);
        Response.Redirect(url);
    }

    public void RedirectToRerturnedPage()
    {
        string url = Vanrise.CommonLibrary.Manager.QueryString("returnto");
        if (string.IsNullOrEmpty(url))
            url = defaultPage;
        else
            url = Server.UrlDecode(url);
        Response.Redirect(url);
    }

    #endregion

    public void CallJavaScriptFunction(string callfunctionName)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "CallFunction", callfunctionName, true);
    }

    public void ShowError(string message)
    {
        CallJavaScriptFunction("showDialog('Error!', '" + message + "', true);");
    }

    public void ShowAlert(string message)
    {
        CallJavaScriptFunction("showDialog('Alert!', '" + message + "', true);");
    }

    public void ShowDialog(string message)
    {
        CallJavaScriptFunction("showDialog('Dialog', '" + message + "', false);");
    }

    protected override void OnLoad(EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            CurrentUser.PreviousPage = CurrentUser.CurrentPage;
            CurrentUser.CurrentPage = "~/" + this.GetType().BaseType.Name + ".aspx";
        } 
        base.OnLoad(e);
    }

    #region Session Properties

    public void ExportToExcel(RadGrid grid, dynamic data, List<int> hideColumns, bool allPages)
    {
        if (allPages)
        {
            grid.AllowPaging = false;
            grid.DataSource = data;
            grid.DataBind();
        }
        if(hideColumns != null)
            foreach (int index in hideColumns)
            { grid.Columns[index].Visible = false; }
        DownloadManager.DownloadToExcel(Response, grid);
        if (allPages)
        {
            grid.AllowPaging = true;
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    #endregion



}