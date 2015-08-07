using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
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
        ((MasterPage)Master).PageHeaderTitle = "Sent Emails";


        gvEmails.Columns[2].HeaderText = "To";
        gvEmails.Columns[3].HeaderText = "Subject";
        gvEmails.Columns[4].HeaderText = "Is Sent";
        gvEmails.Columns[5].HeaderText = "Sent Date";
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
        Vanrise.Fzero.MobileCDRAnalysis.prGetEmails_Result.DeleteEmail(ListIds);
        gvEmails.Rebind();
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.SentEmails))
            PreviousPageRedirect();


        gvEmails.Columns[gvEmails.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.DeleteSentEmails);
    }

    protected void gvEmails_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvEmails.DataSource = Vanrise.Fzero.MobileCDRAnalysis.prGetEmails_Result.GetAllEmails(txtEmail.Text, rdpFromDate.SelectedDate, rdpToDate.SelectedDate);
    }

}