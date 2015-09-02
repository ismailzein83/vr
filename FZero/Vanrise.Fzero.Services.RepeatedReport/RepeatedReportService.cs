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



namespace Vanrise.Fzero.Services.RepeatedReport
{
    public partial class RepeatedReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public RepeatedReportService()
	    {
		    InitializeComponent();
	    }

        private void ErrorLog(string message)
        {
            string cs = "Repeated Report Service";
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

            // Create a timer with a ten second interval.
            aTimer = new System.Timers.Timer(604800000);// 1 hours
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

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
                        if (i.AutoReport && i.User.ClientID != null && i.RepeatedCases == true)
                        {
                            List<vwRepeatedCase> listFraudCases = GeneratedCall.GetRepeatedCases(i.User.ClientID, i.ID, (i.User.GMT - SysParameter.Global_GMT));
                            List<int> listDistinctFraudCases = new List<int>();
                            foreach (vwRepeatedCase v in listFraudCases)
                            {
                                listDistinctFraudCases.Add(v.ID);
                            }


                            if (listDistinctFraudCases.Count > 0)
                            {
                                GeneratedCall.UpdateReportStatus(listDistinctFraudCases, (int)Enums.ReportingStatuses.TobeReported, null);
                                SendReport(listFraudCases, listDistinctFraudCases, i.User.FullName, (int)Enums.Statuses.Fraud, i.ID, string.Empty, i.User.ClientID.Value, (i.User.GMT - SysParameter.Global_GMT));
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

        private void SendReport(List<vwRepeatedCase> listFraudCases, List<int> ListIds, string MobileOperatorName, int StatusID, int MobileOperatorID, string EmailAddress, int ClientID, int DifferenceInGMT)
        {
            ReportViewer rvToOperator = new ReportViewer();
            Vanrise.Fzero.Bypass.Report report = new Vanrise.Fzero.Bypass.Report();


            report.SentDateTime = DateTime.Now;

            if (ClientID == 3)
            {
                report.RecommendedAction = "It is highly recommended to immediately block these Repeated fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.";
                report.ApplicationUserID = 8;
            }
            else
            {
                report.RecommendedAction = "It is highly recommended to immediately investigate and trace these Repeated international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.";
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
                ReportID = ReportIDBeforeCounter + (int.Parse(LastReport.ReportID.Substring(9)) + 1).ToString("D4");
            }


            report.ReportID = ReportID + "- Repeated Numbers";



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
                parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately block these Repeated fraudulent MSISDNs as they are terminating international calls without passing legally through ST IGW.");
            }
            else
            {
                parameters[1] = new ReportParameter("RecommendedAction", "It is highly recommended to immediately investigate and trace these Repeated international calls and provide us with the respective CDR's of these Fradulent Calls as they were termnated to your Network and did not pass legally through ITPC's IGW.");
            }

            parameters[2] = new ReportParameter("MobileOperator", MobileOperatorName);

            string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.RepeatedReport\");
            string reportPath = string.Empty;



            if (ClientID == (int)Enums.Clients.ST)//-- Syrian Telecom
            {
                reportPath = Path.Combine(exeFolder, @"Reports\rptRepeatedToSyrianOperator.rdlc");
            }
            else if (ClientID == (int)Enums.Clients.Zain)//-- Zain
            {
                reportPath = Path.Combine(exeFolder, @"Reports\rptRepeatedToZainOperator.rdlc");
            }
            else if (ClientID == (int)Enums.Clients.ITPC)//-- ITPC
            {
                reportPath = Path.Combine(exeFolder, @"Reports\rptRepeatedToOperator.rdlc");
            }
            else
            {
                reportPath = Path.Combine(exeFolder, @"Reports\rptRepeatedDefaultToOperator.rdlc");
            }





            rvToOperator.LocalReport.ReportPath = reportPath;

            rvToOperator.LocalReport.SetParameters(parameters);

            if (DifferenceInGMT != 0)
                foreach (var i in listFraudCases)
                {
                    i.FirstAttemptDateTime = i.FirstAttemptDateTime.AddHours(DifferenceInGMT);
                    i.LastAttemptDateTime = i.LastAttemptDateTime.AddHours(DifferenceInGMT);
                }


            ReportDataSource SignatureDataset = new ReportDataSource("SignatureDataset", (ApplicationUser.LoadbyUserId(1)).User.Signature);
            rvToOperator.LocalReport.DataSources.Add(SignatureDataset);


            ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("dsViewGeneratedCalls", listFraudCases);
            rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);
            rvToOperator.LocalReport.Refresh();

            string CCs = EmailCC.GetClientEmailCCs(ClientID);

            if (ClientID == 3)
            {
                ExportReportToPDF(report.ReportID + ".pdf", rvToOperator);
                EmailManager.SendRepeatedReporttoMobileSyrianOperator(ExportReportToExcel(report.ReportID + ".xls", rvToOperator), EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, "FMS_Syria_Profile");

            }
            else
            {
                EmailManager.SendRepeatedReporttoMobileOperator(ExportReportToPDF(report.ReportID + ".pdf", rvToOperator), EmailAddress, ConfigurationManager.AppSettings["OperatorPath"] + "?ReportID=" + report.ReportID, CCs, report.ReportID, "FMS_Profile");

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









