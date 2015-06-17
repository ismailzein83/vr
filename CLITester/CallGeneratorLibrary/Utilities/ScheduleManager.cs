using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Repositories;
using System.Diagnostics;
using System.Net.Mail;
using System.Web.Configuration;
using System.Configuration;
using System.Net;
using System.Data;
using Microsoft.Reporting;
using Microsoft.Reporting.WebForms;
using System.IO;
namespace CallGeneratorLibrary.Utilities
{
    public class ScheduleManager
    {
        private static readonly object _syncRoot = new object();

        public static void CLISchedule()
        {
            lock (_syncRoot)
            try
            {
                ScheduleLog currentSchedule = null;

                using (CallGeneratorModelDataContext db = new CallGeneratorModelDataContext())
                {
                    List<Schedule> schedules = ScheduleRepository.GetSchedules();

                    foreach (Schedule schedule in schedules)
                    {
                        
                        ScheduleLog log = db.ScheduleLogs.Where(s => s.ScheduleId == schedule.Id && s.EndDate == null).OrderByDescending(s => s.StartDate).FirstOrDefault();

                        if (log == null)
                        {
                            ScheduleLog lastlog = db.ScheduleLogs.Where(s => s.ScheduleId == schedule.Id && s.EndDate != null).OrderByDescending(s => s.StartDate).FirstOrDefault();

                            DateTime lastRunDate = schedule.StartDate.Value;

                            if (lastlog != null)
                                lastRunDate = lastlog.StartDate.Value;

                            DateTime currentRunDate = DateTime.Now;

                            if (schedule.StartDate <= DateTime.Now && (schedule.EndDate == null || schedule.EndDate.Value >= DateTime.Now))
                            {
                                if (!schedule.OccursEvery.HasValue || schedule.OccursEvery.Value == 0) schedule.OccursEvery = 1;
                                if (lastlog == null)
                                {
                                    if (schedule.SpecificTime.HasValue)
                                    {
                                        DateTime spec = (DateTime)schedule.SpecificTime.Value;
                                        DateTime specificTime = new DateTime(2000, 1, 1, spec.Hour, spec.Minute, spec.Second);
                                        DateTime now = new DateTime(2000, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                        if (specificTime > now)
                                        {
                                            currentRunDate = new DateTime(currentRunDate.Date.Year, currentRunDate.Date.Month, currentRunDate.Date.Day,
                                                            schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute,
                                                            schedule.SpecificTime.Value.Second);
                                        }
                                        else
                                        {
                                            currentRunDate = new DateTime(currentRunDate.Date.Year, currentRunDate.Date.Month, currentRunDate.Date.Day,
                                                            schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute,
                                                            schedule.SpecificTime.Value.Second).AddDays(1);
                                        }
                                    }
                                }
                                else
                                {
                                    if (schedule.SpecificTime1 == null)
                                    {
                                        if (schedule.SpecificTime != null && (lastRunDate.Hour < schedule.SpecificTime.Value.Hour))
                                        {
                                            currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                                    schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute, schedule.SpecificTime.Value.Second);
                                            WriteToEventLogEx("currentRunDate21: " + currentRunDate);
                                        }
                                        else
                                        {
                                            currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                                        schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute, schedule.SpecificTime.Value.Second).AddDays(1);
                                            WriteToEventLogEx("currentRunDate31: " + currentRunDate);
                                        }
                                    }
                                    else
                                    {
                                        if (lastRunDate.Hour < schedule.SpecificTime.Value.Hour)
                                        {
                                            currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                                schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute, schedule.SpecificTime.Value.Second);

                                            WriteToEventLogEx("currentRunDate41: " + currentRunDate);
                                        }
                                        else
                                        {
                                            if (lastRunDate.Hour < schedule.SpecificTime1.Value.Hour)
                                            {
                                                currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                                    schedule.SpecificTime1.Value.Hour, schedule.SpecificTime1.Value.Minute, schedule.SpecificTime1.Value.Second);

                                                WriteToEventLogEx("currentRunDate51: " + currentRunDate);
                                            }
                                            else
                                            {
                                                currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                                    schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute, schedule.SpecificTime.Value.Second).AddDays(1);

                                                WriteToEventLogEx("currentRunDate61: " + currentRunDate);
                                            }
                                        }
                                    }
                                    //currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                    //            lastRunDate.Hour, lastRunDate.Minute, lastRunDate.Second).AddDays(1);
                                }

                                WriteToEventLogEx("currentRunDate:::: " + currentRunDate);

                                currentSchedule = new ScheduleLog();
                                currentSchedule.ScheduleId = schedule.Id;
                                currentSchedule.StartDate = currentRunDate;
                                currentSchedule.Frequency = 0;
                                ScheduleLogRepository.Save(currentSchedule);
                                WriteToEventLogEx("currentSchedule:::: " + currentSchedule.Id);
                                //db.ScheduleLogs.InsertOnSubmit(currentSchedule);
                                //db.SubmitChanges();

                                //List<ScheduleOperator> LstShcOp = ScheduleOperatorRepository.GetScheduleOperatorsByScheduleId(schedule.Id);
                                //foreach (ScheduleOperator SchOp in LstShcOp)
                                //{
                                //    TestOperator testOp = new TestOperator();
                                //    testOp.UserId = schedule.UserId;
                                //    testOp.OperatorId = SchOp.OperatorId;
                                //    testOp.NumberOfCalls = 1;
                                //    testOp.CreationDate = DateTime.Now;
                                //    testOp.CarrierPrefix = SchOp.Carrier.Prefix;
                                //    testOp.CallerId = schedule.User.CallerId;
                                //    testOp.ScheduleId = schedule.Id;
                                //    bool saveB = TestOperatorRepository.Save(testOp);
                                //}
                                //WriteToEventLogEx("5");
                            }
                        }
                        else
                        {
                            ScheduleLog NewLog = ScheduleLogRepository.Load(log.Id);

                            DateTime dt = new DateTime(
                                schedule.SpecificTime.Value.Date.Year, schedule.SpecificTime.Value.Date.Month, schedule.SpecificTime.Value.Date.Day,
                                NewLog.StartDate.Value.Hour, NewLog.StartDate.Value.Minute, NewLog.StartDate.Value.Second);
                            DateTime dt1 = new DateTime();
                            if (schedule.SpecificTime1 != null)
                                dt1 = new DateTime(
                                    schedule.SpecificTime1.Value.Date.Year, schedule.SpecificTime1.Value.Date.Month, schedule.SpecificTime1.Value.Date.Day,
                                    NewLog.StartDate.Value.Hour, NewLog.StartDate.Value.Minute, NewLog.StartDate.Value.Second);

                            TimeSpan span = new TimeSpan();
                            TimeSpan span1 = new TimeSpan();

                            TimeSpan span2 = new TimeSpan();

                            if (dt != null)
                            {
                                if (schedule.SpecificTime.HasValue)
                                {
                                    span = schedule.SpecificTime.Value - dt;
                                    if (schedule.SpecificTime1.HasValue)
                                    {
                                        span1 = schedule.SpecificTime1.Value - dt1;

                                        if (span.TotalSeconds != 0 && span1.TotalSeconds != 0)
                                        {
                                            DateTime dt2 = new DateTime(
                                        DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day,
                                        schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute, schedule.SpecificTime.Value.Second);

                                            span2 = dt2 - DateTime.Now;
                                            if (span2.TotalSeconds >= 0)
                                            {
                                                NewLog.StartDate = dt2;
                                            }
                                            else
                                            {
                                                NewLog.StartDate = dt2.AddDays(1);
                                            }

                                            WriteToEventLogEx("NewLog4 " + NewLog.Id + " " + NewLog.StartDate);

                                            ScheduleLogRepository.Save(NewLog);
                                        }
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds != 0)
                                        {
                                            DateTime dt2 = new DateTime(
                                        DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day,
                                        schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute, schedule.SpecificTime.Value.Second);

                                            span2 = dt2 - DateTime.Now;
                                            if (span2.TotalSeconds >= 0)
                                            {
                                                NewLog.StartDate = dt2;
                                            }
                                            else
                                            {
                                                NewLog.StartDate = dt2.AddDays(1);
                                            }

                                            WriteToEventLogEx("NewLog4 " + NewLog.Id + " " + NewLog.StartDate);

                                            ScheduleLogRepository.Save(NewLog);
                                        }
                                    }
                                }
                            }
                            
                            if (NewLog.Frequency < schedule.OccursEvery)
                            {
                                DateTime start = NewLog.StartDate.Value;
                                double min = NewLog.Frequency.Value * 2;
                                start = start.AddMinutes(min);
                                //WriteToEventLogEx("Frequency " + NewLog.Frequency.Value);


                                if (start <= DateTime.Now)
                                {
                                    
                                    //currentSchedule = log;
                                    NewLog.Frequency++;

                                    WriteToEventLogEx("NewLog5 " + NewLog.Id + " " + NewLog.StartDate);
                                    ScheduleLogRepository.Save(NewLog);
                                    List<ScheduleOperator> LstShcOp = ScheduleOperatorRepository.GetScheduleOperatorsByScheduleId(schedule.Id);

                                    int balance = 0;
                                    int Requested = 0;
                                    int ParentId = 0;
                                    ParentId = UserRepository.GetParentId(schedule.UserId.Value);

                                    balance = UserRepository.Load(schedule.UserId.Value).Balance.Value;

                                    Requested = TestOperatorRepository.GetRequestedTestOperatorsByUser(ParentId);

                                    if (balance - Requested >= LstShcOp.Count)
                                    {
                                        foreach (ScheduleOperator SchOp in LstShcOp)
                                        {
                                            //if (SchOp.Frequency <= NewLog.Frequency)
                                            if (NewLog.Frequency <= SchOp.Frequency)
                                            {
                                                TestOperator testOp = new TestOperator();
                                                testOp.UserId = schedule.UserId;
                                                testOp.ParentUserId = ParentId;
                                                testOp.OperatorId = SchOp.OperatorId;
                                                testOp.NumberOfCalls = 1;
                                                testOp.CreationDate = DateTime.Now;
                                                testOp.CarrierPrefix = SchOp.Carrier.Prefix;
                                                testOp.CallerId = schedule.User.CallerId;
                                                testOp.ScheduleId = schedule.Id;
                                                bool saveB = TestOperatorRepository.Save(testOp);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        WriteToEventLogEx("NewLog1 " + NewLog.Id + " " + NewLog.StartDate);
                                        
                                        NewLog.EndDate = DateTime.Now;
                                        ScheduleLogRepository.Save(NewLog);

                                        ActionLog action = new ActionLog();
                                        action.ObjectId = schedule.Id;
                                        action.ObjectType = "Schedule";
                                        action.ActionType = (int)Enums.ActionType.ScheduleFailed;
                                        action.UserId = schedule.UserId;
                                        AuditRepository.Save2(action);

                                        //Schedule Log finished => Send Email
                                        //WriteToEventLogEx("DONEE " + schedule.User.Id);
                                        while (SendFailureEmail(schedule.User, NewLog) == false) ;
                                    }
                                }
                            }
                            else
                            {
                                WriteToEventLogEx("NewLog2 " + NewLog.Id + " " + NewLog.StartDate);
                                NewLog.EndDate = DateTime.Now;
                                ScheduleLogRepository.Save(NewLog);

                                ActionLog action = new ActionLog();
                                action.ObjectId = schedule.Id;
                                action.ObjectType = "Schedule";
                                action.ActionType = (int)Enums.ActionType.ScheduleDone;
                                action.UserId = schedule.UserId;
                                AuditRepository.Save2(action);

                                //Schedule Log finished => Send Email
                                //WriteToEventLogEx("DONEE " + schedule.User.Id);
                                while(SendEmail2(schedule.User, NewLog) == false);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
            }
        }

        public static ScheduleLog CurrentSchedule
        {
            get
            {
                try
                {
                    ScheduleLog currentSchedule = null;

                    using (CallGeneratorModelDataContext db = new CallGeneratorModelDataContext())
                    {
                        //List<ScheduleLog> LstScheLogs = db.ScheduleLogs.Where(s => s.EndDate == null).ToList<ScheduleLog>();

                        //foreach (ScheduleLog schedLog in LstScheLogs)
                        //{
                        //    TimeSpan span = DateTime.Now - schedLog.StartDate.Value;
                        //    double totalMinutes = span.TotalMinutes;

                        //    if (totalMinutes > 4)
                        //    {
                        //        schedLog.EndDate = DateTime.Now;
                        //        db.SubmitChanges();
                        //    }
                        //}

                        List<Schedule> schedules = db.Schedules.ToList<Schedule>();
                        //Get all entries from a schedule
                        //Get all Occurs Every by a specific time.
                        //

                        foreach (Schedule schedule in schedules)
                        {
                            ScheduleLog log = db.ScheduleLogs.Where(s => s.ScheduleId == schedule.Id && s.EndDate == null).OrderByDescending(s => s.StartDate).FirstOrDefault();

                            if (log == null)
                            {
                                ScheduleLog lastlog = db.ScheduleLogs.Where(s => s.ScheduleId == schedule.Id && s.EndDate != null).OrderByDescending(s => s.StartDate).FirstOrDefault();

                                DateTime lastRunDate = schedule.StartDate.Value;

                                if (lastlog != null)
                                    lastRunDate = lastlog.StartDate.Value;

                                DateTime currentRunDate = DateTime.Now;

                                if (schedule.StartDate <= DateTime.Now && (schedule.EndDate == null || schedule.EndDate.Value >= DateTime.Now))
                                {
                                    if (schedule.TimeFrequency.HasValue && schedule.TimeFrequency.Value > 0)
                                    {
                                        if (lastlog != null)
                                        {
                                            currentRunDate = lastRunDate.AddMinutes(schedule.TimeFrequency.Value);
                                        }
                                        else
                                            currentRunDate = currentRunDate.AddMinutes(schedule.TimeFrequency.Value);
                                    }
                                    else if (schedule.Frequency.HasValue)
                                    {
                                        //if (schedule.Frequency.Value == ScheduleFrequency.Daily.GetHashCode())
                                        //{
                                        //    if (lastlog == null && schedule.OccursEvery.HasValue)
                                        //        currentRunDate = currentRunDate.AddDays(schedule.OccursEvery.Value);
                                        //    else if (schedule.OccursEvery.HasValue)//if occurs every day is set
                                        //    {
                                        //        currentRunDate = lastRunDate.Date.AddDays(schedule.OccursEvery.Value);
                                        //    }
                                        //}

                                        if (schedule.Frequency.Value == ScheduleFrequency.Daily.GetHashCode())
                                        {
                                            if (!schedule.OccursEvery.HasValue || schedule.OccursEvery.Value == 0) schedule.OccursEvery = 1;
                                            if (lastlog == null)
                                            {
                                                if (schedule.SpecificTime.HasValue)
                                                {
                                                    DateTime spec = (DateTime)schedule.SpecificTime.Value;
                                                    DateTime specificTime = new DateTime(2000, 1, 1, spec.Hour, spec.Minute, spec.Second);
                                                    DateTime now = new DateTime(2000, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                                                    if (specificTime > now)
                                                    {
                                                        currentRunDate = new DateTime(currentRunDate.Date.Year, currentRunDate.Date.Month, currentRunDate.Date.Day,
                                                                        schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute,
                                                                        schedule.SpecificTime.Value.Second);
                                                    }
                                                    else
                                                    {
                                                        currentRunDate = new DateTime(currentRunDate.Date.Year, currentRunDate.Date.Month, currentRunDate.Date.Day,
                                                                        schedule.SpecificTime.Value.Hour, schedule.SpecificTime.Value.Minute,
                                                                        schedule.SpecificTime.Value.Second).AddDays(schedule.OccursEvery.Value);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                currentRunDate = new DateTime(lastRunDate.Date.Year, lastRunDate.Date.Month, lastRunDate.Date.Day,
                                                            lastRunDate.Hour, lastRunDate.Minute, lastRunDate.Second).AddDays(schedule.OccursEvery.Value);
                                            }
                                        }

                                        else if (schedule.Frequency.Value == ScheduleFrequency.Weekly.GetHashCode())
                                        {
                                            if (schedule.ScheduleWeekDays != null)
                                            {
                                                foreach (ScheduleWeekDay day in schedule.ScheduleWeekDays.OrderBy(x => x.WeekDay.Value))
                                                {
                                                    if (currentRunDate.DayOfWeek.GetHashCode() <= day.WeekDay.Value)
                                                    {
                                                        currentRunDate = currentRunDate.AddDays(day.WeekDay.Value - currentRunDate.DayOfWeek.GetHashCode());
                                                    }
                                                }
                                            }
                                        }
                                        else if (schedule.Frequency.Value == ScheduleFrequency.Monthly.GetHashCode())
                                        {
                                            if (schedule.MonthDay.HasValue)
                                            {
                                                if (currentRunDate.Day <= schedule.MonthDay.Value)
                                                {
                                                    currentRunDate = new DateTime(currentRunDate.Year, currentRunDate.Month, schedule.MonthDay.Value);
                                                }
                                            }
                                        }
                                    }

                                    currentSchedule = new ScheduleLog();
                                    currentSchedule.Schedule = schedule;
                                    currentSchedule.StartDate = currentRunDate;
                                    db.ScheduleLogs.InsertOnSubmit(currentSchedule);
                                    db.SubmitChanges();
                                }
                            }
                            else
                            {
                                if (log.StartDate <= DateTime.Now)
                                    currentSchedule = log;
                            }
                        }
                    }
                    return currentSchedule;
                }
                catch (System.Exception ex)
                {
                    WriteToEventLogEx(ex.ToString());
                    return null;
                }
            }
        }

        public static bool SendEmail2(User member, ScheduleLog s)
        {
            try
            {
                List<TestOperator> LstOperators = TestOperatorRepository.GetTestOperatorsByScheduleLogId(s.StartDate.Value, s.ScheduleId.Value);

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
               new DataColumn("OperatorName",typeof(System.String))
             , new DataColumn("ScheduleName",typeof(System.String))
             , new DataColumn("CreationDate",typeof(System.String))
             , new DataColumn("EndDate",typeof(System.String))
             , new DataColumn("TestCli",typeof(System.String))
             , new DataColumn("ReceivedCli",typeof(System.String))
             , new DataColumn("Status",typeof(System.String))
            });

                foreach (TestOperator app in LstOperators)
                {
                    DataRow row = dt.NewRow();

                    if (app.Operator.FullName != null)
                        row["OperatorName"] = app.Operator.FullName;

                    if (app.Schedule.DisplayName != null)
                        row["ScheduleName"] = app.Schedule.DisplayName;

                    if (app.CreationDate != null)
                        row["CreationDate"] = app.CreationDate;

                    if (app.EndDate != null)
                        row["EndDate"] = app.EndDate;

                    if (app.TestCli != null)
                        row["TestCli"] = app.TestCli;

                    if (app.ReceivedCli != null)
                        row["ReceivedCli"] = app.ReceivedCli;

                    if (app.Status != null)
                        row["Status"] = app.Status;

                    dt.Rows.Add(row);
                }


                ///Attach a file
                //string[][] output = new string[][]{
                //    new string[]{"Name","Schedule","Creation Date","End Date","Test Cli","Received Cli","Status"}
                //    };
                //int ii = 0;

                StringBuilder EmailBody = new StringBuilder();
                EmailBody.Append("<table cellspacing='0' cellpadding='0'>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 13pt; font-weight: bold'>CLITester Website</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Dear&nbsp;" + member.Name + ",</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Please check out last schedule log details:</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'><table><tr style='font-family: Arial; font-size: 13pt; font-weight: bold'><td style='Width: 150px;'>Name</td><td style='Width: 150px;'>Schedule</td><td style='Width: 180px;'>Creation Date</td><td style='Width: 180px;'>End Date</td><td style='Width: 180px;'>Test Cli</td><td style='Width: 180px;'>Received Cli</td><td style='Width: 130px;'>Status</td></tr>");
                bool trouv = false;
                foreach (TestOperator t in LstOperators)
                {
                    if (t.EndDate == null)
                    {
                        trouv = true;
                    }

                    //WriteToEventLogEx("ID:: " + t.Id  +" t.Operator " + t.Operator + " t.Schedule.DisplayName " + t.Schedule.DisplayName
                    //    + " t.TestCli " + t.TestCli + " t.ReceivedCli " + t.ReceivedCli + " t.CreationDate "
                    //    + t.CreationDate + " t.EndDate " + t.EndDate + " t.Status " + t.Status);

                    string opname = "";
                    if (t.Operator != null)
                        opname = t.Operator.FullName;

                    string scname = "";
                    if (t.Schedule.DisplayName != null)
                        scname = t.Schedule.DisplayName;

                    string tstCli = "";
                    if (t.TestCli != null)
                        tstCli = t.TestCli;

                    string recCli = "";
                    if (t.ReceivedCli != null)
                        recCli = t.ReceivedCli;

                    string creadate = "";
                    if (t.CreationDate != null)
                        creadate = t.CreationDate.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    string enddate = "";
                    if (t.EndDate != null)
                        enddate = t.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    string ss = "";
                    if (t.Status == null)
                        ss = "No status";
                    else if (t.Status == 1)
                        ss = "CLI Delivered";
                    else
                        ss = "CLI not delivered";

                    // WriteToEventLogEx("member.Email " + member.Email.ToString());
                    EmailBody.Append("<tr><td>" + opname + "</td><td>" + scname + "</td><td>" + creadate + "</td><td>" + enddate + "</td><td>" + tstCli + "</td><td>" + recCli + "</td><td>" + ss + "</td></tr>");

                    
                    ///Attach a file
                    //string[] sadd = { opname, scname, creadate, enddate, tstCli, recCli, ss };
                    //output[ii] = sadd;
                    //ii = ii + 1;
                }





                EmailBody.Append("</table></td></tr>");

                //EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Password = " + member.Password + "</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>For more information, don't hesitate to contact us.</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Thanks,</td></tr>");
                EmailBody.Append("<tr><td><a style='font-family: Arial; font-size: 11pt' href='http://www.vanrise.com'>www.vanrise.com</a> Team</td></tr>");
                EmailBody.Append("</table>");

                if (trouv == false)
                {

                    ///Attach a file
                    //string filePath = "C:\\Android\\Resources\\" + DateTime.Now.ToString("yyyymmddhhmmss") + ".csv";

                    ////filePath = "" + "filename" + ".csv";
                    //if (!File.Exists(filePath))
                    //{
                    //    File.Create(filePath).Close();
                    //}
                    //string delimiter = ",";

                    //int length = output.GetLength(0);
                    //StringBuilder sb = new StringBuilder();
                    //for (int index = 0; index < length; index++)
                    //    sb.AppendLine(string.Join(delimiter, output[index]));
                    //File.AppendAllText(filePath, sb.ToString());


                    MailMessage objMail = new MailMessage();

                    objMail.To.Add(member.Email);

                    string strEmailFrom = ConfigurationSettings.AppSettings["SendingEmail"];

                    objMail.From = new MailAddress(strEmailFrom, "CLI Tester");
                    objMail.Subject = "CLITester - Schedule done";
                    objMail.Body = EmailBody.ToString();
                    objMail.IsBodyHtml = true;
                    objMail.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"], 587);
                    smtp.Host = ConfigurationSettings.AppSettings["SmtpServer"];
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(strEmailFrom, "passwordQ1");
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                    smtp.Send(objMail);
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("ex " + ex.ToString());
                return false;
            }
        }

        private static bool SendFailureEmail(User member,ScheduleLog s)
        {
            try
            {
                StringBuilder EmailBody = new StringBuilder();
                EmailBody.Append("<table cellspacing='0' cellpadding='0'>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 13pt; font-weight: bold'>CLITester Website</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Dear&nbsp;" + member.Name + ",</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>The Schedule cannot be done since your balance is empty:</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");

                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>For more information, don't hesitate to contact us.</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td>&nbsp;</td></tr>");
                EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Thanks,</td></tr>");
                EmailBody.Append("<tr><td><a style='font-family: Arial; font-size: 11pt' href='http://www.vanrise.com'>www.vanrise.com</a> Team</td></tr>");
                EmailBody.Append("</table>");

                MailMessage objMail = new MailMessage();

                objMail.To.Add(member.Email);

                string strEmailFrom = ConfigurationSettings.AppSettings["SendingEmail"];
                    
                objMail.From = new MailAddress(strEmailFrom, "CLI Tester");
                objMail.Subject = "CLITester - Schedule Failed";
                objMail.Body = EmailBody.ToString();
                objMail.IsBodyHtml = true;
                objMail.Priority = MailPriority.High;

                SmtpClient smtp = new SmtpClient(ConfigurationSettings.AppSettings["SmtpServer"], 587);
                smtp.Host = ConfigurationSettings.AppSettings["SmtpServer"];
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(strEmailFrom, "passwordQ1");
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                smtp.Send(objMail);
                return true;
            }
            catch(System.Exception ex) {
                WriteToEventLogEx("ex " + ex.ToString());
                return false;
            }
        }

        private static void WriteToEventLogEx(string message)
        {
            string cs = "LogCallGen";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
    }
}
