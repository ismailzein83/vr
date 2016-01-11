﻿using System;
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




public partial class testingpost : BasePage
{


    private void SendReport(List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT)
    {
        ReportViewer rvToOperator = new ReportViewer();
        Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();


        report.SentDateTime = DateTime.Now;

        if (ClientID == 3)
        {
            report.RecommendedAction = "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
            report.ApplicationUserID = 8;
        }
        else
        {
            report.RecommendedAction = "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
            report.ApplicationUserID = 3;
        }



        string ReportID;

        string ReportIDBeforeCounter = "FZ" + MobileOperatorName.Substring(0, 1) + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");


        Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);
        if (LastReport == null)
        {
            ReportID = ReportIDBeforeCounter + "0001";
        }
        else
        {
            ReportID = ReportIDBeforeCounter + (int.Parse(LastReport.ReportID.Replace("- Repeated Numbers", "").Substring(9)) + 1).ToString("D4");
        }

        report.ReportID = ReportID;


        if (StatusID == (int)Enums.Statuses.Suspect)
        {
            report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
        }
        else if (StatusID == (int)Enums.Statuses.Fraud)
        {
            report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

        }


        GeneratedCall.SendReport(ListIds, Vanrise.Fzero.Bypass.Report.Save(report).ID);
        ReportParameter[] parameters = new ReportParameter[3];
        parameters[0] = new ReportParameter("ReportID", report.ReportID);



        if (ClientID == 3)
        {
            parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.");
        }
        else
        {
            parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.");
        }



        string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.Report\");
        string reportPath = string.Empty;



        if (ClientID == (int)Enums.Clients.ST)//-- Syrian Telecom
        {
            reportPath = Path.Combine(exeFolder, @"Reports\rptToSyrianOperator.rdlc");
        }
        else if (ClientID == (int)Enums.Clients.Zain)//-- Zain
        {
            reportPath = Path.Combine(exeFolder, @"Reports\rptToZainOperator.rdlc");
        }
        else if (ClientID == (int)Enums.Clients.ITPC)//-- ITPC
        {
            reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
        }





        rvToOperator.LocalReport.ReportPath = reportPath;



        ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(1)).User.Signature);
        rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


        ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
        rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

        ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", GeneratedCall.GetReportedCalls(report.ReportID, DifferenceInGMT));
        rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);





        parameters[2] = new ReportParameter("HideSignature", "true");
        rvToOperator.LocalReport.SetParameters(parameters);
        rvToOperator.LocalReport.Refresh();
        string filenameExcel = ExportReportToExcel(report.ReportID + ".xls", rvToOperator);





        parameters[2] = new ReportParameter("HideSignature", "false");
        rvToOperator.LocalReport.SetParameters(parameters);
        rvToOperator.LocalReport.Refresh();
        string filenamePDF = ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);









        string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);


        if (ClientID == 3)
        {
            EmailManager.SendReporttoMobileSyrianOperator(ListIds.Count, filenameExcel + ";" + filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, "FMS_Syria_Profile");

        }
        else
        {
            EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, "FMS_Profile");

        }

    }

    private string ExportReportToPDF(string reportName, ReportViewer rvToOperator)
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

    private string ExportReportToExcel(string reportName, ReportViewer rvToOperator)
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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.HasPermission(Enums.SystemPermissions.ReporttoOperator))
            PreviousPageRedirect();
    }
    protected void btnTest_Click(object sender, EventArgs e)
    {
        try
        {
            if (HttpHelper.CheckInternetConnection("mail.vanrise.com", 26))
            {

                foreach (MobileOperator i in MobileOperator.GetMobileOperators())
                {
                    Response.Write(i.User.FullName+"\n");
                    if (i.AutoReport && i.User.ClientID != null)
                    {
                        List<string> DistinctCLIs = new List<string>();
                        List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetFraudCases(i.User.ClientID, i.ID);
                        List<int> listDistinctFraudCases = new List<int>();
                        List<int> listRepeatedFraudCases = new List<int>();
                        foreach (ViewGeneratedCall v in listFraudCases)
                        {
                            if (!DistinctCLIs.Contains(v.CLI))
                            {
                                DistinctCLIs.Add(v.CLI);
                                listDistinctFraudCases.Add(v.ID);
                            }
                            else
                            {
                                listRepeatedFraudCases.Add(v.ID);
                            }
                        }


                        if (listRepeatedFraudCases.Count > 0)
                        {
                            GeneratedCall.UpdateReportStatus(listRepeatedFraudCases, (int)Enums.ReportingStatuses.Ignored, null);
                        }


                        if (listDistinctFraudCases.Count > 0)
                        {
                            GeneratedCall.UpdateReportStatus(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);

                            SendReport(listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, i.User.EmailAddress, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT));
                        }


                    }
                }


            }
        }
        catch
        {
           
        }

    }
}