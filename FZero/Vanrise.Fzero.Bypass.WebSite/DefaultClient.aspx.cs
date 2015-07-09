using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Vanrise.CommonLibrary;

using Telerik.Web.UI;
using Telerik.Charting;
using Vanrise.Fzero.Bypass;
using System.Drawing;

public partial class DefaultClient : BasePage
{
    #region Methods

    public DateTime fromDateTime
    {
        get
        {
            if (rdpFrom.SelectedDate == null)
            {
                rdpFrom.SelectedDate = DateTime.Now.Date;
            }
            return rdpFrom.SelectedDate.Value;
        }
    }

    public DateTime toDateTime
    {
        get
        {
            if (rdpTo.SelectedDate == null)
            {
                rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }
            return rdpTo.SelectedDate.Value;
        }
    }

    private void FillCombos()
    {
        Manager.BindCombo(ddlClients, Vanrise.Fzero.Bypass.Client.GetAllClients(), "Name", "Id", "0", "0");
        ddlClients.Items[0].Visible=false;
        ddlClients.Items.FindItemByValue("1").Selected = true;
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.Dashboard))
            RedirectTo("ReportedCases.aspx");

    }

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.Dashboard;

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


      

    }

    public bool ReportingStatusVisible(int ReportingStatusID)
    {
        if (ReportingStatusID == (int)Vanrise.Fzero.Bypass.Enums.ReportingStatuses.Reported)
        {
            return true;
        }
        return false;
    }

    private void Populate()
    {
        gvCases.Rebind();
        gvClientSummary.Rebind();
        gvCarrier.Rebind();
        gvOriginationNetwork.Rebind();

        switch (rtsMain.SelectedIndex)
        {
            case 2:
                List<Vanrise.Fzero.Bypass.prGetTimeActiveonNetwork_Result> lstView_TimeActiveonNetwork = Vanrise.Fzero.Bypass.prGetTimeActiveonNetwork_Result.prGetTimeActiveonNetwork((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), CurrentUser.MobileOperatorID, fromDateTime, toDateTime);
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

            

        }
    }

    #endregion

    #region Events

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Populate();
        ddlDateRange.SelectedValue = "0";
    }

    protected void gvClientSummary_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvClientSummary.DataSource = Dashboard.listprGetSummaryClient_Result;
    }

    protected void gvCases_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        Dashboard.GetDashboard((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()),0, fromDateTime, toDateTime,true); 
        gvCases.DataSource = Dashboard.listprSummary_Result;
    }

    protected void ddlDateRange_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        switch (ddlDateRange.SelectedValue)
        {

            case "0"://Specific Date

                break;

            case "1"://Today
                rdpFrom.SelectedDate = DateTime.Now.Date;
                rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                Populate();
                break;

            case "2"://Yesterday
                rdpFrom.SelectedDate = DateTime.Now.Date.AddDays(-1);
                rdpTo.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                Populate();
                break;

            case "3"://This Week
                DateTime input = DateTime.Now.Date;
                int delta = DayOfWeek.Monday - input.DayOfWeek;
                DateTime monday = input.AddDays(delta);
                rdpFrom.SelectedDate = monday;
                rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                Populate();

                break;

            case "4"://This Month
                rdpFrom.SelectedDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                Populate();
                break;

            case "5"://This Year
                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                rdpFrom.SelectedDate = firstDay;
                rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                Populate();
                break;

            case "6"://Last Week
                DateTime inputlastweek = DateTime.Now.Date.AddDays(-7);
                int deltalastweek = DayOfWeek.Monday - inputlastweek.DayOfWeek;
                DateTime mondaylastweek = inputlastweek.AddDays(deltalastweek);
                rdpFrom.SelectedDate = mondaylastweek;
                rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                Populate();

                break;

            case "7"://Last Month
                DateTime LastMonthLastDate = DateTime.Today.AddDays(0 - DateTime.Today.Day).AddHours(12).AddSeconds(-1);
                DateTime LastMonthFirstDate = DateTime.Today.AddDays(0 - DateTime.Today.Day).AddDays(1 - DateTime.Today.AddDays(0 - DateTime.Today.Day).Day);

                rdpFrom.SelectedDate = LastMonthFirstDate;
                rdpTo.SelectedDate = LastMonthLastDate;
                Populate();
                break;


        }


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

    protected void ddlClients_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        Populate();
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
       
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();


        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            rdpFrom.SelectedDate = DateTime.Now.Date;
            rdpTo.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
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
            int MobileOperator = CurrentUser.MobileOperatorID;
            gvOnnetFraudGeneratedCalls.VirtualItemCount = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand_Count((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), MobileOperator, fromDateTime, toDateTime, 1, true, 0, gvOnnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOnnetFraudGeneratedCalls.PageSize);
            gvOnnetFraudGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), MobileOperator, fromDateTime, toDateTime, 1, true, 0, gvOnnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOnnetFraudGeneratedCalls.PageSize);
        }
    }

    protected void gvOfnetFraudGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (rtsMain.SelectedIndex == 4)
        {
            int MobileOperator = CurrentUser.MobileOperatorID;
            gvOfnetFraudGeneratedCalls.VirtualItemCount = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand_Count((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), MobileOperator, fromDateTime, toDateTime, 2, true, 0, gvOfnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOfnetFraudGeneratedCalls.PageSize);
            gvOfnetFraudGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases_LoadonDemand((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), MobileOperator, fromDateTime, toDateTime, 2, true, 0, gvOfnetFraudGeneratedCalls.CurrentPageIndex + 1, gvOfnetFraudGeneratedCalls.PageSize);
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
            gvAllGeneratedCalls.VirtualItemCount = Vanrise.Fzero.Bypass.GeneratedCall.GetAllCases_LoadonDemand_Count((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), 0, fromDateTime, toDateTime,  true, 0, gvAllGeneratedCalls.CurrentPageIndex + 1, gvAllGeneratedCalls.PageSize);
            gvAllGeneratedCalls.DataSource = Vanrise.Fzero.Bypass.GeneratedCall.GetAllCases_LoadonDemand((ddlClients.SelectedValue.ToInt() == 0 ? 1 : ddlClients.SelectedValue.ToInt()), 0, fromDateTime, toDateTime,  true, 0, gvAllGeneratedCalls.CurrentPageIndex + 1, gvAllGeneratedCalls.PageSize);
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

   

    protected void rdpFrom_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        Populate();
        ddlDateRange.SelectedValue = "0";
    }

    protected void rdpTo_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        Populate();
        ddlDateRange.SelectedValue = "0";
    }

    #endregion
}
