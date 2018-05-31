﻿using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
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

namespace Vanrise.Fzero.Services.NonFruadReport
{
    partial class NonFruadReportService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public NonFruadReportService()
        {
            InitializeComponent();
        }
        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

        protected override void OnStart(string[] args)
        {
            base.RequestAdditionalTime(15000); // 10 minutes timeout for startup
            //Debugger.Launch(); // launch and attach debugger
            int timeInterval;
            bool parsed = Int32.TryParse(ConfigurationManager.AppSettings["NonFruadReportTimeInterval"], out timeInterval);

            aTimer = new System.Timers.Timer(timeInterval);// 2 hours
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = timeInterval;// 2 hours
            aTimer.Enabled = true;
            GC.KeepAlive(aTimer);
            OnTimedEvent(null, null);// TODO: Add code here to start your service.
        }
       public class CleanCases
       {
           public int GeneratedCallId { get; set; }
           public string ANumber { get; set; }
           public string BNumber { get; set; }
           public string RecievedCLI { get; set; }
           public DateTime AttamptDateTime { get; set; }
           public int? Duration { get; set; }
           public bool DifferentCLI { get; set; }
       }
       private void OnTimedEvent(object source, ElapsedEventArgs e)
       {
           try
           {
               if (HttpHelper.CheckInternetConnection("mail.vanrise.com", 26))
               {
                   foreach (MobileOperator i in MobileOperator.GetMobileOperators())
                   {
                       if (i.EnableNonFruadReport.HasValue && i.EnableNonFruadReport.Value && i.User.ClientID.HasValue)
                       {
                           List<CleanCases> listDistinctCleanCases = new List<CleanCases>();
                           SqlDatabase db = new SqlDatabase(ConfigurationManager.ConnectionStrings["FMSConnectionString"].ConnectionString);
                           var cmd = db.GetStoredProcCommand("dbo.[sp_GeneratedCalls_GetNonReportedClean]");
                           cmd.CommandTimeout = 600;
                           using (cmd)
                           {
                               db.AssignParameters(cmd, new Object[] { i.ID, i.User.ClientID.Value});
                               using (IDataReader reader = db.ExecuteReader(cmd))
                               {
                                   while (reader.Read())
                                   {
                                        listDistinctCleanCases.Add(new CleanCases
                                        {
                                            GeneratedCallId = (int)reader["ID"],
                                            ANumber = reader["a_number"] as string,
                                            AttamptDateTime =  (DateTime)reader["AttemptDateTime"],
                                            BNumber =reader["b_number"]as string ,
                                            DifferentCLI =  (int)reader["DifferentCLI"] == 0?false:true,
                                            Duration = reader["DurationInSeconds"] != DBNull.Value ? (int?)reader["DurationInSeconds"] : default(int?),
                                            RecievedCLI =  reader["RecievedCLI"] as string
                                        });
                                       
                                   }
                                   reader.Close();
                               }
                           }

                           ErrorLog("Count: " + listDistinctCleanCases.Count);
                         
                           if (listDistinctCleanCases.Count > 0)
                           {
                               SendReport(listDistinctCleanCases, i, (i.User.GMT - SysParameter.Global_GMT));
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
            string cs = "Non Fruad Report Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        private void SendReport(List<CleanCases> listDistinctCleanCases,MobileOperator mobileOperator,  int DifferenceInGMT)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                string reportName= "FZ" + mobileOperator.User.FullName.Substring(0, 1) + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + ".csv";
                
                stringBuilder.Append("Calling Number, Called Number, Recieved CLI, Attampt Date Time, Duration, Different CLI");

                foreach (var listDistinctCleanCase in listDistinctCleanCases)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("{0},{1},{2},{3},{4}", listDistinctCleanCase.ANumber, listDistinctCleanCase.BNumber, listDistinctCleanCase.RecievedCLI, listDistinctCleanCase.AttamptDateTime, listDistinctCleanCase.Duration);
                }


                string profileName = ClientVariables.GetProfileName(mobileOperator.User.ClientID.Value);

                if (!string.IsNullOrEmpty(mobileOperator.NonFruadReportEmail))
                {
                    if (listDistinctCleanCases.Count > 0)
                    {

                       string fileName = ClientVariables.SaveCSVClientReport(reportName, stringBuilder);
                        int ID = (int)Enums.EmailTemplates.ReporttoMobileOperator;
                        EmailTemplate template = EmailTemplate.Load(ID);
                        if (template.IsActive)
                        {
                            Email email = new Email() { EmailTemplateID = ID };
                            email.DestinationEmail = mobileOperator.NonFruadReportEmail;
                            email.Subject = string.Format("Fzero CLI Delivery Report {0}", DateTime.Today.ToString("dd-MM-yyyy"));
                            email.Body = @"Dear Sir/Madame,
                                          <br />Kindly find attached our Fzero CLI Delivery Report for your revision.
                                        
                                          <br /> <br />Best Regards,";
                            email.CC = "";
                          bool sentSuccessfully = Email.SendMailWithAttachement(email, fileName, profileName);
                          if (sentSuccessfully)
                                ErrorLog("Email Sent Successfully.");
                          else
                              ErrorLog("Send email failed.");
                        }
                       
                        SqlDatabase db = new SqlDatabase(ConfigurationManager.ConnectionStrings["FMSConnectionString"].ConnectionString);
                        var cmd = db.GetStoredProcCommand("dbo.[sp_ReportedCleanCalls_Insert]");
                        cmd.CommandTimeout = 600;
                        using (cmd)
                        {
                            string reportedCallsIds = string.Join(",", listDistinctCleanCases.Select(x => x.GeneratedCallId));
                            db.AssignParameters(cmd, new Object[] { reportedCallsIds });
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
