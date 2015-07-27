using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Vanrise.CommonLibrary;

using Telerik.Web.UI;
using Telerik.Charting;
using Vanrise.Fzero.Bypass;
using System.Drawing;
using System.Configuration;

public partial class DefaultClient : MobileOperatorPage
{
    #region Methods

    public int DifferenceinGMT
    {
        get
        {
            return (CurrentUser.User.GMT-SysParameter.Global_GMT);
        }
    }

    public DateTime fromDateTime
    {
        get
        {
            if (rdpFrom.SelectedDate == null)
            {
                rdpFrom.SelectedDate = DateTime.Now.AddHours(DifferenceinGMT).Date;
            }
            return rdpFrom.SelectedDate.Value.AddHours(-DifferenceinGMT);
        }
    }

    public DateTime toDateTime
    {
        get
        {
            if (rdpTo.SelectedDate == null)
            {
                rdpTo.SelectedDate = DateTime.Now.AddHours(DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
            }
            return rdpTo.SelectedDate.Value.AddHours(-DifferenceinGMT);
        }
    }

    private void FillCombos()
    {
       
    }

    private void SetPermissions()
    {
        if (CurrentUser.User.UserName != null && CurrentUser.User.UserName.ToLower() != ConfigurationManager.AppSettings["ClientName"].ToLower() || CurrentUser.User.Password != ConfigurationManager.AppSettings["ClientPassword"])
            RedirectTo("Login.aspx");
    }

    private void SetCaptions()
    {
        ((Master)Master).PageHeaderTitle = Resources.Resources.Dashboard;
        ((Master)Master).SignedInUser = CurrentUser.User.FullName;

        int columnIndexOnnetFraud = 0;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.CaseID;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.RouteID;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.Country;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.CLI;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.Occurance;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.FirstAttemptDateTime;
        gvOnnetFraudGeneratedCalls.Columns[columnIndexOnnetFraud++].HeaderText = Resources.Resources.LastAttemptDateTime;

        int columnIndexOfnetFraud = 0;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.CaseID;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.RouteID;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.Country;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.CLI;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.Occurance;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.FirstAttemptDateTime;
        gvOfnetFraudGeneratedCalls.Columns[columnIndexOfnetFraud++].HeaderText = Resources.Resources.LastAttemptDateTime;
        
        


        int columnIndexAll = 0;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.CaseID;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.RouteID;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.Country;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.Status;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.b_number;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.CLI;
        gvAllGeneratedCalls.Columns[columnIndexAll++].HeaderText = Resources.Resources.AttemptDateTime;


        int columnIndex = 2;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CaseID;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.AttemptDateTime;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CLI;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Action;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Feedback;

    }

    private void Populate()
    {

        gvCases.Rebind();
        gvClientSummary.Rebind();
        gvOriginationNetwork.Rebind();
        gvCarrier.Rebind();

        switch (rtsMain.SelectedIndex)
        {
            case 2:
                List<prGetTimeActiveonNetwork_Result> lstView_TimeActiveonNetwork = Vanrise.Fzero.Bypass.prGetTimeActiveonNetwork_Result.prGetTimeActiveonNetwork(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime);
                rcActiveTime.DataSource = lstView_TimeActiveonNetwork;
                rcActiveTime.DataBind();

                int index = 0;
                foreach (var i in lstView_TimeActiveonNetwork)
                {
                    rcActiveTime.Series[0].Items[index].Name = i.Period.Replace("twenty_twentyfour", "8 pm - 12 am").Replace("zero_four", "0 am - 4 am").Replace("four_eight", "4 am - 8 am").Replace("eight_twelve", "8 am - 12 am").Replace("twelve_sixteen", "12 am - 4 pm").Replace("sixteen_twenty", "4 pm - 8 pm");
                    index++;
                }


                rcActiveTime.Series[0].Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
                break;

            case 3:
                gvOnnetFraudGeneratedCalls.CurrentPageIndex = 0;
                gvOnnetFraudGeneratedCalls.Rebind();
                break;

            case 4:
                gvOfnetFraudGeneratedCalls.CurrentPageIndex = 0;
                gvOfnetFraudGeneratedCalls.Rebind();
                break;

            case 5:
                gvAllGeneratedCalls.CurrentPageIndex = 0;
                gvAllGeneratedCalls.Rebind();
                break;

            case 6:
                gvGeneratedCalls.Rebind();
                break;

        }
    }

    #endregion

    #region Events

    protected void gvClientSummary_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvClientSummary.DataSource = Dashboard.listprGetSummaryClient_Result;
    }

    protected void gvCases_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        Dashboard.GetDashboard(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, false);
        gvCases.DataSource = Dashboard.listprSummary_Result;
    }

    protected void rblDateRange_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (rblDateRange.SelectedValue)
        {

            case "0"://Specific Date

                break;

            case "1"://Today
                rdpFrom.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date;
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
                Populate();
                break;

            case "2"://Yesterday
                rdpFrom.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(-1);
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddSeconds(-1);
                Populate();
                break;

            case "3"://This Week
                DateTime input = DateTime.Now.AddHours(-DifferenceinGMT).Date;
                int delta = DayOfWeek.Monday - input.DayOfWeek;
                DateTime monday = input.AddDays(delta);
                rdpFrom.SelectedDate = monday;
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
                Populate();

                break;

            case "4"://This Month
                rdpFrom.SelectedDate = DateTime.Today.AddHours(-DifferenceinGMT).AddDays(1 - DateTime.Today.Day);
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
                Populate();
                break;

            case "5"://This Year
                int year = DateTime.Now.AddHours(-DifferenceinGMT).Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                rdpFrom.SelectedDate = firstDay;
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
                Populate();
                break;

            case "6"://Last Week
                DateTime inputlastweek = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(-7);
                int deltalastweek = DayOfWeek.Monday - inputlastweek.DayOfWeek;
                DateTime mondaylastweek = inputlastweek.AddDays(deltalastweek);
                rdpFrom.SelectedDate = mondaylastweek;
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
                Populate();

                break;

            case "7"://Last Month
                int lastyear = DateTime.Now.AddHours(-DifferenceinGMT).Year - 1;
                DateTime lastyear_firstDay = new DateTime(lastyear, 1, 1);
                rdpFrom.SelectedDate = lastyear_firstDay;
                rdpTo.SelectedDate = DateTime.Now.AddHours(-DifferenceinGMT).Date.AddSeconds(-1);
                Populate();
                break;


        }


    }

    protected void gvGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (rtsMain.SelectedIndex == 6)
        {
            gvGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetReportedCalls(string.Empty, string.Empty, string.Empty, fromDateTime, toDateTime, string.Empty, 0, 0, 0, 0, ConfigurationManager.AppSettings["ClientID"].ToInt(), DifferenceinGMT);
        }
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

    public bool ReportingStatusVisible(int ReportingStatusID)
    {
        if (ReportingStatusID == (int)Vanrise.Fzero.Bypass.Enums.ReportingStatuses.Reported)
        {
            return true;
        }
        return false;
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        gvAllGeneratedCalls.ExportSettings.IgnorePaging = true;
        gvAllGeneratedCalls.ExportSettings.ExportOnlyData = true;
        gvAllGeneratedCalls.ExportSettings.OpenInNewWindow = true;
        gvAllGeneratedCalls.MasterTableView.ExportToExcel();
    }

    protected void btnExportOnnetFraud_Click(object sender, EventArgs e)
    {
        gvOnnetFraudGeneratedCalls.ExportSettings.IgnorePaging = true;
        gvOnnetFraudGeneratedCalls.ExportSettings.ExportOnlyData = true;
        gvOnnetFraudGeneratedCalls.ExportSettings.OpenInNewWindow = true;
        gvOnnetFraudGeneratedCalls.MasterTableView.ExportToExcel();
    }

    protected void btnExportOfnetFraud_Click(object sender, EventArgs e)
    {
        gvOfnetFraudGeneratedCalls.ExportSettings.IgnorePaging = true;
        gvOfnetFraudGeneratedCalls.ExportSettings.ExportOnlyData = true;
        gvOfnetFraudGeneratedCalls.ExportSettings.OpenInNewWindow = true;
        gvOfnetFraudGeneratedCalls.MasterTableView.ExportToExcel();
    }

    protected void btnRefershFrom_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        Populate();
    }

    protected void btnRefershTo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        Populate();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ((Master)this.Master).ShowMenu(false);
       
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

      
        if (!IsPostBack)
        {
            ((Master)this.Master).ShowTitle(false);
            SetPermissions();
            SetCaptions();

            
            rdpFrom.SelectedDate = DateTime.Now.AddHours(DifferenceinGMT).Date;
            rdpTo.SelectedDate = DateTime.Now.AddHours(DifferenceinGMT).Date.AddDays(1).AddSeconds(-1);
            Populate();
            FillCombos();
        }
    }

    protected void gvOriginationNetwork_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        List<prVwOrigination_Result> listprVwOrigination_Result = Dashboard.listprVwOrigination_Result;
        gvOriginationNetwork.DataSource = listprVwOrigination_Result;
        rcOrigination.DataSource = listprVwOrigination_Result;
            rcOrigination.DataBind();

            Random random = new Random();
            int index = 0;
            foreach (var i in listprVwOrigination_Result)
            {
                Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                rcOrigination.Series[0].Items[index].Name = i.OriginationNetwork;
                rcOrigination.Series[0].Items[index].Appearance.FillStyle.MainColor = color;
                rcOrigination.Series[0].Items[index].Appearance.FillStyle.SecondColor = color;
                index++;
            }

            rcOrigination.Series[0].Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
    }

    protected void gvCarrier_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        List<prVwCarrier_Result> listprVwCarrier_Result = Dashboard.listprVwCarrier_Result;
        gvCarrier.DataSource = listprVwCarrier_Result;
        rcCarrier.DataSource = listprVwCarrier_Result;
            rcCarrier.DataBind();

            Random random = new Random();
            int index = 0;
            foreach (var i in listprVwCarrier_Result)
            {
               
                Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                rcCarrier.Series[0].Items[index].Name = i.Carrier;
                rcCarrier.Series[0].Items[index].Appearance.FillStyle.MainColor = color;
                rcCarrier.Series[0].Items[index].Appearance.FillStyle.SecondColor = color;
                index++;
            }

            rcCarrier.Series[0].Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
    }

    protected void rtsMain_TabClick(object sender, RadTabStripEventArgs e)
    {
        Populate();
    }

    protected void gvOnnetFraudGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (rtsMain.SelectedIndex == 3)
        {
            gvOnnetFraudGeneratedCalls.VirtualItemCount = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand_Count(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, 1, false, DifferenceinGMT, gvOnnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOnnetFraudGeneratedCalls.PageSize);
            gvOnnetFraudGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, 1, false, DifferenceinGMT,gvOnnetFraudGeneratedCalls.CurrentPageIndex+1,gvOnnetFraudGeneratedCalls.PageSize);
        }
    }

    protected void gvOfnetFraudGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (rtsMain.SelectedIndex == 4)
        {
            gvOfnetFraudGeneratedCalls.VirtualItemCount = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand_Count(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, 2, false, DifferenceinGMT, gvOfnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOfnetFraudGeneratedCalls.PageSize);
            gvOfnetFraudGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, 2, false, DifferenceinGMT, gvOfnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOfnetFraudGeneratedCalls.PageSize);
        }
    }

    protected void gvOnnetFraudGeneratedCalls_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {

        if (e.Item is GridItem)
        {
            GridItem item = (GridItem)e.Item;
            if (item.ItemType == GridItemType.AlternatingItem || item.ItemType == GridItemType.Item)
            {
                item.ForeColor = System.Drawing.Color.Red;
            }

        }
    }

    protected void gvOfnetFraudGeneratedCalls_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {

        if (e.Item is GridItem)
        {
            GridItem item = (GridItem)e.Item;
            if (item.ItemType == GridItemType.AlternatingItem || item.ItemType == GridItemType.Item)
            {
                item.ForeColor = System.Drawing.Color.Red;
            }

        }
    }

    protected void ddlSearchOperatorReply_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        Populate();
    }

    protected void gvAllGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {

        if (rtsMain.SelectedIndex == 5)
        {
            gvAllGeneratedCalls.VirtualItemCount = Vanrise.Fzero.Bypass.GeneratedCall.GetAllCases_LoadonDemand_Count(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, false, DifferenceinGMT, gvAllGeneratedCalls.CurrentPageIndex + 1, gvAllGeneratedCalls.PageSize);
            gvAllGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetAllCases_LoadonDemand(ConfigurationManager.AppSettings["ClientID"].ToInt(), 0, fromDateTime, toDateTime, false, DifferenceinGMT, gvAllGeneratedCalls.CurrentPageIndex + 1, gvAllGeneratedCalls.PageSize);
        }
    }

    protected void gvGeneratedCalls_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {

        if (e.Item is GridItem)
        {
            GridItem item = (GridItem)e.Item;
            Label lbl = (Label)item.FindControl("lblMobileOperatorFeedbackName");

            if (lbl != null)
            {

                lbl.Font.Bold = true;
                if (lbl.Text == "Blocked")
                {
                    item.BackColor = System.Drawing.Color.Gray;
                }

                else if (lbl.Text == "Rejected")
                {
                    item.BackColor = System.Drawing.Color.Pink;
                }
              

            }







        }
    }

    protected void gvAllGeneratedCalls_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
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
                    item.ForeColor = System.Drawing.Color.Red;
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

    protected void rdTo_Command(object sender, DockCommandEventArgs e)
    {
        Populate();
    }

    protected void rcFrom_SelectionChanged(object sender, Telerik.Web.UI.Calendar.SelectedDatesEventArgs e)
    {
        Populate();
    }

    protected void rtFrom_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        Populate();
    }

    protected void rtTo_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        Populate();
    }

   

    #endregion
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Populate();
        rblDateRange.SelectedValue = "0";
    }
}