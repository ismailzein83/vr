using Vanrise.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;
using System.Net;
using System.IO;
using System.Configuration;


public partial class ResultedCases : BasePage
{
    #region Properties

    List<String> ListCLi = new List<String>();
    List<String> ListDistinctCLi = new List<String>();
    
    #endregion

    #region Methods


    private void FillCombos()
    {
        Manager.BindCombo(ddlSearchClient, Vanrise.Fzero.Bypass.Client.GetAllClients(), "Name", "Id", Resources.Resources.AllDashes, "0");

        Manager.BindCombo(ddlSearchReceivedSource, Vanrise.Fzero.Bypass.Source.GetSourcesReceive(), "Name", "Id", Resources.Resources.AllDashes, "0");
        Manager.BindCombo(ddlSearchSource, Vanrise.Fzero.Bypass.Source.GetSourcesGenerate(), "Name", "Id", Resources.Resources.AllDashes, "0");
        Manager.BindCombo(ddlSearchPriority, Vanrise.Fzero.Bypass.Priority.GetPriorities(), "Name", "Id", Resources.Resources.AllDashes, "0");
        Manager.BindCombo(ddlSearchStatus, Vanrise.Fzero.Bypass.Status.GetStatuses(), "Name", "Id", Resources.Resources.AllDashes, "0");
        ddlSearchStatus.Items.FindItemByValue(((int)Enums.Statuses.Fraud).ToString()).Selected = true;


        Manager.BindCombo(ddlSearchReportingStatus, Vanrise.Fzero.Bypass.ReportingStatus.GetReportingStatuses(), "Name", "Id", Resources.Resources.AllDashes, "0");
        ddlSearchReportingStatus.Items.FindItemByValue(((int)Enums.ReportingStatuses.Pending).ToString()).Selected = true;

        ddlSearchCLIMobileOperator.Items.Add( new RadComboBoxItem(Resources.Resources.AllDashes, "0"));
        foreach (MobileOperator i in Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperators())
        {
            ddlSearchCLIMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }

        rblCLIReported.SelectedValue = "2";
    
    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ResultedCases;
        int columnIndex = 1;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CaseID;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Client;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.GeneratedBy;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.ReceivedBy;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Status;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Priority;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.ReportingStatus;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.a_number;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.b_number;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CLI;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.AttemptDateTime;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CLIReported;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.OperatorFeedback;
    }

    private void ClearSearchForm()
    {
        rdtpSearchFromAttemptDate.SelectedDate = DateTime.Now.Date;
        rdtpSearchToAttemptDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
        txtSearcha_number.Text = string.Empty;
        txtSearchb_number.Text = string.Empty;
        txtSearchCLIReceived.Text= string.Empty;
        ddlSearchStatus.SelectedIndex = 0;
        ddlSearchPriority.SelectedIndex = 0;
        ddlSearchSource.SelectedIndex = 0;
        ddlSearchCLIMobileOperator.SelectedIndex = 0;
        ddlSearchClient.SelectedIndex = 0;
        ddlSearchReportingStatus.SelectedIndex = 0;
        txtSearchCaseID.Text = string.Empty;
        txtSearchOriginationNetwork.Text = string.Empty;
        ddlSearchReportingStatus.SelectedValue = "1";
    }

    public bool ReportingStatusVisible(int ReportingStatusID)
    {
        if (ReportingStatusID == (int)Enums.ReportingStatuses.Reported)
        {
            return true;
        }
        return false;
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ViewResultedCalls))
            PreviousPageRedirect();

        gvGeneratedCalls.Columns[gvGeneratedCalls.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ViewResultedCalls);
    }

    private void ShowViewSection()
    {
        divView.Visible = true;
        divData.Visible = false;
        divFilter.Visible = false;
    }

    private void ShowSearchSection()
    {
        divView.Visible = false;
        divData.Visible = true;
        divFilter.Visible = true;
    }

    private void HideSections()
    {
        divView.Visible = false;
        divData.Visible = true;
        divFilter.Visible = true;
    }

    

    #endregion

    #region Events

    protected void rdtpSearchFromAttemptDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        ddlDateRange.SelectedValue = "0";
    }

    protected void rdtpSearchToAttemptDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        ddlDateRange.SelectedValue = "0";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)//Admin
            PreviousPageRedirect();


        if (!IsPostBack)
        {
            SetCaptions();
            SetPermissions();
            FillCombos();
        }
    }

    protected void btnSearchClear_Click(object sender, EventArgs e)
    {
        ClearSearchForm();
        gvGeneratedCalls.Rebind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvGeneratedCalls.Rebind();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        
        HideSections();
    }

    protected void gvGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        int ClientID = ddlSearchClient.SelectedValue.ToInt();
        string CLIReportedBool = "";
        if (rblCLIReported.SelectedValue.ToInt() == 1)
        {
            CLIReportedBool = "true";
        }
        else if (rblCLIReported.SelectedValue.ToInt() == 2)
        {
            CLIReportedBool = "false";
        }

        string OriginationNetwork = txtSearchOriginationNetwork.Text.Trim();
        string CLIReceived = txtSearchCLIReceived.Text.Trim();

        string a_number = txtSearcha_number.Text.Trim();

        string b_number = txtSearchb_number.Text.Trim();

        string CaseID = txtSearchCaseID.Text.Trim();

        int Source = ddlSearchSource.SelectedValue.ToInt();
        int ReceivedSource = ddlSearchReceivedSource.SelectedValue.ToInt();
        int MobileOperatorID = ddlSearchCLIMobileOperator.SelectedValue.ToInt();
        int Status = ddlSearchStatus.SelectedValue.ToInt();

        if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.DistintFraud)
        {
            Status = (int)Enums.Statuses.Fraud;
        }

        int Priority = ddlSearchPriority.SelectedValue.ToInt();
        int ReportingStatus = ddlSearchReportingStatus.SelectedValue.ToInt();
        DateTime? AttemptFrom = rdtpSearchFromAttemptDate.SelectedDate;
        DateTime? AttemptTo = rdtpSearchToAttemptDate.SelectedDate;


        
        List<prResultingCases_Result> ListViewGeneratedCalls = prResultingCases_Result.prResultingCases(CaseID, Source, ReceivedSource, MobileOperatorID, Status, Priority, ReportingStatus, a_number, b_number, CLIReceived, OriginationNetwork, AttemptFrom, AttemptTo, ClientID, CLIReportedBool);
        List<prResultingCases_Result> FinalListViewGeneratedCalls = ListViewGeneratedCalls.ToList();

        

        if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.DistintFraud)
        {
            foreach (prResultingCases_Result v in ListViewGeneratedCalls)
            {
                if (v.StatusName == "Fraud")
                {
                    if (ListDistinctCLi.Contains(v.CLI))
                    {
                        FinalListViewGeneratedCalls.Remove(v);
                    }
                    else
                    {
                        ListDistinctCLi.Add(v.CLI);
                    }

                }
            }
        }


            if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.DistintFraud || ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Fraud || ddlSearchStatus.SelectedValue.ToInt() == 0)
            {
                spanFraudCases.InnerText= ListViewGeneratedCalls.Where(x => x.StatusID == (int)Enums.Statuses.Fraud).Count().ToString();
                spanDistinctFraudCases.InnerText = ListViewGeneratedCalls.Where(x => x.StatusID == (int)Enums.Statuses.Fraud).GroupBy(x => x.CLI).Count().ToString();
                tblSummary.Visible = true;
            }
            else
            {
                spanFraudCases.InnerText = string.Empty;
                spanDistinctFraudCases.InnerText = string.Empty;
                tblSummary.Visible = false;
            }
            gvGeneratedCalls.DataSource = FinalListViewGeneratedCalls;

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
            case "View":
                wucGeneratedCallInformation.GeneratedCallId = Id.ToString();
                wucGeneratedCallInformation.FillData(prVwGeneratedCall_Result.View(Id));
                ShowViewSection();
                break;


            case "Report":
                ClientScript.RegisterStartupScript(GetType(), "DownloadFile", "window.open('DownloadFile.aspx?ReportRealID=" + ReportRealID + "');", true);
                break;
        }
    }
       
    protected void btnReportingStatus_ToBeReported_Click(object sender, EventArgs e)
    {

        IList<string> CLIs = new List<String>();


       foreach (GridDataItem item in gvGeneratedCalls.Items)
        {
            if (item.Selected && item.GetDataKeyValue("MobileOperatorFeedbackName") != null)
            {
                ShowError("Please Uncheck Cases that has feedback from operator.");
                return;
            }
            if (item.Selected && item.GetDataKeyValue("CLIReported").ToString().ToBoolean())
            {
                ShowError("Please Uncheck Previously Reported Numbers");
                return;
            }


            if (item.Selected && CLIs.Contains(item.GetDataKeyValue("CLI").ToString()))
            {
                ShowError("Please Uncheck Repeated CLIs");
                return;
            }
            else
            {
                CLIs.Add(item.GetDataKeyValue("CLI").ToString());
            }
           
        }





        List<int> ListIds = new List<int>();
        foreach (GridDataItem item in gvGeneratedCalls.Items)
        {
            if (item.Selected)
            {
              
                ListIds.Add(item.GetDataKeyValue("ID").ToString().ToInt());
            }
        }
        if (ListIds.Count == 0)
        {
            ShowError("Specify the cases to update reporting status");
            return;
        }
        GeneratedCall.UpdateReportStatus(ListIds, (int)Enums.ReportingStatuses.TobeReported, CurrentUser.ApplicationUserID);
        gvGeneratedCalls.Rebind();
        LoggedAction.AddLoggedAction((int)Enums.ActionTypes.ChangedcasesreportingstatusToBeReported, CurrentUser.User.ID);
    }

    protected void btnReportingStatus_Ignored_Click(object sender, EventArgs e)
    {
        List<int> ListIds = new List<int>();
        foreach (GridDataItem item in gvGeneratedCalls.Items)
        {

            if (item.Selected)
            {
                if (item.GetDataKeyValue("MobileOperatorFeedbackName") != null)
                {
                    ShowError("Please Uncheck Cases that has feedback from operator.");
                    return;
                }
                ListIds.Add(item.GetDataKeyValue("ID").ToString().ToInt());
            }
        }
        if (ListIds.Count == 0)
        {
            ShowError("Specify the cases to update reporting status");
            return;
        }
        GeneratedCall.UpdateReportStatus(ListIds, (int)Enums.ReportingStatuses.Ignored, CurrentUser.ApplicationUserID);
        gvGeneratedCalls.Rebind();
        LoggedAction.AddLoggedAction((int)Enums.ActionTypes.ChangedcasesreportingstatusIgnored, CurrentUser.User.ID);
        
    }

    protected void gvGeneratedCalls_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        
        if (e.Item is GridItem)
        {
            GridItem item = (GridItem)e.Item;
            Label lbl = (Label)item.FindControl("lblStatusName");
            
            if (lbl != null)
            {

                
                


                lbl.Font.Bold = true;
                if (lbl.Text == "Fraud")
                {
                    GridDataItem CLI_Item = e.Item as GridDataItem;
                    String CurrentCLI=string.Empty;
                    if (CLI_Item.GetDataKeyValue("CLI")!= null)
                    {
                         CurrentCLI = CLI_Item.GetDataKeyValue("CLI").ToString();
                    }
                    

                    if (ListCLi.Contains(CurrentCLI))
                    {
                        lbl.Text = "Repeated in Grid";
                       
                    }
                    else
                    {
                        ListCLi.Add(CurrentCLI);
                        item.ForeColor = System.Drawing.Color.Red;
                    }

                }
                else if (lbl.Text == "Suspect")
                {
                    item.ForeColor = System.Drawing.Color.OrangeRed;
                }
                else if (lbl.Text == "Pending")
                {
                    item.ForeColor = System.Drawing.Color.Gray;
                }
                else if (lbl.Text == "Clean")
                {
                    item.ForeColor = System.Drawing.Color.Green;
                }
                else if (lbl.Text == "Null")
                {
                    item.ForeColor = System.Drawing.Color.Orange;
                }
             
                else if (lbl.Text == "Ignored")
                {
                    item.Font.Strikeout = true;
                }



            }





            
           
        }
    }

    protected void rtsResultedCases_TabClick(object sender, RadTabStripEventArgs e)
    {
        gvGeneratedCalls.Rebind();
    }

    protected void btnReOpen_Click(object sender, EventArgs e)
    {
        List<int> ListIds = new List<int>();
        foreach (GridDataItem item in gvGeneratedCalls.Items)
        {
            if (item.Selected)
            {
                ListIds.Add(item.GetDataKeyValue("ID").ToString().ToInt());
                if (item.GetDataKeyValue("MobileOperatorFeedbackName") == null)
                {
                    ShowError("Only rejected cases can be re-opened.");
                    return;
                }

                if (item.GetDataKeyValue("MobileOperatorFeedbackName").ToString() != "Rejected")
                {
                    ShowError("Only rejected cases can be re-opened.");
                    return;
                }

            }
        }


        if (ListIds.Count == 0)
        {
            ShowError("Specify the cases to re-open");
            return;
        }
        GeneratedCall.UpdateReportStatus(ListIds, (int)Enums.ReportingStatuses.Reopened, CurrentUser.ApplicationUserID);
        gvGeneratedCalls.Rebind();
        LoggedAction.AddLoggedAction((int)Enums.ActionTypes.ChangedcasesreportingstatusIgnored, CurrentUser.User.ID);
        
    }

    protected void btnVerified_Click(object sender, EventArgs e)
    {
        List<int> ListIds = new List<int>();
        foreach (GridDataItem item in gvGeneratedCalls.Items)
        {
            if (item.Selected)
            {
                ListIds.Add(item.GetDataKeyValue("ID").ToString().ToInt());
                if (item.GetDataKeyValue("MobileOperatorFeedbackName") ==null )
                {
                    ShowError("Only cases with customer feedback can be verified.");
                    return;
                }

                if (item.GetDataKeyValue("MobileOperatorFeedbackName").ToString() == "Pending")
                {
                    ShowError("Only cases with customer feedback can be verified.");
                    return;
                }

            }
        }


        if (ListIds.Count == 0)
        {
            ShowError("Specify the cases to update reporting status");
            return;
        }
        GeneratedCall.UpdateReportStatus(ListIds, (int)Enums.ReportingStatuses.Verified, CurrentUser.ApplicationUserID);
        gvGeneratedCalls.Rebind();
        LoggedAction.AddLoggedAction((int)Enums.ActionTypes.ChangedcasesreportingstatusIgnored, CurrentUser.User.ID);
        
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        gvGeneratedCalls.ExportSettings.IgnorePaging = true;
        gvGeneratedCalls.ExportSettings.ExportOnlyData = true;
        gvGeneratedCalls.ExportSettings.OpenInNewWindow = true;
        gvGeneratedCalls.MasterTableView.ExportToExcel();
    }

    protected void btnToBeInvestigated_Click(object sender, EventArgs e)
    {
        List<int> ListIds = new List<int>();
        foreach (GridDataItem item in gvGeneratedCalls.Items)
        {

            if (item.Selected)
            {
                if (item.GetDataKeyValue("MobileOperatorFeedbackName") != null)
                {
                    ShowError("Please Uncheck Cases that has feedback from operator.");
                    return;
                }
                ListIds.Add(item.GetDataKeyValue("ID").ToString().ToInt());
            }
        }
        if (ListIds.Count == 0)
        {
            ShowError("Specify the cases to update reporting status");
            return;
        }
      
        GeneratedCall.UpdateReportStatus(ListIds, (int)Enums.ReportingStatuses.TobeInvestigated, CurrentUser.ApplicationUserID);
        gvGeneratedCalls.Rebind();
        LoggedAction.AddLoggedAction((int)Enums.ActionTypes.ChangedcasesreportingstatusIgnored, CurrentUser.User.ID);
        
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

    #endregion



   
}