using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Telerik.Web.UI;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;

public partial class testingReports : BasePage
{
    #region Methods
    private void SetCaptions()
    {
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
        }
    }

    private void FillCombos()
    {

        List<Client> Clients = Vanrise.Fzero.Bypass.Client.GetAllClients();
        Manager.BindCombo(ddlSearchClient, Clients, "Name", "Id", "All Clients ...", "0");


        ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(Resources.Resources.AllDashes, "0"));
        foreach (MobileOperator i in Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList())
        {
            ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }


    }


    protected void btnSendReport_Click(object sender, EventArgs e)
    {
        string ReportID = txtReportID.Text;

        ReportParameter[] parameters = new ReportParameter[3];
        parameters[0] = new ReportParameter("ReportID", ReportID);
        parameters[1] = new ReportParameter("RecommendedAction", txtRecomnededAction.Text.Trim());
        

        ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(CurrentUser.User.ID)).User.Signature);
        rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


        ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
        rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

        ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", GeneratedCall.GetReportedCalls(ReportID, (Vanrise.Fzero.Bypass.MobileOperator.Load(ddlSearchMobileOperator.SelectedValue.ToInt()).User.GMT - SysParameter.Global_GMT)));
        rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);


        if (ddlSearchClient.SelectedValue.ToInt() == (int)Enums.Clients.ST)//-- Syrian Telecom
        {
            rvToOperator.LocalReport.ReportPath = "Reports\\rptToSyrianOperator.rdlc";
        }
        else if (ddlSearchClient.SelectedValue.ToInt() == (int)Enums.Clients.Zain)//-- Zain
        {
            rvToOperator.LocalReport.ReportPath = "Reports\\rptToZainOperator.rdlc";
        }
        else if (ddlSearchClient.SelectedValue.ToInt() == (int)Enums.Clients.ITPC)//-- ITPC
        {
            rvToOperator.LocalReport.ReportPath = "Reports\\rptToOperator.rdlc";
        }



        parameters[2] = new ReportParameter("HideSignature", "true");
        rvToOperator.LocalReport.SetParameters(parameters);
        rvToOperator.LocalReport.Refresh();
        ExportReportToExcel(ReportID + ".xls");





        parameters[2] = new ReportParameter("HideSignature", "false");
        rvToOperator.LocalReport.SetParameters(parameters);
        rvToOperator.LocalReport.Refresh();
        ExportReportToPDF(ReportID + ".pdf");


    }

    private string ExportReportToPDF(string reportName)
    {
        Warning[] warnings;
        string[] streamids;
        string mimeType;
        string encoding;
        string filenameExtension;
        byte[] bytes = rvToOperator.LocalReport.Render(
           "PDF", null, out mimeType, out encoding, out filenameExtension,
            out streamids, out warnings);

        string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        using (var fs = new FileStream(filename, FileMode.Create))
        {
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        return filename;
    }

    private string ExportReportToExcel(string reportName)
    {
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

    #endregion
}