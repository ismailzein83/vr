using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.CommonLibrary;
using System.Data;
using Telerik.Web.UI;
using System.Configuration;

public partial class SuspectionAnalysis : BasePage
{
    #region Properties

    List<vwSuspectionAnalysi> CurrentSuspectionList
    {
        get
        {
            if (Session["SuspectionOccurance.CurrentSuspectionList"] == null
                || !(Session["SuspectionOccurance.CurrentSuspectionList"] is List<vwSuspectionAnalysi>))
                GetList();
            return (List<vwSuspectionAnalysi>)Session["SuspectionOccurance.CurrentSuspectionList"];
        }
        set
        {
            Session["SuspectionOccurance.CurrentSuspectionList"] = value;
        }
    }

    public static void BindCombo<T>(RadComboBox ddl, List<T> source, string DataTextField, string DataValueField, string DefaultTextField, string DefaultValueField)
    {
        ddl.DataTextField = DataTextField;
        ddl.DataValueField = DataValueField;
        ddl.DataSource = source;
        ddl.Sort = RadComboBoxSort.Ascending;
        ddl.DataBind();
        ddl.Items.Sort();
        if (DataTextField != string.Empty && DataValueField != string.Empty)
            ddl.Items.Insert(0, new RadComboBoxItem(DefaultTextField, DefaultValueField));

    }


    //-----------------
    int id { get { return (int)ViewState["Id"]; } set { ViewState["Id"] = value; } }


    private void FillControls()
    {
        BindCombo(ddlSearchStrategy, Strategy.GetAll(), "Name", "Id", "", "0");
       
        List<Suspicion_Level> suspection_Levels = new List<Suspicion_Level>();
        suspection_Levels = Suspicion_Level.GetAll();
        

        int StartegyId;
        int.TryParse(Request.QueryString["Strategyid"], out StartegyId);

        if ((StartegyId) != 0)
        {
            ddlSearchStrategy.SelectedValue = StartegyId.ToString();
        }

        List<Report> lr = Report.GetTop(20);
        Manager.BindCombo(ddlreports, lr, "ReportNumber", "Id", "", "0");
        

    }

    #endregion

    #region Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsAuthenticated)
            RedirectToAuthenticationPage();

        if (!IsPostBack)
        {
            SetCaptions();
            SetPermissions();
            SetDetailsVisible(false);
            FillControls();
            LoadData();
            ddlSearchStrategy.SelectedValue = "1";
            dtpFromDate.SelectedDate = DateTime.Now.AddYears(-1);
            dtpToDate.SelectedDate = DateTime.Now;
        }
    }






    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.SuspectionAnalysis))
            RedirectToAuthenticationPage();

        ddlreports.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.AddCasestoReports);
        lnkReport.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.AddCasestoReports);
        gvData.Columns[gvData.Columns.Count - 3].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ReportManagement);
        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ViewCaseDetails);
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Suspicion Analysis";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadData();
    }
   
   
    #endregion

    #region Methods
    private bool IsValidData()
    {
        return true;
    }
    private void SetDetailsVisible(bool flag)
    {
        divFilter.Visible = !flag;
        divData.Visible = !flag;
        divDetails.Visible = flag;
    }

    private void ClearFiltrationFields()
    {
        ddlSearchStrategy.SelectedValue = "1";
        dtpFromDate.SelectedDate = DateTime.Now.AddYears(-1);
        dtpToDate.SelectedDate = DateTime.Now;
    }

    private void LoadData()
    {

        GetList();
        gvData.PageIndex = 0;
        FillGrid();
    }
    private string Getchecklist()
    {
        string st = "";
        // chkClean  chkSuspicious  chkHighlySuspicious chkFraud
        if (cblSuspiciousLevel.Items.FindByValue("Clean").Selected == true)
        {
            st = "1";
        }
        if (cblSuspiciousLevel.Items.FindByValue("Suspicious").Selected == true)
        {
            if (st == "")
                st = "2";
            else
                st =st + ",2";
        }
        if (cblSuspiciousLevel.Items.FindByValue("HighlySuspicious").Selected == true)
        {
            if (st == "")
                st = "3";
            else
                st = st + ",3";
        }
        if (cblSuspiciousLevel.Items.FindByValue("Fraud").Selected == true)
        {
            if (st == "")
                st = "4";
            else
                st = st + ",4";
        }
       

        return st;
    }
    private void GetList()
    {
        string st = Getchecklist();

        if (st == "")
        {
            st = "2,3,4";
            cblSuspiciousLevel.Items.FindByValue("Suspicious").Selected = true;
            cblSuspiciousLevel.Items.FindByValue("HighlySuspicious").Selected = true;
            cblSuspiciousLevel.Items.FindByValue("Fraud").Selected = true;
        }
        int strategyId = 0;
        if (ddlSearchStrategy.SelectedItem != null)
            strategyId = int.Parse(ddlSearchStrategy.SelectedItem.Value);
        int minimumOccurance = 0;
        if (int.TryParse(txtMinimumOccurance.Text, out minimumOccurance) == false)
        {
            minimumOccurance = 1;
            txtMinimumOccurance.Text = "1";
        }


        DateTime? fromDate = dtpFromDate.SelectedDate;
        DateTime? toDate = dtpToDate.SelectedDate;

        CurrentSuspectionList = SuspectionOccurance.GetList(strategyId, fromDate, toDate, st, minimumOccurance, ConfigurationManager.AppSettings["SQLType"]);

    }
    private void FillGrid()
    {

        gvData.DataSource = CurrentSuspectionList;
        gvData.DataBind();
    }
    private void FillDetails(string subscriberNumber)
    {
       
        IEnumerable<string> sl = NormalCDR.GetRelatedList(subscriberNumber);
        lstRelatedNumber.DataSource = sl;
        lstRelatedNumber.DataBind();
        lstRelatedNumber.SelectedIndex=lstRelatedNumber.Items.IndexOf(new ListItem(subscriberNumber));
        SetDetailsVisible(true);
        txtSubscriberNumber.Text = subscriberNumber;
        Strategy st = Strategy.Load(id);
        txtStrategy.Text = st.Name;
        dtpNumberFromDate.SelectedDate = dtpFromDate.SelectedDate;
        dtpNumberToDate.SelectedDate = dtpToDate.SelectedDate;

        cleardetails();
        //SearchDetails(lstRelatedNumber.SelectedValue);

      
    }

     private void SearchDetails ( string subscriberNumber) 
     {
        lblNumberInfo.Text = "Subscriber Number : " + subscriberNumber ;

        if (chkThresholds.Checked == true)
        {
            gvSubscriberThreshold.Rebind();
            RadPanelBar1.Items[0].Visible = true;
            RadPanelBar1.Items[0].Text="Subscriber Thresholds (" + gvSubscriberThreshold.Items.Count.ToString() +")";
        }
        else
        {
            RadPanelBar1.Items[0].Visible = false;
        }

        if (chkNormal.Checked == true)
        {
            gvNormalCdr.Rebind();
            RadPanelBar1.Items[1].Visible = true;
            RadPanelBar1.Items[1].Text = "Normal CDRs (" + gvNormalCdr.Items.Count.ToString() + ")";
        }
        else
        {
            RadPanelBar1.Items[1].Visible = false;
        }


        if (chkValues.Checked == true)
        {
            gvSubscriberValues.Rebind();
            RadPanelBar1.Items[2].Visible = true;
            RadPanelBar1.Items[2].Text = "Subscriber Values (" + gvSubscriberValues.Items.Count.ToString() + ")";
        }
        else
        {
            RadPanelBar1.Items[2].Visible = false;
        }


        if (chkDailyProfile.Checked == true)
        {
            gvDailyProfile.Rebind();
            RadPanelBar1.Items[3].Visible = true;
            RadPanelBar1.Items[3].Text = "Daily Number Profiles (" + gvDailyProfile.Items.Count.ToString() + ")";
        }
        else
        {
            RadPanelBar1.Items[3].Visible = false;
        }


        if (chkMonthlyProfile.Checked == true)
        {
            gvMonthlyProfile.Rebind();
            RadPanelBar1.Items[4].Visible = true;
            RadPanelBar1.Items[4].Text = "Monthly Number Profiles (" + gvMonthlyProfile.Items.Count.ToString() + ")";
        }
        else
        {
            RadPanelBar1.Items[4].Visible = false;
        }


        if (chkHourlyProfile.Checked == true)
        {
            gvHourlyProfile.Rebind();
            RadPanelBar1.Items[5].Visible = true;
            RadPanelBar1.Items[5].Text = "Hourly Number Profiles (" + gvHourlyProfile.Items.Count.ToString() + ")";
        }
        else
        {
            RadPanelBar1.Items[5].Visible = false;
        }


     }


 
    #endregion


     protected void btnClear_Click(object sender, EventArgs e)
     {
         ClearFiltrationFields();
         LoadData();
     }


    protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Modify":
                    id = e.CommandArgument.ToString().Split('-').First().ToInt();
                    FillDetails(e.CommandArgument.ToString().Substring(e.CommandArgument.ToString().IndexOf("-") + 1, e.CommandArgument.ToString().Length - e.CommandArgument.ToString().IndexOf("-") - 1));
                    break;

                case "Report":
                    Response.Redirect("ReportManagement.aspx?ReportNumber=" + e.CommandArgument.ToString());
                    break;
                
            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        cleardetails();
        SetDetailsVisible(false);
    }
   
    protected void cleardetails()
    {
        lblNumberInfo.Text = "";

        gvSubscriberThreshold.DataSource = null;
        gvSubscriberThreshold.DataBind();
        
        gvNormalCdr.DataSource = null;
        gvNormalCdr.DataBind();
       
        gvSubscriberValues.DataSource = null;
        gvSubscriberValues.DataBind();

        gvDailyProfile.DataSource = null;
        gvDailyProfile.DataBind();

        gvMonthlyProfile.DataSource = null;
        gvMonthlyProfile.DataBind();

        gvHourlyProfile.DataSource = null;
        gvHourlyProfile.DataBind();

     
        RadPanelBar1.Items[0].Visible = false;
        RadPanelBar1.Items[1].Visible = false;
        RadPanelBar1.Items[2].Visible = false;
        RadPanelBar1.Items[3].Visible = false;
        RadPanelBar1.Items[4].Visible = false;
        RadPanelBar1.Items[5].Visible = false;

        RadPanelBar1.CollapseAllItems();
    }


    protected void lnkSearchNumber_Click(object sender, EventArgs e)
    {
        cleardetails();
        SearchDetails(lstRelatedNumber.SelectedValue);
       
    }
    protected void lnkReport_Click(object sender, EventArgs e)
    {
        Report report =new Report();
        report = Report.Load(int.Parse(ddlreports.SelectedItem.Value));
        //if (report !=null && report.Id != 0 )
        //{
        //    List<ReportDetail> reportDetails = GetSelectedReportDetails(GetSelectedRecords(), report);
        //    foreach (ReportDetail rd in reportDetails)
        //    { 
        //      rd.ReportId=report.Id;
        //    }
        //    ReportDetail.Save(reportDetails);
        //    LoadData();
        //}
        
    }

    protected DataTable GetSelectedRecords()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[3] { new DataColumn("StrategyId"), new DataColumn("SubscriberNumber"), new DataColumn("lastReport") });
        foreach (GridViewRow row in gvData.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkReport = (row.Cells[1].FindControl("chkReport") as CheckBox);
                if (chkReport.Checked && row.Cells[3].Text.Trim() !=string.Empty)
                {
                    string StrategyId = row.Cells[1].Text;
                    string SubscriberNumber = row.Cells[3].Text;
                    string lastReport = row.Cells[4].Text;
                    dt.Rows.Add(StrategyId, SubscriberNumber, lastReport);
                }
            }
        }

        return dt;
    }



    protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[4].Text == "Highly Suspicious")
            {
                e.Row.Cells[4].ForeColor = System.Drawing.Color.OrangeRed;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.OrangeRed;
            }
            else if (e.Row.Cells[4].Text == "Suspicious")
            {
                e.Row.Cells[4].ForeColor = System.Drawing.Color.BlueViolet;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.BlueViolet;
            }
            else if (e.Row.Cells[4].Text == "Fraud")
            {
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                e.Row.Cells[4].ForeColor = System.Drawing.Color.Green;
                e.Row.Cells[3].ForeColor = System.Drawing.Color.Green;
            }
           
        }
    }


    protected void gvHourlyProfile_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvHourlyProfile.DataSource = NumberProfile.GetHourlyList(lstRelatedNumber.SelectedValue, dtpNumberFromDate.SelectedDate, dtpNumberToDate.SelectedDate);

    }

    protected void gvMonthlyProfile_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvMonthlyProfile.DataSource = NumberProfile.GetMonthlyList(lstRelatedNumber.SelectedValue, dtpNumberFromDate.SelectedDate, dtpNumberToDate.SelectedDate);

    }

    protected void gvDailyProfile_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvDailyProfile.DataSource = NumberProfile.GetDailyList(lstRelatedNumber.SelectedValue, dtpNumberFromDate.SelectedDate, dtpNumberToDate.SelectedDate);

    }

    protected void gvSubscriberValues_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        //gvSubscriberValues.DataSource = Subscriber_Values.GetList(id, lstRelatedNumber.SelectedValue);

    }

    protected void gvNormalCdr_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvNormalCdr.DataSource = NormalCDR.GetList(lstRelatedNumber.SelectedValue);

    }

    protected void gvSubscriberThreshold_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        gvSubscriberThreshold.DataSource = SubscriberThreshold.GetList(id, lstRelatedNumber.SelectedValue);

    }

}