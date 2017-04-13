﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using Vanrise.Fzero.Bypass;
using Microsoft.Reporting.WebForms;
using Vanrise.CommonLibrary;



namespace Vanrise.Fzero.Services.Report
{
    public partial class ReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public ReportService()
	    {
		    InitializeComponent();
	    }

        private void ErrorLog(string message)
        {
            string cs = "Report Service";
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

            aTimer = new System.Timers.Timer(timeInterval);// 2 hours
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
                    foreach (MobileOperator i in MobileOperator.GetMobileOperators())
                    {
                        if (i.AutoReport && i.User.ClientID != null)
                        {
                            HashSet<string> DistinctCLIs = new HashSet<string>();
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

                            ErrorLog("Count: " + listDistinctFraudCases.Count);
                            if (listDistinctFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatus(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);

                                SendReport(DistinctCLIs, listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, i.User.EmailAddress, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT));
                            }


                        }

                        if (i.AutoReportSecurity && i.User.ClientID != null)
                        {
                            HashSet<string> DistinctCLIs = new HashSet<string>();
                            List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetFraudCasesSecurity(i.User.ClientID, i.ID);
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
                                GeneratedCall.UpdateReportStatusSecurity(listRepeatedFraudCases, (int)Enums.ReportingStatuses.Ignored, null);
                            }

                            ErrorLog("Count: " + listDistinctFraudCases.Count);
                            if (listDistinctFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatusSecurity(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);

                                SendReportSecurity(DistinctCLIs, listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, i.AutoReportSecurityEmail, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT));
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

        private void SendReport(HashSet<string> CLIs, List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            try
            {
                ReportViewer rvToOperator = new ReportViewer();
                ReportViewer rvToOperator2 = new ReportViewer();
                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();


                report.SentDateTime = DateTime.Now;

                if (ClientID == 3) //ST
                {
                    report.RecommendedAction = "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
                    report.ApplicationUserID = 8;
                }
                else
                {
                    report.RecommendedAction = "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
                    report.ApplicationUserID = 3;
                }

                ErrorLog("ClientID: " + ClientID);

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
                ErrorLog("ReportID: " + ReportID);
                report.ReportID = ReportID;


                if (StatusID == (int)Enums.Statuses.Suspect)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                }
                else if (StatusID == (int)Enums.Statuses.Fraud)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

                }

                Vanrise.Fzero.Bypass.Report reportSaved = Vanrise.Fzero.Bypass.Report.Save(report);
                GeneratedCall.SendReport(ListIds, reportSaved.ID);
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
                string reportPath2 = string.Empty;


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
                    reportPath2 = Path.Combine(exeFolder, @"Reports\rptToZainOperatorExcel.rdlc");
                    ErrorLog("reportPath: " + reportPath);
                    ErrorLog("reportPath2: " + reportPath2);
                }
                //else if (ClientID == (int)Enums.Clients.Madar)//-- Madar
                //{
                //    reportPath = Path.Combine(exeFolder, @"Reports\rptToMadarOperator.rdlc");
                //    ErrorLog("reportPath: " + reportPath);
                //}
                else
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptDefaultToOperator.rdlc");
                }
                




                rvToOperator.LocalReport.ReportPath = reportPath;
                if (ClientID == (int)Enums.Clients.ITPC)
                    rvToOperator2.LocalReport.ReportPath = reportPath2;


                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(1)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                //
                //Get reported calls compared from generated calls
                //
                List<ViewGeneratedCall> reportedCalls = GeneratedCall.GetReportedCalls(report.ReportID, DifferenceInGMT);
                int reportedCallsCount = reportedCalls.Count;
                if (reportedCallsCount == 0)
                {
                    Vanrise.Fzero.Bypass.Report.Delete(reportSaved);
                }

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", reportedCalls);
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
                if (ClientID == (int)Enums.Clients.ITPC)
                    rvToOperator2.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);




                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenameExcel = ExportReportToExcel(report.ReportID + ".xls", rvToOperator);
                string filenameExcel2 = "";
                if (ClientID == (int)Enums.Clients.ITPC)
                {
                    rvToOperator2.LocalReport.SetParameters(parameters);
                    rvToOperator2.LocalReport.Refresh();
                    filenameExcel2 = ExportReportToExcel(report.ReportID + ".xls", rvToOperator2);
                    ErrorLog("filenameExcel: " + filenameExcel);
                    ErrorLog("filenameExcel2: " + filenameExcel2);
                }
                
                




                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenamePDF = ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                ErrorLog("filenamePDF: " + filenamePDF);







                string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);
                string profile_name = "FMS_Profile";

                if (ClientID == 3)
                    profile_name = "FMS_Syria_Profile";



                if (ClientID == 3)
                {
                    EmailManager.SendReporttoMobileSyrianOperator(reportedCallsCount, filenameExcel + ";" + filenamePDF,
                                EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, profile_name);
                }
                //else if (ClientID == (int)Enums.Clients.Madar)
                //{
                //    EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenamePDF,
                //                EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                //                report.ReportID, profile_name);
                //}
                else
                {
                    string zainExcel = ConfigurationManager.AppSettings["ZainExcel"];

                    ErrorLog("zainExcel: " + zainExcel);

                    if (string.IsNullOrEmpty(zainExcel))
                        EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, "FMS_Profile");
                    else
                    {
                        string reportCode = ReportID.Substring(0, 3);

                        if (reportCode == "FZZ")
                        {
                            ErrorLog("Zain Client");

                            if (zainExcel == "true")
                            {
                                ErrorLog("reportedCallsCount: " + reportedCallsCount + " filenameExcel2: " + filenameExcel2 + " filenamePDF: " + filenamePDF
                                        + " EmailAddress: " + EmailAddress + " report.ReportID: " + report.ReportID);

                                EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenameExcel2 + ";" + filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                            }
                            else
                                EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                        }
                        else
                            EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profile_name);
                    }

                }


                MobileOperator mobileOperator = Vanrise.Fzero.Bypass.MobileOperator.Load(MobileOperatorID);

                if (mobileOperator.EnableAutoBlock && !string.IsNullOrEmpty(mobileOperator.AutoBlockEmail))
                {
                    EmailManager.SendAutoBlockReport(mobileOperator.AutoBlockEmail, CLIs, report.ReportID, profile_name);
                }
            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }


        }

        private void SendReportSecurity(HashSet<string> CLIs, List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            try
            {

                ReportViewer rvToOperator = new ReportViewer();
                Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();


                report.SentDateTime = DateTime.Now;

                if (ClientID == 3) //ST
                {
                    report.RecommendedAction = "It is highly recommended to immediately block these fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
                    report.ApplicationUserID = 8;
                }
                else
                {
                    report.RecommendedAction = "It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
                    report.ApplicationUserID = 3;
                }

                ErrorLog("ClientID: " + ClientID);

                string ReportID;

                string ReportIDBeforeCounter = "FZ" + MobileOperatorName.Substring(0, 1) + "S" + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");


                Vanrise.Fzero.Bypass.Report LastReport = Vanrise.Fzero.Bypass.Report.Load(ReportIDBeforeCounter);
                if (LastReport == null)
                {
                    ReportID = ReportIDBeforeCounter + "0001";
                }
                else
                {
                    ReportID = ReportIDBeforeCounter + (int.Parse(LastReport.ReportID.Replace("- Repeated Numbers", "").Substring(10)) + 1).ToString("D4");
                }
                ErrorLog("ReportID: " + ReportID);
                report.ReportID = ReportID;


                if (StatusID == (int)Enums.Statuses.Suspect)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Investigate;
                }
                else if (StatusID == (int)Enums.Statuses.Fraud)
                {
                    report.RecommendedActionID = (int)Enums.RecommendedAction.Block;

                }

                Vanrise.Fzero.Bypass.Report ReportSaved = Vanrise.Fzero.Bypass.Report.Save(report);
                GeneratedCall.SendReportSecurity(ListIds, ReportSaved.ID);
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
                reportPath = Path.Combine(exeFolder, @"Reports\rptToOperatorIraqNationalSec.rdlc");
                rvToOperator.LocalReport.ReportPath = reportPath;

                ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(1)).User.Signature);
                rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


                ReportDataSource rptDataSourceDataSet1 = new ReportDataSource("DataSet1", AppType.GetAppTypes());
                rvToOperator.LocalReport.DataSources.Add(rptDataSourceDataSet1);

                ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", GeneratedCall.GetReportedSecCalls(ReportSaved.ID, DifferenceInGMT));
                rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);

                parameters[2] = new ReportParameter("HideSignature", "true");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenameExcel = ExportReportToExcel(report.ReportID + ".xls", rvToOperator);

                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenamePDF = ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                ErrorLog("filenamePDF: " + filenamePDF);
                    
                string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);
                string profile_name = "FMS_Profile";

                if (ClientID == 3)
                    profile_name = "FMS_Syria_Profile";

                EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                    ConfigurationManager.AppSettings["EmailCCNatSec"], report.ReportID, "FMS_Profile");


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









