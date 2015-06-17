using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vanrise.Fzero.MobileCDRAnalysis;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;
using Vanrise.CommonLibrary;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.IO;
using System.Net;
using Telerik.Web.UI;


public partial class ReportManagement : BasePage
{
    #region Properties

   
    private void FillControls()
    {
        List<Suspicion_Level> suspection_Levels = new List<Suspicion_Level>();
        suspection_Levels = Suspicion_Level.GetAll();
        int StartegyId;
        int.TryParse(Request.QueryString["Strategyid"], out StartegyId);
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
            FillCombos();
            SetPermissions();
            SetDetailsVisible("Main");
            FillControls();

            if (Request.QueryString["ReportNumber"] != null )
            {
                txtReportNumber.Text = Request.QueryString["ReportNumber"];
            }


            gvData.Rebind();
        }
    }

    private void SetCaptions()
    {
        ((MasterPage)this.Master).PageHeaderTitle = "Report Management";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        gvData.Rebind();
    }


    protected void gvData_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string reportNumber = txtReportNumber.Text;
        DateTime? fromDate = dtpFromDate.SelectedDate;
        DateTime? toDate = dtpToDate.SelectedDate;
        string SubscriberNumber = txtSubscriberNumber.Text;

        gvData.DataSource = Vanrise.Fzero.MobileCDRAnalysis.Report.GetList(reportNumber, SubscriberNumber, fromDate, toDate);
    }

    protected void gvData_ItemCommand(object sender, GridCommandEventArgs e)
    {
      
        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Details":
                    if (e.CommandArgument == null)
                         return;

                    hfReportID.Value = e.CommandArgument.ToString();
                    FillDetails(hfReportID.Value.ToInt());
                    break;

                case "Export":
                     if (e.CommandArgument == null)
                         return;

                    hfReportID.Value = e.CommandArgument.ToString();
                    ExportReportToExcel(hfReportID.Value + ".xls", hfReportID.Value.ToInt());

                    string path = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], hfReportID.Value + ".xls");
                    WebClient req = new WebClient();
                    HttpResponse response = HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=\"" + path + "\"");
                    byte[] data = req.DownloadData(path);
                    response.BinaryWrite(data);
                    response.End();


                    break;


                case "Send":
                    if (e.CommandArgument == null)
                         return;

                    hfReportID.Value = e.CommandArgument.ToString();
                    Vanrise.Fzero.MobileCDRAnalysis.Report report = Vanrise.Fzero.MobileCDRAnalysis.Report.Load(hfReportID.Value.ToString());
                    string ReportID = "CA" + report.ReportNumber + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2");
                    EmailManager.SendReporttoITPC(ExportReportToExcel(hfReportID.Value.ToString() + ".xls", hfReportID.Value.ToInt()), ReportID, "FMS_Profile");
                    report.SentDate = DateTime.Now;
                    report.SentBy = CurrentUser.User.ID;
                    report.ReportingStatusID = (int)Enums.ReportingStatuses.Sent;
                    report.ReportID = ReportID;
                    Vanrise.Fzero.MobileCDRAnalysis.Report.Save(report);
                    gvData.Rebind();
                    ShowAlert("Report sent successfully");
                    break;



                case "Remove":
                    if (e.CommandArgument == null)
                         return;

                    hfReportID.Value = e.CommandArgument.ToString();

                    if (Vanrise.Fzero.MobileCDRAnalysis.Report.Delete(hfReportID.Value.ToInt()))
                    {
                        gvData.Rebind();
                    }
                    else
                    {
                        ShowError("Unable to Delete!.");
                    }
                    break;


            }
        }
    }

    #endregion

    #region Methods
   
    private void FillCombos()
    {
        Manager.BindCombo(ddlReportingStatus,Vanrise.Fzero.MobileCDRAnalysis.ReportingStatu.GetAll(), "Name", "Id", "Choose Status ...", "");
    }
    private void SetPermissions()
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ReportManagement))
            RedirectToAuthenticationPage();

        btnAdd.Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageReport);
        gvData.Columns[gvData.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ManageReport);
    }
    private bool IsValidData()
    {
        return true;
    }
    private void SetDetailsVisible(string flag)
    {
        
        if (flag == "Main")
        {
            divFilter.Visible = true;
            divData.Visible = true;
            divDetails.Visible = false;
            divAddtion.Visible = false;
        }
        else if (flag == "Details")  
        {
            divFilter.Visible = false;
            divData.Visible = false;
            divDetails.Visible = true;
            divAddtion.Visible = false;
        }
        else if (flag == "Addition")
        {
            divFilter.Visible = false;
            divData.Visible = false;
            divDetails.Visible = false;
            divAddtion.Visible = true;
        }

    }
    private void ClearFiltrationFields()
    {
        txtReportNumber.Text = "";
        ddlReportingStatus.SelectedIndex = -1;
        txtSubscriberNumber.Text = "";
        dtpFromDate.Clear();
        dtpToDate.Clear();
    }
    private void FillDetails(int reportId)
    {
        ClearDetails();
        SetDetailsVisible("Details");

        Vanrise.Fzero.MobileCDRAnalysis.Report report = Vanrise.Fzero.MobileCDRAnalysis.Report.Load(reportId);
        txtDetailsReportStatus.Text = report.ReportingStatu.Name;
        txtDetailsReportNumber.Text = report.ReportNumber.ToString();
        txtDetailsCreationDate.Text = report.ReportDate.ToString();
       
        List<ReportDetail> reportDetails = ReportDetail.GetList(reportId);
        gvDetails.DataSource = reportDetails;
        gvDetails.DataBind();

    }
    protected void gvDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;

        hfReportDetailID.Value = e.CommandArgument.ToString();

        

        if (e.CommandArgument != null)
        {
            switch (e.CommandName)
            {
                case "Remove":

                    if (ReportDetail.Delete(hfReportDetailID.Value.ToInt()))
                    {
                        List<ReportDetail> reportDetails = ReportDetail.GetList(hfReportID.Value.ToInt());
                        gvDetails.DataSource = reportDetails;
                        gvDetails.DataBind();
                    }
                    else
                    {
                        ShowError( "Unable to Delete!.");
                    }
                    break;


            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearDetails();
        SetDetailsVisible("Main");
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        ClearDetails();
        SetDetailsVisible("Main");
    }
    protected void ClearDetails()
    {


    }
    protected void lnkSearchNumber_Click(object sender, EventArgs e)
    {
        ClearDetails();
    }
    private string ExportReportToExcel(string reportName, int ReportID)
    {
        ReportViewer rvToOperator = new ReportViewer();

        rvToOperator.LocalReport.ReportPath = Path.Combine(string.Empty, @"Reports\rptReportedNumbers.rdlc");

        ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("DSReportDetails", ReportDetail.GetList(ReportID));
        rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);

        rvToOperator.LocalReport.Refresh();

        Warning[] warnings;
        string[] streamids;
        string mimeType;
        string encoding;
        string filenameExtension;
        byte[] bytes = rvToOperator.LocalReport.Render(
           "Excel", null, out mimeType, out encoding, out filenameExtension,
            out streamids, out warnings);

        string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        using (var fs = new FileStream(filename, FileMode.Create))
        {
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        return filename;
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearFiltrationFields();
        gvData.Rebind();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        SetDetailsVisible("Addition");

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        Vanrise.Fzero.MobileCDRAnalysis.Report rt = new Vanrise.Fzero.MobileCDRAnalysis.Report();
        rt.Description = txtDescription.Text;
        rt.UserId = CurrentUser.User.ID;
        rt.ReportingStatusID = (int)Enums.ReportingStatuses.ToBeSent;
        Vanrise.Fzero.MobileCDRAnalysis.Report.SetReportVariables(rt);

        if (!Vanrise.Fzero.MobileCDRAnalysis.Report.Save(rt))
        {
            ShowError( "An error occured when trying to save data, kindly try to save later.");
            return;
        }

        SetDetailsVisible("Main");
        gvData.Rebind();

    }

    #endregion

  
}