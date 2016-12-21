using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using Vanrise.Fzero.Bypass;
using Microsoft.Reporting.WebForms;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Services.ClientReport
{
    public partial class ClientReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public ClientReportService()
	    {
		    InitializeComponent();
	    }

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(15000); // timeout in minutes for startup
            //Debugger.Launch(); // launch and attach debugger
            int timeInterval;
            bool parsed = Int32.TryParse(ConfigurationManager.AppSettings["TimeInterval"], out timeInterval);

            // Create a timer.
            aTimer = new System.Timers.Timer(timeInterval);// 2 hours
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            aTimer.Interval = timeInterval;// 2 hours
            aTimer.Enabled = true;

            GC.KeepAlive(aTimer);
            OnTimedEvent(null, null);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (HttpHelper.CheckInternetConnection("mail.vanrise.com", 26))
                {
                    foreach (Client client in Client.GetAllClients())
                    {
                        //Check if the client should send report or not
                        if (client.ClientReport.Value || client.ClientReportSecurity.Value)
                        {
                            List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetClientFraudCases(client.ID);
                            List<ViewGeneratedCall> listFraudCasesSecurity = GeneratedCall.GetClientFraudCasesSecurity(client.ID);
                            List<string> listDistinctCLIs = new List<string>();
                            List<int> listDistinctFraudCases = new List<int>();
                            List<int> listRepeatedFraudCases = new List<int>();

                            List<string> listDistinctCLIsSecurity = new List<string>();
                            List<int> listDistinctFraudCasesSecurity = new List<int>();
                            List<int> listRepeatedFraudCasesSecurity = new List<int>();

                            foreach (ViewGeneratedCall generatedCall in listFraudCases)
                            {
                                if (!listDistinctCLIs.Contains(generatedCall.CLI))
                                {
                                    listDistinctCLIs.Add(generatedCall.CLI);
                                    listDistinctFraudCases.Add(generatedCall.ID);
                                }
                                else
                                {
                                    listRepeatedFraudCases.Add(generatedCall.ID);
                                }
                            }

                            foreach (ViewGeneratedCall generatedCall in listFraudCasesSecurity)
                            {
                                if (!listDistinctCLIsSecurity.Contains(generatedCall.CLI))
                                {
                                    listDistinctCLIsSecurity.Add(generatedCall.CLI);
                                    listDistinctFraudCasesSecurity.Add(generatedCall.ID);
                                }
                                else
                                {
                                    listRepeatedFraudCasesSecurity.Add(generatedCall.ID);
                                }
                            }

                            //If the generated call is repeated update the generated call as IGNORED
                            if (listRepeatedFraudCases.Count > 0)
                            {
                                if (client.ClientReport.Value)
                                    GeneratedCall.UpdateReportStatus(listRepeatedFraudCases, (int)Enums.ReportingStatuses.Ignored, null);
                            }
                            if (listRepeatedFraudCasesSecurity.Count > 0)
                            {
                                if (client.ClientReportSecurity.Value)
                                    GeneratedCall.UpdateReportStatusSecurity(listRepeatedFraudCasesSecurity, (int)Enums.ReportingStatuses.Ignored, null);
                            }

                            //If the generated call is distinct update the generated call as Reported
                            if (listDistinctFraudCases.Count > 0)
                            {
                                if (client.ClientReport.Value)
                                {
                                    GeneratedCall.UpdateReportStatus(listDistinctFraudCases,
                                        (int) Enums.ReportingStatuses.TobeReported, null);
                                    SendReport(listDistinctFraudCases, client.Name, (int) Enums.Statuses.Fraud,
                                        client.ClientEmail, client.ID, (client.GMT.Value - SysParameter.Global_GMT));
                                }
                            }

                            if (listDistinctFraudCasesSecurity.Count > 0)
                            {
                                if (client.ClientReportSecurity.Value)
                                {
                                    GeneratedCall.UpdateReportStatusSecurity(listDistinctFraudCasesSecurity,
                                        (int) Enums.ReportingStatuses.TobeReported, null);
                                    SendReportSecurity(listDistinctFraudCasesSecurity, client.Name,
                                        (int) Enums.Statuses.Fraud, client.ClientEmail, client.ID,
                                        (client.GMT.Value - SysParameter.Global_GMT));
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorLog("OnTimedEvent: " + ex.Message);
                ErrorLog("OnTimedEvent: " + ex.InnerException);
                ErrorLog("OnTimedEvent: " + ex.ToString());
            }
        }

        private void UpdateRecommendedAction(Vanrise.Fzero.Bypass.Report report, int clientId, int statusId)
        {
            if (clientId == (int)Enums.Clients.ST)
            {
                report.RecommendedAction =
                    "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
                report.ApplicationUserID = 8;
            }
            else
            {
                report.RecommendedAction =
                    "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
                report.ApplicationUserID = 3;
            }

            if (statusId == (int)Enums.Statuses.Suspect)
            {
                report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
            }
            else if (statusId == (int)Enums.Statuses.Fraud)
            {
                report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

            }
        }

        private void SendReport(List<int> ListIds, string ClientName, int StatusID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            try
            {
                ReportViewer rvToOperator = new ReportViewer();
                ReportViewer rvToOperatorExcel = new ReportViewer();
                string ReportID;

                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();

                report.SentDateTime = DateTime.Now;

                UpdateRecommendedAction(report, ClientID, StatusID);
  
                string ReportIDBeforeCounter = "FZ" + ClientName.Substring(0, 1) +
                                               DateTime.Now.Year.ToString("D2").Substring(2) +
                                               DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
                Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);

                if (LastReport == null)
                {
                    ReportID = ReportIDBeforeCounter + "0001";
                }
                else
                {
                    ReportID = ReportIDBeforeCounter + (int.Parse(LastReport.ReportID.Substring(9)) + 1).ToString("D4");
                }

                report.ReportID = ReportID;

                GeneratedCall.SendReport(ListIds, Vanrise.Fzero.Bypass.Report.Save(report).ID);
                ReportParameter[] parameters = new ReportParameter[3];
                parameters[0] = new ReportParameter("ReportID", report.ReportID);

                if (ClientID == 3)
                {
                    parameters[1] = new ReportParameter("RecommendedAction",
                        "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.");
                }
                else
                {
                    parameters[1] = new ReportParameter("RecommendedAction",
                        "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.");
                }

                string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.ClientReport\");
                string reportPath = string.Empty;
                string reportPathExcel = string.Empty;

                string reportPathNatSec = string.Empty;
                if (ClientID == (int) Enums.Clients.ST) //-- Syrian Telecom
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToSyrianOperator.rdlc");
                }
                else if (ClientID == (int) Enums.Clients.Zain) //-- Zain
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToZainOperator.rdlc");
                    reportPathExcel = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");
                }
                else if (ClientID == (int) Enums.Clients.ITPC) //-- ITPC
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                }
                else
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                }

                reportPathExcel = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");

                rvToOperator.LocalReport.ReportPath = reportPath;
                rvToOperatorExcel.LocalReport.ReportPath = reportPathExcel;

                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset",
                    (ApplicationUser.LoadbyUserId(1)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);
                

                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls",
                    GeneratedCall.GetReportedCalls(report.ReportID, DifferenceInGMT));
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                rvToOperatorExcel.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                string CCs = EmailCC.GetClientEmailCCs(ClientID);
                
                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                rvToOperatorExcel.LocalReport.SetParameters(parameters);
                rvToOperatorExcel.LocalReport.Refresh();

                string emailNatSec = ConfigurationManager.AppSettings["EmailNatSec"];
                string emailCCNatSec = ConfigurationManager.AppSettings["EmailCCNatSec"];

                string filenameExcel = ExportReportToExcel(report.ReportID + ".xls", rvToOperator);
                string filenameExcelZain = ExportReportToExcel(report.ReportID + ".xls", rvToOperatorExcel);

                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenamePDF = ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                if (ClientID == (int)Enums.Clients.ST) //-- Syrian Telecom
                {
                    ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);
                    EmailManager.SendReporttoMobileSyrianOperator(ListIds.Count, filenameExcel + ";" + filenamePDF,
                        EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                        CCs, report.ReportID, "FMS_Syria_Profile");
                }
                else
                {
                    string zainExcel = ConfigurationManager.AppSettings["ZainExcel"];

                    if (string.IsNullOrEmpty(zainExcel))
                    {
                        EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress,
                            ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                            report.ReportID, "FMS_Profile");
                    }
                    else
                    {
                        if (ClientID == (int)Enums.Clients.Zain)
                        {
                            if (zainExcel == "true")
                            {
                                EmailManager.SendReporttoMobileOperator(ListIds.Count,
                                    filenameExcelZain + ";" + filenamePDF, EmailAddress,
                                    ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                                    CCs, report.ReportID, "FMS_Profile");

                            }
                            else
                            {
                                EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress,
                                    ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                                    CCs, report.ReportID, "FMS_Profile");

                            }
                        }
                        else
                        {
                            EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress,
                                ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, "FMS_Profile");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }
        }


        private void SendReportSecurity(List<int> ListIds, string ClientName, int StatusID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            try
            {
                ReportViewer rvToOperatorExcel = new ReportViewer();
                ReportViewer rvToOperatorNatSec = new ReportViewer();
                string ReportID;

                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();

                report.SentDateTime = DateTime.Now;

                UpdateRecommendedAction(report, ClientID, StatusID);

                string ReportIDBeforeCounter = "FZ" + ClientName.Substring(0, 1) +
                                               DateTime.Now.Year.ToString("D2").Substring(2) +
                                               DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
                Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);

                if (LastReport == null)
                {
                    ReportID = ReportIDBeforeCounter + "0001";
                }
                else
                {
                    ReportID = ReportIDBeforeCounter + (int.Parse(LastReport.ReportID.Substring(9)) + 1).ToString("D4");
                }

                report.ReportID = ReportID;

                GeneratedCall.SendReportSecurity(ListIds, Vanrise.Fzero.Bypass.Report.Save(report).ID);

                ReportParameter[] parameters = new ReportParameter[3];
                parameters[0] = new ReportParameter("ReportID", report.ReportID);

                if (ClientID == 3)
                {
                    parameters[1] = new ReportParameter("RecommendedAction",
                        "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.");
                }
                else
                {
                    parameters[1] = new ReportParameter("RecommendedAction",
                        "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.");
                }

                string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.ClientReport\");
                string reportPath = string.Empty;
                string reportPathExcel = string.Empty;
                bool isNatSecurity = false;

                string reportPathNatSec = string.Empty;
                if (ClientID == (int)Enums.Clients.ST) //-- Syrian Telecom
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToSyrianOperator.rdlc");
                }
                else if (ClientID == (int)Enums.Clients.Zain) //-- Zain
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToZainOperator.rdlc");
                    reportPathExcel = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");
                }
                else if (ClientID == (int)Enums.Clients.ITPC) //-- ITPC
                {
                    isNatSecurity = true;
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                }
                else
                {
                    isNatSecurity = true;
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                }

                reportPathExcel = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");
                reportPathNatSec = Path.Combine(exeFolder, @"Reports\rptToOperatorIraqNationalSec.rdlc");


                rvToOperatorExcel.LocalReport.ReportPath = reportPathExcel;
                rvToOperatorNatSec.LocalReport.ReportPath = reportPathNatSec;

                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset",
                    (ApplicationUser.LoadbyUserId(1)).User.Signature);

                rvToOperatorNatSec.LocalReport.DataSources.Add(SignatureDataset);


                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());

                rvToOperatorNatSec.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls",
                    GeneratedCall.GetReportedCalls(report.ReportID, DifferenceInGMT));

                rvToOperatorExcel.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                rvToOperatorNatSec.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                string CCs = EmailCC.GetClientEmailCCs(ClientID);

                parameters[2] = new ReportParameter("HideSignature", "true");

                rvToOperatorExcel.LocalReport.SetParameters(parameters);
                rvToOperatorExcel.LocalReport.Refresh();
                rvToOperatorNatSec.LocalReport.SetParameters(parameters);
                rvToOperatorNatSec.LocalReport.Refresh();
                string emailNatSec = ConfigurationManager.AppSettings["EmailNatSec"];
                string emailCCNatSec = ConfigurationManager.AppSettings["EmailCCNatSec"];


                string filenameExcelZain = ExportReportToExcel(report.ReportID + ".xls", rvToOperatorExcel);

                parameters[2] = new ReportParameter("HideSignature", "false");

                rvToOperatorNatSec.LocalReport.SetParameters(parameters);
                rvToOperatorNatSec.LocalReport.Refresh();

                string filenamePDFNatSec = ExportReportToPDF(report.ReportID + ".pdf", rvToOperatorNatSec);

                if (ClientID == (int)Enums.Clients.ST) //-- Syrian Telecom
                {
                }
                else
                {
                    string zainExcel = ConfigurationManager.AppSettings["ZainExcel"];

                    if (string.IsNullOrEmpty(zainExcel))
                    {
                        if (ClientID == (int)Enums.Clients.ITPC)
                        {
                            SendEmailNationalSecurity(report.ReportID, ListIds.Count, filenamePDFNatSec);
                        }
                    }
                    else
                    {
                        SendEmailNationalSecurity(report.ReportID, ListIds.Count, filenamePDFNatSec);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }
        }

        #region SendEmailNatSec
        private void SendEmailNationalSecurity(string reportId, int countListID, string filenamePDFNatSec)
        {
            string emailNatSec = ConfigurationManager.AppSettings["EmailNatSec"];
            string emailCCNatSec = ConfigurationManager.AppSettings["EmailCCNatSec"];
            string operatorPath = ConfigurationManager.AppSettings["OperatorPath"];

            EmailManager.SendReporttoMobileOperator(countListID, filenamePDFNatSec, emailNatSec,
                    operatorPath + "?ReportID=" + reportId, emailCCNatSec, reportId, "FMS_Profile");
        }
        #endregion

        #region ExportReportsPDFExcel
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

        #endregion

        #region Logging
        private void ErrorLog(string message)
        {
            string cs = "Client Report Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
        #endregion
    }
}