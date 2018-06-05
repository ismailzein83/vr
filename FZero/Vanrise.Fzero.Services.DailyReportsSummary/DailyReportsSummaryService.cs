using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.Bypass;

namespace Vanrise.Fzero.Services.DailyReportsSummary
{
    partial class DailyReportsSummaryService : ServiceBase
    {
        private static System.Timers.Timer aTimer;
        public DailyReportsSummaryService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(15000); // 10 minutes timeout for startup
            //Debugger.Launch(); // launch and attach debugger
            int timeInterval;
            bool parsed = Int32.TryParse(ConfigurationManager.AppSettings["DailyReportsSummaryTimeInterval"], out timeInterval);

            aTimer = new System.Timers.Timer(timeInterval);// 2 hours
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = timeInterval;// 2 hours
            aTimer.Enabled = true;
            GC.KeepAlive(aTimer);
            OnTimedEvent(null, null);// TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
        public class ReportedCLI
        {
            public string CLI { get; set; }
            public string ReportName { get; set; }
            public DateTime SentDate { get; set; }
            public ReportedCLI()
            {

            }
            public IEnumerable<ReportedCLI> GetReportedCLISchema()
            {
                return null;
            }
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (HttpHelper.CheckInternetConnection("mail.vanrise.com", 26))
                {
                    foreach (MobileOperator i in MobileOperator.GetMobileOperators())
                    {
                        if (i.EnableDailyReportsSummary.HasValue && i.EnableDailyReportsSummary.Value && i.User.ClientID.HasValue)
                        {


                            List<ReportedCLI> listReportedCLI = new List<ReportedCLI>();
                            SqlDatabase db = new SqlDatabase(ConfigurationManager.ConnectionStrings["FMSConnectionString"].ConnectionString);
                            var cmd = db.GetStoredProcCommand("dbo.[sp_Reports_GetReportedCLI]");
                            cmd.CommandTimeout = 600;
                            using (cmd)
                            {
                                var fromDate = DateTime.Today.AddDays(-1).Date;
                                var toDate = DateTime.Today.Date;
                                db.AssignParameters(cmd, new Object[] { i.ID, i.User.ClientID.Value, fromDate,toDate });
                                using (IDataReader reader = db.ExecuteReader(cmd))
                                {
                                    while (reader.Read())
                                    {
                                        listReportedCLI.Add(new ReportedCLI
                                        {
                                            CLI = reader["CLI"] as string,
                                            ReportName = reader["ReportName"] as string,
                                            SentDate = (DateTime)reader["SentDate"],
                                        });

                                    }
                                    reader.Close();
                                }
                            }

                            ErrorLog("Count: " + listReportedCLI.Count);

                            if (listReportedCLI.Count > 0)
                            {
                                SendReport(listReportedCLI, i, (i.User.GMT - SysParameter.Global_GMT));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog("OnTimedEvent: " + ex.Message);
                ErrorLog("OnTimedEvent: " + ex.InnerException);
                ErrorLog("OnTimedEvent: " + ex.ToString());
            }

        }
        private void ErrorLog(string message)
        {
            string cs = "DailyReportsSummaryService";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        private void SendReport(List<ReportedCLI> listReportedCLI, MobileOperator mobileOperator, int DifferenceInGMT)
        {
            try
            {

                ReportViewer rptDailyReport = new ReportViewer();

                string exeFolder = Path.GetDirectoryName(@"C:\FMS\Vanrise.Fzero.Services.DailyReportsSummary\");
                string reportPath = Path.Combine(exeFolder, @"Reports\rptDailyReportSummary.rdlc");
                rptDailyReport.LocalReport.ReportPath = reportPath;



                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("MobileOperator", mobileOperator.User.FullName);
                parameters[1] = new ReportParameter("Count", listReportedCLI.Count().ToString());

                rptDailyReport.LocalReport.SetParameters(parameters);

                ReportDataSource rptDSDailyReport = new ReportDataSource("ReportedCLI", listReportedCLI);
                rptDailyReport.LocalReport.DataSources.Add(rptDSDailyReport);

                rptDailyReport.LocalReport.Refresh();

                string CCs = EmailCC.GetClientEmailCCs(mobileOperator.User.ClientID.Value);
                string profileName = ClientVariables.GetProfileName(mobileOperator.User.ClientID.Value);
                string reportName = "FZ" + mobileOperator.User.FullName.Substring(0, 1) + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + ".pdf";




                if (!string.IsNullOrEmpty(mobileOperator.DailyReportsSummaryEmail))
                {
                    if (listReportedCLI.Count > 0)
                    {
                        string fileName = ClientVariables.ExportReportToPDF(reportName, rptDailyReport);

                        int ID = (int)Enums.EmailTemplates.ReporttoMobileOperator;
                        EmailTemplate template = EmailTemplate.Load(ID);
                        if (template.IsActive)
                        {
                            Email email = new Email() { EmailTemplateID = ID };
                            email.DestinationEmail = mobileOperator.DailyReportsSummaryEmail;
                            email.Subject = string.Format("Daily Fraud Report {0}", DateTime.Today.ToString("dd-MM-yyyy"));
                            email.Body = "Dear Sir/Madame,"
                                       + "<br />Kindly find attached our daily fraud detection reports for " + DateTime.Today.AddDays(-1).ToString("dd-MM-yyyy") + "."
                                       + "<br />Total numbers detected: "
                                       + listReportedCLI.Count()
                                       + " <br /> <br />Best Regards,";
                            email.CC = "";
                            bool sentSuccessfully = Email.SendMailWithAttachement(email, fileName, profileName);
                            ErrorLog("Email Sent Successfully.");
                        }

                        SqlDatabase db = new SqlDatabase(ConfigurationManager.ConnectionStrings["FMSConnectionString"].ConnectionString);
                        var cmd = db.GetStoredProcCommand("dbo.[sp_DailySentReport_Insert]");
                        cmd.CommandTimeout = 600;
                        using (cmd)
                        {
                            db.AssignParameters(cmd, new Object[] { reportName, DateTime.Now });
                            var rowsAffected = db.ExecuteNonQuery(cmd);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog("SendReport: " + e.Message);
            }
        }
    }
}
