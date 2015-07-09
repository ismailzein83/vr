using System;
using System.Web.UI.WebControls;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Telerik.Web.UI;
using System.Data.Entity;


public partial class Default : BasePage
{
    #region Methods

    private void SetCaptions()
    {
        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.MonitoringnTracking;
        rdpFromLogDate.SelectedDate = DateTime.Today.Date;
        rdpToLogDate.SelectedDate = DateTime.Today.Date.AddDays(1);
    }

    private void ClearSearchForm()
    {
        rdpFromLogDate.Clear();
        rdpToLogDate.Clear();
    }

    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.MonitoringnTracking))
            PreviousPageRedirect();
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (CurrentUser.portalType != 2)
            RedirectTo("~/DefaultMobileOperator.aspx");

        if (!IsPostBack)
        {
            SetPermissions();
            SetCaptions();
            rdpFromLogDate.SelectedDate = DateTime.Now.Date;
            rdpToLogDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
        }
    }

    protected void gvCases_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;

        gvCases.DataSource = prCaseStatuses_Result.GetprCaseStatuses_Result(fromDate, toDate);
    }

    protected void gvClientCases_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;

        gvClientCases.DataSource = prClientCases_Result.prClientCases(fromDate, toDate);
    }

    protected void gvGeneratedRecieved_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;

        gvGeneratedRecieved.DataSource = prSourceGeneratesRecieves_Result.GetViewSourceGeneratesRecieves(fromDate, toDate);
    }

    protected void gvGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
       DateTime? fromDate = rdpFromLogDate.SelectedDate;
       DateTime? toDate = rdpToLogDate.SelectedDate;

       gvGeneratedCalls.DataSource = prSourceGenerates_Result.GetViewSourceGenerates(fromDate, toDate);
    }
    
    protected void gvReceivedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;

        gvReceivedCalls.DataSource = prSourceRecieves_Result.GetViewSourceRecieves(fromDate, toDate);
    }

    protected void gvImports_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;
        gvImports.DataSource = prImports_Result.GetViewImports(fromDate, toDate);
    }

    protected void gvReports_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;

        gvReports.DataSource = prReports_Result.GetprReports_Results(fromDate, toDate);
    }

    protected void gvLastImports_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvLastImports.DataSource = prLastImports_Result.GetViewLastImports();
    }

    protected void gvCompares_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DateTime? fromDate = rdpFromLogDate.SelectedDate;
        DateTime? toDate = rdpToLogDate.SelectedDate;

        gvCompares.DataSource = prComparisons_Result.GetViewComparisons(fromDate, toDate);
    }

    protected void gvCases_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridItem)
        {
            GridItem item = (GridItem)e.Item;
            Label lbl = (Label)item.FindControl("lblName");

            if (lbl != null)
            {
                lbl.Font.Bold = true;
                if (lbl.Text == "Fraud")
                {
                    item.ForeColor = System.Drawing.Color.Maroon;
                }
                else if (lbl.Text == "Suspect")
                {
                    item.ForeColor = System.Drawing.Color.OrangeRed;
                }
                else if (lbl.Text == "Distinct Fraud")
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

    protected void gvClientCases_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridItem)
        {
            GridItem item = (GridItem)e.Item;
            Label lbl = (Label)item.FindControl("lblStatus");

            if (lbl != null)
            {
                lbl.Font.Bold = true;
                if (lbl.Text == "Fraud")
                {
                    item.ForeColor = System.Drawing.Color.Maroon;
                }
                else if (lbl.Text == "Suspect")
                {
                    item.ForeColor = System.Drawing.Color.OrangeRed;
                }
                else if (lbl.Text == "Distinct Fraud")
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

    protected void gvLastImports_ItemDataBound(object sender, GridItemEventArgs e)
    {

        if ((e.Item.ItemType == GridItemType.Item) || (e.Item.ItemType == GridItemType.AlternatingItem))
        {

             string _No = gvLastImports.MasterTableView.DataKeyValues[e.Item.ItemIndex]["DiffHour"].ToString();

            if (_No.ToInt() <= 1)
            {
                e.Item.BackColor = System.Drawing.Color.Green;
                e.Item.ForeColor = System.Drawing.Color.White;
            }
            else if (_No.ToInt() > 1 && _No.ToInt() <= 4)
            {
                e.Item.BackColor = System.Drawing.Color.Orange;
                e.Item.ForeColor = System.Drawing.Color.White;
            }
            else if (_No.ToInt() > 4)
            {
                e.Item.BackColor = System.Drawing.Color.Red;
                e.Item.ForeColor = System.Drawing.Color.White;
            }

        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvClientCases.Rebind();
        gvCases.Rebind();
        gvCompares.Rebind();
        gvGeneratedCalls.Rebind();
        gvImports.Rebind();
        gvReports.Rebind();
        gvReceivedCalls.Rebind();
        gvGeneratedRecieved.Rebind();
        ddlDateRange.SelectedValue = "0";
    }

    

    protected void ddlDateRange_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        switch (ddlDateRange.SelectedValue)
        {

            case "0"://Specific Date

                break;

            case "1"://Today
                rdpFromLogDate.SelectedDate = DateTime.Now.Date;
                rdpToLogDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();
                break;

            case "2"://Yesterday
                rdpFromLogDate.SelectedDate = DateTime.Now.Date.AddDays(-1);
                rdpToLogDate.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();
                break;

            case "3"://This Week
                DateTime input = DateTime.Now.Date;
                int delta = DayOfWeek.Monday - input.DayOfWeek;
                DateTime monday = input.AddDays(delta);
                rdpFromLogDate.SelectedDate = monday;
                rdpToLogDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();

                break;

            case "4"://This Month
                rdpFromLogDate.SelectedDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                rdpToLogDate.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();
                break;

            case "5"://This Year
                int year = DateTime.Now.Year;
                DateTime firstDay = new DateTime(year, 1, 1);
                rdpFromLogDate.SelectedDate = firstDay;
                rdpToLogDate.SelectedDate = DateTime.Now.Date.AddSeconds(-1);
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();
                break;

            case "6"://Last Week
                DateTime inputlastweek = DateTime.Now.Date.AddDays(-7);
                int deltalastweek = DayOfWeek.Monday - inputlastweek.DayOfWeek;
                DateTime mondaylastweek = inputlastweek.AddDays(deltalastweek);
                rdpFromLogDate.SelectedDate = mondaylastweek;
                rdpToLogDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();

                break;

            case "7"://Last Month
                DateTime LastMonthLastDate = DateTime.Today.AddDays(0 - DateTime.Today.Day).AddHours(12).AddSeconds(-1);
                DateTime LastMonthFirstDate = DateTime.Today.AddDays(0 - DateTime.Today.Day).AddDays(1 - DateTime.Today.AddDays(0 - DateTime.Today.Day).Day);

                rdpFromLogDate.SelectedDate = LastMonthFirstDate;
                rdpToLogDate.SelectedDate = LastMonthLastDate;

           
                gvCases.Rebind();
                gvCompares.Rebind();
                gvGeneratedCalls.Rebind();
                gvImports.Rebind();
                gvReports.Rebind();
                gvReceivedCalls.Rebind();
                gvGeneratedRecieved.Rebind();
                gvClientCases.Rebind();
                break;


        }

    }

    #endregion
}