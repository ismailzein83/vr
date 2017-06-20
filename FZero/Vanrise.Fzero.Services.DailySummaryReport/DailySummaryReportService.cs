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



namespace Vanrise.Fzero.Services.DailySummaryReport
{
    public partial class DailySummaryReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public DailySummaryReportService()
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
            aTimer = new System.Timers.Timer(86400000);// 24 hours
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
                                SendReport(listViewSummary, i.User.FullName, i.User.ClientID.Value, i.User.EmailAddress, i.ID, DifferenceInGMT);
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
            ReportViewer rptDailySummaryReport = new ReportViewer();

            string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.DailySummaryReport\");
            string reportPath = Path.Combine(exeFolder, @"Reports\rptDailySummaryReport.rdlc");

            rptDailySummaryReport.LocalReport.ReportPath = reportPath;



            ReportParameter[] parameters = new ReportParameter[1];
            parameters[0] = new ReportParameter("MobileOperator", MobileOperatorName);
            rptDailySummaryReport.LocalReport.SetParameters(parameters);


            string ReportIDBeforeCounter = "FZ" + MobileOperatorName.Substring(0, 1) + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
            List<Vanrise.Fzero.Bypass.Report> LastReport = Vanrise.Fzero.Bypass.Report.LoadDaily(ReportIDBeforeCounter);


            ReportDataSource rptDSDailySummaryReport = new ReportDataSource("DataSet1", LastReport);
            rptDailySummaryReport.LocalReport.DataSources.Add(rptDSDailySummaryReport);


            rptDailySummaryReport.LocalReport.Refresh();

            string CCs = EmailCC.GetClientEmailCCs(ClientID);
            string profileName = ClientVariables.GetProfileName(ClientID);
            string reportName = ClientVariables.GetReportName(ClientID, DifferenceInGMT);

            if (ClientID == 3)
            {
                EmailManager.SendSyrianDailyReport(ClientVariables.ExportReportToExcel(reportName, rptDailySummaryReport), EmailAddress, CCs, profileName);
            }
            else if (ClientID == (int)Enums.Clients.Madar)
            {
                string filenameCSV = ClientVariables.ExportReportToCSV(reportName, rptDailySummaryReport);
                EmailManager.SendWeeklyReport(filenameCSV, EmailAddress, CCs, profileName);
            }
            //else
            //{
            //    EmailManager.SendDailyReport(ExportReportToPDF(System.DateTime.Now.AddHours(DifferenceInGMT).Ticks.ToString() + ".pdf", rptDailySummaryReport), EmailAddress, CCs, "FMS_Profile");
            //}

        }
        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }

}









