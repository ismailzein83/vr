using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;

public partial class SentEmails : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack) 
        {
            SetCaptions();
            SetPermissions();
            SetDate();
            gvEmails.Rebind();
        }

    }

    private void SetDate()
    {
        rdpFromDate.SelectedDate = DateTime.Now.AddMonths(-1);
        rdpToDate.SelectedDate = DateTime.Now;
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.SentEmails;

        btnSearch.Text = Resources.Resources.Search;
        btnSearchCancel.Text = Resources.Resources.Clear;
        btnDelete.Text = Resources.Resources.Delete;

        gvEmails.Columns[2].HeaderText = Resources.Resources.To;
        gvEmails.Columns[3].HeaderText = Resources.Resources.Subject;
        gvEmails.Columns[4].HeaderText = Resources.Resources.IsSent;
        gvEmails.Columns[5].HeaderText = Resources.Resources.SentDate;
    }

    protected void btnSearchCancel_Click(object sender, EventArgs e)
    {
        txtEmail.Text = "";
        SetDate();
        gvEmails.Rebind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvEmails.Rebind();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string ListIds = "";
        foreach (GridDataItem item in gvEmails.Items)
        {
            if (item.Selected)
            {
                if (ListIds == "")
                    ListIds += item.GetDataKeyValue("ID").ToString();
                else
                    ListIds += "," + item.GetDataKeyValue("ID").ToString();
            }
        }
        Vanrise.Fzero.Bypass.Email.DeleteEmail(ListIds);
        gvEmails.Rebind();
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ViewEmails))
            PreviousPageRedirect();


        gvEmails.Columns[gvEmails.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.DeleteEmails);
    }

    protected void gvEmails_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvEmails.DataSource = Vanrise.Fzero.Bypass.Email.GetAllEmails(txtEmail.Text, rdpFromDate.SelectedDate, rdpToDate.SelectedDate);
    }

}