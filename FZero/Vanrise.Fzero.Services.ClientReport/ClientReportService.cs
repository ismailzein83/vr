using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(15000); // 10 minutes timeout for startup
            //Debugger.Launch(); // launch and attach debugger
            int timeInterval;

            bool parsed = Int32.TryParse(ConfigurationManager.AppSettings["TimeInterval"], out timeInterval);

            // Create a timer with a ten second interval.
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

                    foreach (Client i in Client.GetAllClients())
                    {
                        if (i.ClientReport.Value)
                        {
                            List<string> DistinctCLIs = new List<string>();
                            List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetClientFraudCases(i.ID);
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

                            ErrorLog("Count: " + listDistinctFraudCases.Count);
                            if (listDistinctFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatus(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);
                                SendReport(listDistinctFraudCases, i.Name, (int)Enums.Statuses.Fraud,  i.ClientEmail, i.ID, (i.GMT.Value - SysParameter.Global_GMT));
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

        private void SendReport(List<int> ListIds, string ClientName, int StatusID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            try
            {
                ReportViewer rvToOperator = new ReportViewer();
                ReportViewer rvToOperator2 = new ReportViewer();
                ReportViewer rvToOperatorNatSec = new ReportViewer();
                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();


                report.SentDateTime = DateTime.Now;

                if (ClientID == 3)
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

                ErrorLog("ClientID: " + ClientID);

                string ReportID;
                    
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

                ErrorLog("ReportID: " + ReportID);

                report.ReportID = ReportID;



                if (StatusID == (int) Enums.Statuses.Suspect)
                {
                    report.RecommendedActionID = (int) Enums.RecommendedAction.Investigate;
                }
                else if (StatusID == (int) Enums.Statuses.Fraud)
                {
                    report.RecommendedActionID = (int) Enums.RecommendedAction.Block;

                }


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
                string reportPath2 = string.Empty;
                bool isNatSecurity = false;
                
                if (ClientName.Substring(0, 1) == "K" || ClientName.Substring(0, 1) == "A")
                    isNatSecurity = true;
                
                string reportPathNatSec = string.Empty;
                if (ClientID == (int) Enums.Clients.ST) //-- Syrian Telecom
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToSyrianOperator.rdlc");
                }
                else if (ClientID == (int) Enums.Clients.Zain) //-- Zain
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToZainOperator.rdlc");
                    reportPath2 = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");

                    ErrorLog("reportPath: " + reportPath);
                    ErrorLog("reportPath2: " + reportPath2);

                }
                else if (ClientID == (int) Enums.Clients.ITPC) //-- ITPC
                {
                    isNatSecurity = true;
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                }
                else
                {
                    isNatSecurity = true;
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToOperator.rdlc");
                }

                reportPathNatSec = Path.Combine(exeFolder, @"Reports\rptToOperatorIraqNationalSec.rdlc");



                rvToOperator.LocalReport.ReportPath = reportPath;
                rvToOperator2.LocalReport.ReportPath = reportPath2;
                rvToOperatorNatSec.LocalReport.ReportPath = reportPathNatSec;



                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset",
                    (ApplicationUser.LoadbyUserId(1)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);
                rvToOperatorNatSec.LocalReport.DataSources.Add(SignatureDataset);
                

                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);
                rvToOperatorNatSec.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls",
                    GeneratedCall.GetReportedCalls(report.ReportID, DifferenceInGMT));
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                rvToOperator2.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                rvToOperatorNatSec.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                string CCs = EmailCC.GetClientEmailCCs(ClientID);



                
                



                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                rvToOperator2.LocalReport.SetParameters(parameters);
                rvToOperator2.LocalReport.Refresh();
                rvToOperatorNatSec.LocalReport.SetParameters(parameters);
                rvToOperatorNatSec.LocalReport.Refresh();
                string emailNatSec = ConfigurationManager.AppSettings["EmailNatSec"];
                string emailCCNatSec = ConfigurationManager.AppSettings["EmailCCNatSec"];

                string filenameExcel = ExportReportToExcel(report.ReportID + ".xls", rvToOperator);
                string filenameExcel2 = ExportReportToExcel(report.ReportID + ".xls", rvToOperator2);

                ErrorLog("filenameExcel: " + filenameExcel);
                ErrorLog("filenameExcel2: " + filenameExcel2);



                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                rvToOperatorNatSec.LocalReport.SetParameters(parameters);
                rvToOperatorNatSec.LocalReport.Refresh();

                string filenamePDF = ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);
                string filenamePDFNatSec = ExportReportToPDF(report.ReportID + ".pdf", rvToOperatorNatSec);

                ErrorLog("filenamePDF: " + filenamePDF);


                if (ClientID == 3)
                {
                    ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);
                    EmailManager.SendReporttoMobileSyrianOperator(ListIds.Count, filenameExcel + ";" + filenamePDF,
                        EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                        CCs, report.ReportID, "FMS_Syria_Profile");

                    if (isNatSecurity)
                    {
                        ExportReportToPDF(report.ReportID + ".pdf", rvToOperatorNatSec);
                        EmailManager.SendReporttoMobileSyrianOperator(ListIds.Count, filenamePDFNatSec,
                            emailNatSec,
                            ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                            emailCCNatSec, report.ReportID, "FMS_Syria_Profile");
                    }
                }
                else
                {
                    string zainExcel = ConfigurationManager.AppSettings["ZainExcel"];

                    ErrorLog("zainExcel: " + zainExcel);

                    if (string.IsNullOrEmpty(zainExcel))
                    {
                        EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress,
                            ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                            report.ReportID, "FMS_Profile");
                        if (isNatSecurity)
                        {
                            EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDFNatSec, emailNatSec,
                                ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, emailCCNatSec,
                                report.ReportID, "FMS_Profile");
                        }
                    }
                    else
                    {
                        if (ClientID == (int) Enums.Clients.Zain)
                        {
                            ErrorLog("Zain Client");

                            if (zainExcel == "true")
                            {
                                ErrorLog("ListIds.Count: " + ListIds.Count + " filenameExcel2: " + filenameExcel2 +
                                         " filenamePDF: " + filenamePDF
                                         + " EmailAddress: " + EmailAddress + " report.ReportID: " + report.ReportID);
                                EmailManager.SendReporttoMobileOperator(ListIds.Count,
                                    filenameExcel2 + ";" + filenamePDF, EmailAddress,
                                    ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                                    CCs, report.ReportID, "FMS_Profile");

                                EmailManager.SendReporttoMobileOperator(ListIds.Count,
                                    filenamePDFNatSec, emailNatSec,
                                    ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" +
                                    report.ReportID,
                                    emailCCNatSec, report.ReportID, "FMS_Profile");
                            }
                            else
                            {
                                EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress,
                                    ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                                    CCs, report.ReportID, "FMS_Profile");

                                if (isNatSecurity)
                                {
                                    EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDFNatSec,
                                        emailNatSec,
                                        ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" +
                                        report.ReportID,
                                        emailCCNatSec, report.ReportID, "FMS_Profile");
                                }
                            }
                        }
                        else
                        {
                            EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress,
                                ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, "FMS_Profile");
                            if (isNatSecurity)
                            {
                                EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDFNatSec, emailNatSec,
                                    ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, emailCCNatSec,
                                    report.ReportID, "FMS_Profile");
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
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

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }

}









