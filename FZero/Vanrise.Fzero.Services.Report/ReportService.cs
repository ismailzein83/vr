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
using System.Net;
using System.Text;
using System.Linq;
using Rebex.Net;


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
                            HashSet<string> DistinctCLIsWitoutCountryCode = new HashSet<string>();

                            List<ViewGeneratedCall> listFraudCases = GeneratedCall.GetFraudCases(i.User.ClientID, i.ID);
                            List<int> listDistinctFraudCases = new List<int>();
                            List<int> listRepeatedFraudCases = new List<int>();
                            foreach (ViewGeneratedCall v in listFraudCases)
                            {
                                string cli = v.CLI;
                                string cliWithoutCountryCode = v.CLI;

                                if (i.User.ClientID == (int)Enums.Clients.ITPC)
                                {
                                    if (cli.StartsWith("0"))
                                    {
                                        cli = string.Format("964{0}", v.CLI.Substring(1));
                                        cliWithoutCountryCode = string.Format("{0}", v.CLI.Substring(1));
                                    }
                                }
                            
                                if (!DistinctCLIsWitoutCountryCode.Contains(cli))
                                {
                                    DistinctCLIsWitoutCountryCode.Add(cliWithoutCountryCode);
                                }

                                if (!DistinctCLIs.Contains(cli))
                                {
                                    DistinctCLIs.Add(cli);

                                    DistinctCLIsWitoutCountryCode.Add(cliWithoutCountryCode);

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
                                string reportCounter;
                                SendReport(DistinctCLIs, listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, i.User.EmailAddress, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT), out  reportCounter);
                                if (i.EnableFTP.HasValue && i.EnableFTP.Value)
                                {
                                    SaveToFTPFile(i.FTPAddress, i.FTPUserName, i.FTPPassword, i.FTPPort, i.FTPType, i.User.FullName, DistinctCLIsWitoutCountryCode, reportCounter);
                                }
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
                                string cli = v.CLI;
                                if (i.User.ClientID == (int)Enums.Clients.ITPC)
                                {
                                    if (cli.StartsWith("0"))
                                    {
                                        cli = string.Format("964{0}", v.CLI.Substring(1));
                                    }
                                }

                                if (!DistinctCLIs.Contains(cli))
                                {
                                    DistinctCLIs.Add(cli);
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


        private void SaveToFTPFile(string ftpAddress, string ftpUserName, string ftpPassword, string ftpPort, int? ftpType, string mobileOperatorName, HashSet<string> distinctCLIs, string reportCounter)
        {

            try
            {
                if (reportCounter == null)
                    return;

                if (ftpAddress != null && ftpUserName != null & ftpPassword != null)
                {
                    string fileName = string.Format("AutoSuspend_{0:yyyyMMdd}_{1}_Vanrise.txt", DateTime.Now, reportCounter);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in distinctCLIs)
                    {
                        stringBuilder.AppendLine(item.ToString());
                    }

                    byte[] buffer = new ASCIIEncoding().GetBytes(stringBuilder.ToString());

                    if (!ftpType.HasValue || ftpType.Value == (int)FTPTypeEnum.FTP)
                    {



                        FtpWebRequest ftpWebRequest = (System.Net.FtpWebRequest)FtpWebRequest.Create(String.Format("ftp://{0}/{1}", ftpAddress, fileName));
                        ftpWebRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                        ftpWebRequest.KeepAlive = false;
                        ftpWebRequest.Timeout = 20000;
                        ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                        ftpWebRequest.UseBinary = true;
                        Stream stream = ftpWebRequest.GetRequestStream();
                        ftpWebRequest.ContentLength = buffer.Length;
                        stream.Write(buffer, 0, buffer.Length);
                        int contentLen = buffer.Length;
                        stream.Close();
                        stream.Dispose();

                    }
                    else if (ftpType.Value == (int)FTPTypeEnum.SFTP)
                    {
                        Sftp client = new Sftp();
                        int serverPort = 22;
                        if (ftpPort != null)
                        {
                            serverPort = Convert.ToInt32(ftpPort);
                        }
                        var ftpAddressArray = ftpAddress.Split('/');
                        client.Connect(ftpAddressArray[0], serverPort);
                        client.Login(ftpUserName, ftpPassword);
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                        if (ftpAddressArray.Length > 1)
                        {
                            string filePath = "";
                            for (var i = 1; i < ftpAddressArray.Length; i++)
                            {
                                filePath += string.Format("/{0}", ftpAddressArray[i]);
                            }
                            fileName = string.Format("{0}/{1}", filePath, fileName);
                        }
                        client.PutFile(ms, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog("SaveToFTPFile: " + ex.Message);
                ErrorLog("SaveToFTPFile: " + ex.InnerException);
                ErrorLog("SaveToFTPFile: " + ex.ToString());
            }

        }

        private void SendReport(HashSet<string> CLIs, List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT, out string reportCounter)
        {
            reportCounter = null;
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

                string counter;
                if (LastReport == null)
                {
                    counter = "0001";
                   
                }
                else
                {
                    counter = (int.Parse(LastReport.ReportID.Replace("- Repeated Numbers", "").Substring(10)) + 1).ToString("D4");
                }
                reportCounter = counter;
                ReportID = ReportIDBeforeCounter + counter;
              

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
                else if (ClientID == (int)Enums.Clients.Madar)//-- Madar
                {
                    reportPath = Path.Combine(exeFolder, @"Reports\rptToMadarOperator.rdlc");
                    ErrorLog("reportPath: " + reportPath);
                }
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
              
                
                
                string filenameExcel = ClientVariables.ExportReportToExcel(report.ReportID + ".xls", rvToOperator);
                string filenameExcel2 = "";
                if (ClientID == (int)Enums.Clients.ITPC)
                {
                    rvToOperator2.LocalReport.SetParameters(parameters);
                    rvToOperator2.LocalReport.Refresh();
                    filenameExcel2 = ClientVariables.ExportReportToExcel(report.ReportID + ".xls", rvToOperator2);
                    ErrorLog("filenameExcel: " + filenameExcel);
                    ErrorLog("filenameExcel2: " + filenameExcel2);
                }
                
                




                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();

                string attachedPath = null;
                MobileOperator mobileOperator = Vanrise.Fzero.Bypass.MobileOperator.Load(MobileOperatorID);
                if (mobileOperator.IncludeCSVFile.HasValue && mobileOperator.IncludeCSVFile.Value)
                {
                    attachedPath = ClientVariables.ExportReportToCSV(report.ReportID + ".csv", rvToOperator);
                }


                string filenamePDF = ClientVariables.ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                ErrorLog("filenamePDF: " + filenamePDF);



              

                string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);

                string profileName = ClientVariables.GetProfileName(ClientID);


             
                if (ClientID == 3)
                {
                    if (attachedPath != null)
                    {
                        attachedPath += ";" + filenameExcel + ";" + filenamePDF;
                    }else
                    {
                        attachedPath = filenameExcel + ";" + filenamePDF;
                    }
                    EmailManager.SendReporttoMobileSyrianOperator(reportedCallsCount, attachedPath,
                                EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, profileName);
                }
                else if (ClientID == (int)Enums.Clients.Madar)
                {

                    string filenameCSV = ClientVariables.ExportReportToCSV(report.ReportID + ".csv", rvToOperator);
                    ErrorLog("Madar: filenameCSV " + filenameCSV + " report.ReportID " + report.ReportID);

                    EmailManager.SendReporttoMobileOperator(reportedCallsCount, filenameCSV,
                                EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs,
                                report.ReportID, profileName);
                }
                else
                {
                    string zainExcel = ConfigurationManager.AppSettings["ZainExcel"];

                    ErrorLog("zainExcel: " + zainExcel);

                    if (string.IsNullOrEmpty(zainExcel))
                    {
                        if (attachedPath != null)
                        {
                            attachedPath += ";" +  filenamePDF;
                        }
                        else
                        {
                            attachedPath = filenamePDF;
                        }

                        EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                    }
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

                                if (attachedPath != null)
                                {
                                    attachedPath += ";" + filenameExcel2 + ";" + filenamePDF;
                                }
                                else
                                {
                                    attachedPath = filenameExcel2 + ";" + filenamePDF;
                                }

                                EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                            }
                            else
                            {
                                if (attachedPath != null)
                                {
                                    attachedPath += ";" + filenamePDF;
                                }
                                else
                                {
                                    attachedPath = filenamePDF;
                                }

                                EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                            }
                               
                        }
                        else
                        {
                            if (attachedPath != null)
                            {
                                attachedPath += ";"+ filenamePDF;
                            }
                            else
                            {
                                attachedPath = filenamePDF;
                            }


                            EmailManager.SendReporttoMobileOperator(reportedCallsCount, attachedPath, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, profileName);
                        }
                    }

                }



                if (mobileOperator.EnableAutoBlock && !string.IsNullOrEmpty(mobileOperator.AutoBlockEmail))
                {
                    EmailManager.SendAutoBlockReport(mobileOperator.AutoBlockEmail, CLIs, report.ReportID, profileName);
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
                string filenameExcel = ClientVariables.ExportReportToExcel(report.ReportID + ".xls", rvToOperator);

                parameters[2] = new ReportParameter("HideSignature", "false");
                rvToOperator.LocalReport.SetParameters(parameters);
                rvToOperator.LocalReport.Refresh();
                string filenamePDF = ClientVariables.ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);

                ErrorLog("filenamePDF: " + filenamePDF);
                    
                string CCs = EmailCC.GetEmailCCs(MobileOperatorID, ClientID);
                string profileName = ClientVariables.GetProfileName(ClientID);


                EmailManager.SendReporttoMobileOperator(ListIds.Count, filenamePDF, EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID,
                    ConfigurationManager.AppSettings["EmailCCNatSec"], report.ReportID, profileName);


            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }


        }


        //private string ExportReportToPDF(string reportName, ReportViewer rvToOperator)
        //{
        //    Warning[] warnings;
        //    string[] streamids;
        //    string mimeType;
        //    string encoding;
        //    string filenameExtension;
        //    byte[] bytes = rvToOperator.LocalReport.Render(
        //       "PDF", null, out mimeType, out encoding, out filenameExtension,
        //        out streamids, out warnings);

        //    string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        //    ClientVariables.SaveClientReport(filename, bytes);


        //    return filename;
        //}

        //private string ExportReportToExcel(string reportName, ReportViewer rvToOperator)
        //{
        //    Warning[] warnings;
        //    string[] streamids;
        //    string mimeType;
        //    string encoding;
        //    string filenameExtension;
        //    byte[] bytes = rvToOperator.LocalReport.Render(
        //       "Excel", null, out mimeType, out encoding, out filenameExtension,
        //        out streamids, out warnings);

        //    string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        //    ClientVariables.SaveClientReport(filename, bytes);


        //    return filename;
        //}

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }

}









