using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;
using System.Configuration;


public partial class ReportedCases : BasePage
{
    #region Properties
  

    public ViewGeneratedCall currentObject
    {
        get
        {
            if (Session["ManageGeneratedCalls.currentObject"] is ViewGeneratedCall)
                return (ViewGeneratedCall)Session["ManageGeneratedCalls.currentObject"];
            return new ViewGeneratedCall();
        }
        set
        {
            Session["ManageGeneratedCalls.currentObject"] = value;
        }
    }
    
    #endregion

    #region Methods

    private void FillCombos()
    {
        Manager.BindCombo(ddlSearchClient, Vanrise.Fzero.Bypass.Client.GetAllClients(), "Name", "Id", Resources.Resources.AllDashes, "0");

        Manager.BindCombo(ddlSearchRecommendedAction, Vanrise.Fzero.Bypass.RecommendedAction.GetRecommendedActions(), "Name", "Id", Resources.Resources.AllDashes, "0");
        ddlSearchRecommendedAction.Items.FindItemByValue(((int)Enums.RecommendedAction.Block).ToString()).Selected = true;

        ddlSearchMobileOperator.Items.Clear();
        ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(Resources.Resources.AllDashes, "0"));
        List<MobileOperator> listMobileOperator = Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList();
        foreach( MobileOperator i in listMobileOperator)
        {
            ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }
        

        Manager.BindCombo(ddlSearchOperatorReply, Vanrise.Fzero.Bypass.MobileOperatorFeedback.GetMobileOperatorFeedbacks(), "Name", "Id", Resources.Resources.AllDashes, "0");
      
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ReportedCases;

        int columnIndex = 2;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.ReportID;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CaseID;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.AttemptDateTime;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.b_number;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CLI;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.DurationInSeconds;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.RecommendedAction;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.MobileOperatorFeedback;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.FeedbackDateTime;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.ReportingStatus;

        rdtpSearchFromAttemptDate.SelectedDate = DateTime.Now.Date;
        rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
        
    }

    private void ClearSearchForm()
    {
        rdtpSearchFromAttemptDate.SelectedDate = DateTime.Now.AddDays(-1).Date;
        rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddTicks(-1);
        txtSearchb_number.Text = string.Empty;
        txtSearchCLIReceived.Text= string.Empty;
        ddlSearchOperatorReply.SelectedIndex = 0;
        txtSearchReportID.Text = string.Empty;
        txtSearchCaseID.Text = string.Empty;
    }

    public bool ReportingStatusVisible(int ReportingStatusID)
    {
        if (ReportingStatusID == (int)Enums.ReportingStatuses.Reported)
        {
            return true;
        }
        return false;
    }


    #endregion

    #region Events
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            FillCombos();
        }

        if (CurrentUser.portalType == 2)//Application User Portal
            {
               
                ddlSearchMobileOperator.Enabled = true;
                gvGeneratedCalls.Columns[0].Visible = false;
            }
        else if (CurrentUser.portalType == 1)//Mobile Operator
            {
                ddlSearchMobileOperator.SelectedValue = CurrentUser.MobileOperatorID.ToString();
                ddlSearchMobileOperator.Enabled = false;
                gvGeneratedCalls.Columns[0].Visible = true;
            }
        else
            {
                PreviousPageRedirect();
            }
            


        if (!IsPostBack)
        {
            SetCaptions();

            if (Request.QueryString["ReportID"] != null)
            {
                txtSearchReportID.Text = Request.QueryString["ReportID"];
            }

            GridGroupByExpression exprReportID = new GridGroupByExpression(gvGeneratedCalls.Columns[1]);
            gvGeneratedCalls.MasterTableView.GroupByExpressions.Add(exprReportID);
            gvGeneratedCalls.Rebind();
        }
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvGeneratedCalls.Rebind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (ddlSearchRecommendedAction.SelectedValue.ToInt() == 0)
        {
            ShowError("Recommended action should be specifed");
            return;
        }
        else
        {
            
            gvGeneratedCalls.Rebind();
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        
    }

    protected void gvGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        
        int MobileOperatorFeedbackID=ddlSearchOperatorReply.SelectedValue.ToInt();
        int RecommendedActionID=ddlSearchRecommendedAction.SelectedValue.ToInt();

        string CLIReceived = txtSearchCLIReceived.Text.Trim();
        string b_number = txtSearchb_number.Text.Trim();
        string CaseID = txtSearchCaseID.Text.Trim();

        string ReportID = txtSearchReportID.Text.Trim();

        DateTime? AttemptFrom = rdtpSearchFromAttemptDate.SelectedDate;
        DateTime? AttemptTo = rdtpSearchToAttemptDate.SelectedDate;


        int CLIMobileOperatorID = 0;
        int B_NumberMobileOperatorID = 0;

        if (ddlSearchRecommendedAction.SelectedValue.ToInt() == (int) Enums.RecommendedAction.Block)
        {
             CLIMobileOperatorID = ddlSearchMobileOperator.SelectedValue.ToInt();
        }
        else if (ddlSearchRecommendedAction.SelectedValue.ToInt() == (int)Enums.RecommendedAction.Investigate)
        {
             B_NumberMobileOperatorID = ddlSearchMobileOperator.SelectedValue.ToInt();
        }



        gvGeneratedCalls.DataSource = GeneratedCall.GetReportedCalls(CaseID, b_number, CLIReceived, AttemptFrom, AttemptTo, ReportID, CLIMobileOperatorID, B_NumberMobileOperatorID, MobileOperatorFeedbackID, RecommendedActionID,0, 0);
    }

    protected void gvGeneratedCalls_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string[] arg = new string[3];
        arg = e.CommandArgument.ToString().Split(';');

        int Id = 0;
        int CaseID = 0;
        string ReportRealID = string.Empty;

        if (arg.Length == 3)
        {
            Id = Manager.GetInteger(arg[0]);
            CaseID = Manager.GetInteger(arg[1]);
            ReportRealID = arg[2];
        }

        switch (e.CommandName)
        {

            case "Report":
                ClientScript.RegisterStartupScript(GetType(), "DownloadFile", "window.open('DownloadFile.aspx?ReportRealID=" + ReportRealID + "');", true);
                break;
        }
    }
       
    protected void ddlDateRange_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        switch (ddlDateRange.SelectedValue)
        {

            case "0"://Specific Date

                break;

            case "1"://Today
                rdtpSearchFromAttemptDate.SelectedDate = DateTime.Now.Date;
                rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                break;

            case "2"://Yesterday
                rdtpSearchFromAttemptDate.SelectedDate = DateTime.Now.Date.AddDays(-1);
                rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                break;

            case "3"://This Week
                DateTime input = DateTime.Now.Date;
                int delta = DayOfWeek.Monday - input.DayOfWeek;
                DateTime monday = input.AddDays(delta);
                rdtpSearchFromAttemptDate.SelectedDate = monday;
                rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                break;

            case "4"://This Month
                rdtpSearchFromAttemptDate.SelectedDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                gvGeneratedCalls.Rebind();
                break;

            case "5"://This Year
                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                rdtpSearchFromAttemptDate.SelectedDate = firstDay;
                rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                break;

            case "6"://Last Week
                DateTime inputlastweek = DateTime.Now.Date.AddDays(-7);
                int deltalastweek = DayOfWeek.Monday - inputlastweek.DayOfWeek;
                DateTime mondaylastweek = inputlastweek.AddDays(deltalastweek);
                rdtpSearchFromAttemptDate.SelectedDate = mondaylastweek;
                rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                break;

            case "7"://Last Month

                DateTime LastMonthLastDate = DateTime.Today.AddDays(0 - DateTime.Today.Day).AddHours(12).AddSeconds(-1);
                DateTime LastMonthFirstDate = DateTime.Today.AddDays(0 - DateTime.Today.Day).AddDays(1 - DateTime.Today.AddDays(0 - DateTime.Today.Day).Day);

                rdtpSearchFromAttemptDate.SelectedDate = LastMonthFirstDate;
                rdtpSearchToAttemptDate.SelectedDate = LastMonthLastDate;
                break;


        }

    }

    protected void rdtpSearchFromAttemptDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        ddlDateRange.SelectedValue = "0";
    }

    protected void rdtpSearchToAttemptDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        ddlDateRange.SelectedValue = "0";
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        gvGeneratedCalls.ExportSettings.IgnorePaging = true;
        gvGeneratedCalls.ExportSettings.ExportOnlyData = true;
        gvGeneratedCalls.ExportSettings.OpenInNewWindow = true;
        gvGeneratedCalls.MasterTableView.ExportToExcel();
    }

    #endregion

   
}