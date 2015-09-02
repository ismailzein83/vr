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



namespace Vanrise.Fzero.Services.DailyReport
{
    public partial class DailyReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public DailyReportService()
	    {
		    InitializeComponent();
	    }

        private void ErrorLog(string message)
        {
            string cs = "Daily Report Service";
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
            aTimer = new System.Timers.Timer(86400000);// 1 hours
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
                        if ( i.User.Client.SendDailyReport==true)
                        {
                            int DifferenceInGMT = (i.User.GMT - SysParameter.Global_GMT);
                            List<ViewSummary> listViewSummary = GeneratedCall.GetViewSummary(i.User.ClientID.Value, i.ID, DateTime.Now.AddHours(DifferenceInGMT).AddDays(-1), DateTime.Now.AddHours(DifferenceInGMT));

                            if (listViewSummary.Count > 0)
                            {
                                SendReport(listViewSummary, i.User.FullName, i.User.ClientID.Value, string.Empty, i.ID, DifferenceInGMT);
                            }
                        }
                    }
                }


              



            }
            catch(Exception ex)
            {
                ErrorLog("OnTimedEvent() " + ex.Message);
                ErrorLog("OnTimedEvent() " + ex.InnerException);
                ErrorLog("OnTimedEvent() " + ex.ToString());
            }


          
        }

        private void SendReport(List<ViewSummary> listViewSummary, string MobileOperatorName, int ClientID, string EmailAddress, int MobileOperatorId, int DifferenceInGMT)
        {
            ReportViewer rptDailyReport = new ReportViewer();

            string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.DailyReport\");
            string reportPath = Path.Combine(exeFolder, @"Reports\rptDailyReport.rdlc");

            rptDailyReport.LocalReport.ReportPath = reportPath;



            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("MobileOperator", MobileOperatorName);
            rptDailyReport.LocalReport.SetParameters(parameters);



            ReportDataSource rptDSDailyReport = new ReportDataSource("DSDailyReport", GeneratedCall.GetViewSummary(ClientID, MobileOperatorId, DateTime.Now.AddHours(DifferenceInGMT).AddDays(-1), DateTime.Now.AddHours(DifferenceInGMT)));
            rptDailyReport.LocalReport.DataSources.Add(rptDSDailyReport);


            ReportDataSource DSFraudCasesofMobileOperator = new ReportDataSource("DSFraudCasesofMobileOperator", Vanrise.Fzero.Bypass.GeneratedCall.GetFraudCases(ClientID, MobileOperatorId, DateTime.Now.AddHours(DifferenceInGMT).Date, DateTime.Now.AddHours(DifferenceInGMT).Date.AddDays(1).AddSeconds(-1), 0, false, DifferenceInGMT));
            rptDailyReport.LocalReport.DataSources.Add(DSFraudCasesofMobileOperator);


            ReportDataSource rptDSOrigination = new ReportDataSource("DSOrigination", Vanrise.Fzero.Bypass.View_Origination.GetView_Origination(ClientID, MobileOperatorId, DateTime.Now.AddHours(DifferenceInGMT).Date, DateTime.Now.AddHours(DifferenceInGMT).Date.AddDays(1).AddSeconds(-1)));
            rptDailyReport.LocalReport.DataSources.Add(rptDSOrigination);




            rptDailyReport.LocalReport.Refresh();

            string CCs = EmailCC.GetClientEmailCCs(ClientID);


            if (ClientID == 3)
            {
                EmailManager.SendSyrianDailyReport(ExportReportToExcel(System.DateTime.Now.AddHours(DifferenceInGMT).Ticks.ToString() + ".xls", rptDailyReport), EmailAddress, CCs, "FMS_Syria_Profile");
            }
            else
            {
                EmailManager.SendDailyReport(ExportReportToPDF(System.DateTime.Now.AddHours(DifferenceInGMT).Ticks.ToString() + ".pdf", rptDailyReport), EmailAddress, CCs, "FMS_Profile");
            }







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

        private string ExportReportToPDF(string reportName, ReportViewer rptDSDailyReport)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            byte[] bytes = rptDSDailyReport.LocalReport.Render(
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

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }

}









