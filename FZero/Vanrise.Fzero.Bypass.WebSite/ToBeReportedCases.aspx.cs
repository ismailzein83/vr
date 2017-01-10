using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.UI.WebControls;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;
using Microsoft.Reporting.WebForms;
using Telerik.Web.UI;




public partial class ToBeReportedCases : BasePage
{
    #region Properties


    #endregion

    #region Methods

    private void FillCombos()
    {

        List<Client> Clients = Vanrise.Fzero.Bypass.Client.GetAllClients();
        Manager.BindCombo(ddlSearchClient, Clients, "Name", "Id", "All Clients ...", "0");

        Manager.BindCombo(ddlSearchReceivedSource, Vanrise.Fzero.Bypass.Source.GetSourcesReceive(), "Name", "Id", Resources.Resources.AllDashes, "0");
        Manager.BindCombo(ddlSearchSource, Vanrise.Fzero.Bypass.Source.GetSourcesGenerate(), "Name", "Id", Resources.Resources.AllDashes, "0");
        Manager.BindCombo(ddlSearchStatus, Vanrise.Fzero.Bypass.Status.GetStatuses(), "Name", "Id", Resources.Resources.AllDashes, "0");
        ddlSearchStatus.Items.FindItemByValue(((int)Enums.Statuses.Clean).ToString()).Visible = false;
        ddlSearchStatus.Items.FindItemByValue(((int)Enums.Statuses.Pending).ToString()).Visible = false;
        ddlSearchStatus.Items.FindItemByValue(((int)Enums.Statuses.DistintFraud).ToString()).Visible = false;

        ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(Resources.Resources.AllDashes, "0"));
        foreach (MobileOperator i in Vanrise.Fzero.Bypass.MobileOperator.GetMobileOperatorsList())
        {
            ddlSearchMobileOperator.Items.Add(new RadComboBoxItem(i.User.FullName, i.ID.ToString()));
        }


    }

    private void SetCaptions()
    {

        ((MasterPage)Master).PageHeaderTitle = Resources.Resources.ToBeReportedCases;


        int columnIndex = 1;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CaseID;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.GeneratedBy;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.ReceivedBy;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.TotalDuration;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.Status;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CalledNumber;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.CLI;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.DateTime;
        gvGeneratedCalls.Columns[columnIndex++].HeaderText = Resources.Resources.IndirectCLI;



    }

    private void ClearSearchForm()
    {
        ddlSearchStatus.SelectedIndex = 0;
        ddlSearchSource.SelectedIndex = 0;
        ddlSearchMobileOperator.SelectedIndex = 0;
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
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ReporttoOperator))
            PreviousPageRedirect();

        gvGeneratedCalls.Columns[gvGeneratedCalls.Columns.Count - 1].Visible = CurrentUser.HasPermission(Enums.SystemPermissions.ViewResultedCalls);
    }


    #endregion

    #region Events

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
        int Client = ddlSearchClient.SelectedValue.ToInt();
        int MobileOperator = ddlSearchMobileOperator.SelectedValue.ToInt();
        int Status = ddlSearchStatus.SelectedValue.ToInt();

        if (MobileOperator == 0)
        {
            ShowError("Mobile Operator Should be Specified");
        }
        else if (Client == 0)
        {
            ShowError("Client Should be Specified");
        }
        else if (Status == 0)
        {
            ShowError("Status Should be Specified");
        }
        else
        {
            if (ddlSearchClient.SelectedValue.ToInt() == 3)
            {
                txtRecomnededAction.Text = "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
            }
            else
            {
                txtRecomnededAction.Text = "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fraudulent Calls as they were terminated to your Network and did not pass legally through ITPC's IGW.";
            }

            gvGeneratedCalls.Rebind();

        }

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {

    }

    protected void gvGeneratedCalls_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        int ClientID = ddlSearchClient.SelectedValue.ToInt();
        int Source = ddlSearchSource.SelectedValue.ToInt();
        int ReceivedSource = ddlSearchReceivedSource.SelectedValue.ToInt();
        int CLIMobileOperatorID = 0;
        int B_NumberMobileOperatorID = 0;

        if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Fraud)
        {
            CLIMobileOperatorID = ddlSearchMobileOperator.SelectedValue.ToInt();
        }
        else if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Suspect)
        {
            B_NumberMobileOperatorID = ddlSearchMobileOperator.SelectedValue.ToInt();
        }
        int Status = ddlSearchStatus.SelectedValue.ToInt();

        if (CLIMobileOperatorID == 0 && B_NumberMobileOperatorID == 0)
        {
            gvGeneratedCalls.DataSource = new List<prGetToBeReported_Result>();
        }
        else
        {
            gvGeneratedCalls.DataSource = prGetToBeReported_Result.GetToBeReportedCalls(Source, ReceivedSource, CLIMobileOperatorID, B_NumberMobileOperatorID, Status, ClientID);
        }

    }

    protected void gvGeneratedCalls_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandArgument == null)
            return;
        string[] arg = new string[3];
        arg = e.CommandArgument.ToString().Split(';');

        int Id = 0;
        int UserId = 0;
        string FullName = string.Empty;

        if (arg.Length == 3)
        {
            Id = Manager.GetInteger(arg[0]);
            UserId = Manager.GetInteger(arg[1]);
            FullName = arg[2];
        }

        switch (e.CommandName)
        {
            case "View":
                wucGeneratedCallInformation.GeneratedCallId = Id.ToString();
                wucGeneratedCallInformation.FillData(prVwGeneratedCall_Result.View(Id));
                break;

            case "Ignore":
                GeneratedCall.UpdateReportingStatus(Id, (int)Enums.ReportingStatuses.Ignored);
                gvGeneratedCalls.Rebind();
                break;


            case "Report":
                wucGeneratedCallInformation.GeneratedCallId = Id.ToString();
                wucGeneratedCallInformation.FillData(prVwGeneratedCall_Result.View(Id));
                break;
        }
    }

    protected void btnSendReport_Click(object sender, EventArgs e)
    {
        if (HttpHelper.CheckInternetConnection("mail.vanrise.com", 26))
        {
            int Client = ddlSearchClient.SelectedValue.ToInt();
            int MobileOperator = ddlSearchMobileOperator.SelectedValue.ToInt();
            int Status = ddlSearchStatus.SelectedValue.ToInt();

            if (MobileOperator == 0)
            {
                ShowError("Mobile Operator Should be Specified");
            }
            else if (Client == 0)
            {
                ShowError("Client Should be Specified");
            }
            else if (Status == 0)
            {
                ShowError("Status Should be Specified");
            }
            else
            {


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
                    ShowError("Specify the cases to be reported");
                    return;
                }
                MobileOperator mobileOperator = Vanrise.Fzero.Bypass.MobileOperator.Load(MobileOperator);


                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();
                Vanrise.Fzero.Bypass.Report reportSecurity = new Vanrise.Fzero.Bypass.Report();

                report.ApplicationUserID = CurrentUser.ApplicationUserID;
                report.SentDateTime = DateTime.Now;
                report.RecommendedAction = txtRecomnededAction.Text.Trim();


                string ReportID;

                string ReportIDBeforeCounter = "FZ" + ddlSearchMobileOperator.SelectedItem.Text.Substring(0, 1) + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
                Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);

                if (LastReport == null)
                {
                    ReportID = ReportIDBeforeCounter + "0001";
                }
                else
                {
                    ReportID = ReportIDBeforeCounter + (LastReport.ReportID.Replace("- Repeated Numbers", "").Substring(9).ToInt() + 1).ToString("D4");
                }


                report.ReportID = ReportID;



                if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Suspect)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                    reportSecurity.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                }
                else if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Fraud)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

                    reportSecurity.RecommendedActionID = (int)Enums.RecommendedAction.Block;
                }


                GeneratedCall.SendReport(ListIds, Vanrise.Fzero.Bypass.Report.Save(report).ID);

                ReportParameter[] parameters = new ReportParameter[3];

                parameters[0] = new ReportParameter("ReportID", report.ReportID);
                parameters[1] = new ReportParameter("RecommendedAction", txtRecomnededAction.Text);

                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(CurrentUser.User.ID)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);


                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", GeneratedCall.GetReportedCalls(report.ReportID, (mobileOperator.User.GMT - SysParameter.Global_GMT)));
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);


                string profile_name = "FMS_Profile";
                if (ddlSearchClient.SelectedValue.ToInt() == (int)Enums.Clients.ST)//-- Syrian Telecom
                {
                    profile_name = "FMS_Syria_Profile";
                    rvToOperator.LocalReport.ReportPath = "Reports\\rptToSyrianOperator.rdlc";
                }
                else if (ddlSearchClient.SelectedValue.ToInt() == (int)Enums.Clients.Zain)//-- Zain
                {
                    profile_name = "FMS_Profile";
                    rvToOperator.LocalReport.ReportPath = "Reports\\rptToZainOperator.rdlc";
                }
                else if (ddlSearchClient.SelectedValue.ToInt() == (int)Enums.Clients.ITPC)//-- ITPC
                {
                    profile_name = "FMS_Profile";
                    rvToOperator.LocalReport.ReportPath = "Reports\\rptToOperator.rdlc";
                }
                else
                {
                    profile_name = "FMS_Profile";
                    rvToOperator.LocalReport.ReportPath = "Reports\\rptDefaultToOperator.rdlc";
                }



                string CCs = EmailCC.GetEmailCCs(ddlSearchMobileOperator.SelectedValue.ToInt(), ddlSearchClient.SelectedValue.ToInt());



                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenameExcel = ExportReportToExcel(report.ReportID + ".xls");
                




                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenamePDF = ExportReportToPDF(report.ReportID + ".pdf");



                if (ddlReportFormat.SelectedValue == "PDF")
                {
                    if (ddlSearchClient.SelectedValue.ToInt() == 3)
                    {
                        if (mobileOperator.AutoReportSecurity)
                            EmailManager.SendReporttoMobileSyrianOperatorNoBlock(ListIds.Count, filenameExcel + ";" + filenamePDF, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                        else
                            EmailManager.SendReporttoMobileSyrianOperator(ListIds.Count, filenameExcel + ";" + filenamePDF, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                    }
                    else
                    {
                        if (mobileOperator.AutoReportSecurity)
                            EmailManager.SendReporttoMobileOperatorNoBlock(ListIds.Count, filenamePDF, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                        else
                            EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);

                    }

                }
                else if (ddlReportFormat.SelectedValue == "Excel")
                {
                    if (ddlSearchClient.SelectedValue.ToInt() == 3)
                    {
                        if (mobileOperator.AutoReportSecurity)
                            EmailManager.SendReporttoMobileSyrianOperatorNoBlock(ListIds.Count, filenameExcel + ";" + filenamePDF, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                        else 
                            EmailManager.SendReporttoMobileSyrianOperator(ListIds.Count, filenameExcel + ";" + filenamePDF, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                    }
                    else
                    {
                        if (mobileOperator.AutoReportSecurity)
                            EmailManager.SendReporttoMobileOperatorNoBlock(ListIds.Count, filenameExcel, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                        else 
                            EmailManager.SendReporttoMobileOperator(ListIds.Count, filenameExcel, mobileOperator.User.EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                    }
                }



                if (mobileOperator.AutoReportSecurity)
                {
                    reportSecurity.ApplicationUserID = CurrentUser.ApplicationUserID;
                    reportSecurity.SentDateTime = DateTime.Now;
                    reportSecurity.RecommendedAction = txtRecomnededAction.Text.Trim();
                    string ReportSecID;

                    string ReportSecIDBeforeCounter = "FZ" + ddlSearchMobileOperator.SelectedItem.Text.Substring(0, 1) + "S" + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");

                    Vanrise.Fzero.Bypass.Report LastReportSec = Vanrise.Fzero.Bypass.Report.Load(ReportSecIDBeforeCounter);
                    if (LastReportSec == null)
                    {
                        ReportSecID = ReportSecIDBeforeCounter + "0001";
                    }
                    else
                    {
                        ReportSecID = ReportSecIDBeforeCounter + (LastReportSec.ReportID.Replace("- Repeated Numbers", "").Substring(10).ToInt() + 1).ToString("D4");
                    }

                    reportSecurity.ReportID = ReportSecID;
                    if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Suspect)
                    {
                        reportSecurity.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                    }
                    else if (ddlSearchStatus.SelectedValue.ToInt() == (int)Enums.Statuses.Fraud)
                    {
                        reportSecurity.RecommendedActionID = (int)Enums.RecommendedAction.Block;
                    }

                    GeneratedCall.SendReportSecurity(ListIds, Vanrise.Fzero.Bypass.Report.Save(reportSecurity).ID);
                    ReportParameter[] parametersSec = new ReportParameter[3];
                    parametersSec[0] = new ReportParameter("ReportID", reportSecurity.ReportID);
                    parametersSec[1] = new ReportParameter("RecommendedAction", txtRecomnededAction.Text);

                    SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(CurrentUser.User.ID)).User.Signature);
                    rvToSecurity.LocalReport.DataSources.Add(SignatureDataset);

                    rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                    rvToSecurity.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                    rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", GeneratedCall.GetReportedCalls(report.ReportID, (mobileOperator.User.GMT - SysParameter.Global_GMT)));
                    rvToSecurity.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);

                    parametersSec[2] = new ReportParameter("HideSignature", "false");
                    rvToSecurity.LocalReport.SetParameters(parametersSec);
                    rvToSecurity.LocalReport.Refresh();
                    string filenamePDFSec = ExportReportToPDFSec(reportSecurity.ReportID + ".pdf");

                    EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDFSec, mobileOperator.AutoReportSecurityEmail, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + reportSecurity.ReportID,
                        ConfigurationManager.AppSettings["EmailCCNatSec"], reportSecurity.ReportID, "FMS_Profile");

                }


                if (mobileOperator.EnableAutoBlock && !string.IsNullOrEmpty(mobileOperator.AutoBlockEmail))
                {
                    HashSet<string> ListCLIs = new HashSet<string>();
                    foreach (GridDataItem item in gvGeneratedCalls.Items)
                    {
                        if (item.Selected)
                        {
                            ListCLIs.Add(item.GetDataKeyValue("CLI").ToString());
                        }
                    }

                    EmailManager.SendAutoBlockReport(mobileOperator.AutoBlockEmail, ListCLIs, report.ReportID, profile_name);
                }


                gvGeneratedCalls.Rebind();


                LoggedAction.AddLoggedAction((int)Enums.ActionTypes.Reportcasestomobileoperator, CurrentUser.User.ID);

                ShowAlert("Report sent Successfully");
            }
            Response.Redirect("Redirect.aspx");
        }
        else
        {
            ShowAlert("Email not sent, kindly try again later");
        }


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

    private string ExportReportToPDFSec(string reportName)
    {
        Warning[] warnings;
        string[] streamids;
        string mimeType;
        string encoding;
        string filenameExtension;
        byte[] bytes = rvToSecurity.LocalReport.Render(
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
                    item.ForeColor = System.Drawing.Color.Red;
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

    #endregion


}