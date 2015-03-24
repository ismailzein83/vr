using System;
using System.Collections.Generic;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;


public partial class LoggedActions : BasePage
{
    #region Methods

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.LoggedActions;

        gvLoggedActions.Columns[1].HeaderText = Resources.Resources.Id;
        gvLoggedActions.Columns[2].HeaderText = Resources.Resources.Action;
        gvLoggedActions.Columns[3].HeaderText = Resources.Resources.LogDate;
        gvLoggedActions.Columns[4].HeaderText = Resources.Resources.ActionBy;
       


    }

    private void ClearSearchForm()
    {
        rdpFromLogDate.Clear();
        rdpToLogDate.Clear();
        ddlSearchActionBy.SelectedIndex = -1;
        ddlSearchActions.SelectedIndex = -1;
    }

    private void FillCombos()
    {
        Manager.BindCombo(ddlSearchActions, Vanrise.Fzero.Bypass.ActionType.GetAllActionTypes(), "Name", "Id", Resources.Resources.AllDashes, "0");
        Manager.BindCombo(ddlSearchActionBy, Vanrise.Fzero.Bypass.User.GetAllUsers(), "FullName", "Id", Resources.Resources.AllDashes, "0");
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ViewLoggedActions))
            PreviousPageRedirect();
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.DeleteLoggedActions))
        {
            btnDelete.Visible = false;
            gvLoggedActions.Columns[1].Visible = false;
        }
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)
            PreviousPageRedirect();

        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            FillCombos(); 
        }
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvLoggedActions.Rebind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvLoggedActions.Rebind();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        List<int> ListIds = new List<int>();
        foreach (GridDataItem item in gvLoggedActions.Items)
        {
           if (item.Selected)
            {
                ListIds.Add(item.GetDataKeyValue("ID").ToString().ToInt());
            }
        }
        LoggedAction.Delete(ListIds);
        gvLoggedActions.Rebind();
    }

    protected void gvLoggedActions_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        int? actionId = null;
        if (ddlSearchActions.SelectedValue != string.Empty)
        {
            actionId = ddlSearchActions.SelectedValue.ToInt();
        }

        int actionBy = Manager.GetInteger(ddlSearchActionBy.SelectedValue);
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;
       
        gvLoggedActions.DataSource = LoggedAction.GetLoggedActions(actionId, actionBy, fromDate, toDate);
    }

    #endregion
}