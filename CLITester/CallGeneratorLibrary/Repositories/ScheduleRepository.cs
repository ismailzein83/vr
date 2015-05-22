using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class DialledNumber
    {
        public int CLientId { get; set; }
        public string Number { get; set; }
    }

    public class ScheduleRepository
    {
        public static string GetName(int ScheduleId)
        {
            string s = "";

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    s = context.Schedules.Where(l => l.Id == ScheduleId).FirstOrDefault<Schedule>().DisplayName;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return s;
        }

        public static Schedule Load(int ScheduleId)
        {
            Schedule log = new Schedule();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.Schedules.Where(l => l.Id == ScheduleId).FirstOrDefault<Schedule>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<Schedule> GetSchedules()
        {
            List<Schedule> LstSchedules = new List<Schedule>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<Schedule>(c => c.User);
                    context.LoadOptions = options;

                    LstSchedules = context.Schedules.ToList<Schedule>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstSchedules;
        }

        public static List<Schedule> GetSchedules(int UserId)
        {
            List<Schedule> LstSchedules = new List<Schedule>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstSchedules = context.Schedules.Where(l => l.UserId == UserId).ToList<Schedule>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstSchedules;
        }

        public static List<Schedule> GetCurrentSchedules()
        {
            List<Schedule> LstSchedules = new List<Schedule>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstSchedules = context.Schedules.Where(l => ((l.StartDate >= DateTime.Now) && (l.EndDate <= DateTime.Now))).ToList<Schedule>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstSchedules;
        }
        
        public static bool Delete(int Id)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    List<ScheduleOperator> LstOperators = context.ScheduleOperators.Where(m => m.ScheduleId == Id).ToList<ScheduleOperator>();
                    context.ScheduleOperators.DeleteAllOnSubmit(LstOperators);

                    Schedule schedule = context.Schedules.Where(u => u.Id == Id).Single<Schedule>();
                    context.Schedules.DeleteOnSubmit(schedule);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        public static bool NewSchedule(Schedule schedule, List<ScheduleNumber> LstEntries)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Schedules.InsertOnSubmit(schedule);
                    context.SubmitChanges();
                    foreach (ScheduleNumber entry in LstEntries)
                    {
                        ScheduleNumber newentry = new ScheduleNumber();
                        newentry.Number = entry.Number;
                        newentry.ScheduleId = schedule.Id;
                        ScheduleNumberRepository.Save(newentry);
                    }
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        public static bool Save(Schedule schedule)
        {
            bool success = false;
            if (schedule.Id == default(int))
                success = Insert(schedule);
            else
                success = Update(schedule);
            return success;
        }

        private static bool Insert(Schedule schedule)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.Schedules.InsertOnSubmit(schedule);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        private static bool Update(Schedule schedule)
        {
            bool success = false;
            Schedule look = new Schedule();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.Schedules.Single(l => l.Id == schedule.Id);

                    look.DisplayName = schedule.DisplayName;
                    look.Frequency = schedule.Frequency;
                    look.TimeFrequency = schedule.TimeFrequency;
                    look.MonthDay = schedule.MonthDay;
                    look.OccursEvery = schedule.OccursEvery;
                    look.SpecificTime = schedule.SpecificTime;
                    look.SpecificTime1 = schedule.SpecificTime1;
                    look.StartDate = schedule.StartDate;
                    look.EndDate = schedule.EndDate;
                    look.SipAccountId = schedule.SipAccountId;
                    look.CreatedBy = schedule.CreatedBy;
                    look.CreationDate = schedule.CreationDate;
                    look.TotalNumbers = schedule.TotalNumbers;
                    look.RatioOfCalls = schedule.RatioOfCalls;
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        public static void create_session1(Schedule schedule)
        {
            try
            {
                string sessionId = Guid.NewGuid().ToString();
                using (CallGeneratorModelDataContext db = new CallGeneratorModelDataContext())
                {

                    CallSession session = new CallSession();
                    List<ScheduleNumber> dialledNumbers = (from nb in db.ScheduleNumbers
                                                           where nb.ScheduleId.Value == schedule.Id
                                                           select nb).ToList<ScheduleNumber>();
                    session.SessionId = sessionId;
                    session.SipAccountId = schedule.SipAccountId;
                    session.StartDate = DateTime.Now;
                    session.CallerId = (from sa in db.SipAccounts where sa.Id == schedule.SipAccountId select sa.DisplayName).SingleOrDefault<string>();
                    session.CheckAfter = 0;
                    //session.UserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
                    session.UserId = null;
                    db.CallSessions.InsertOnSubmit(session);
                    db.SubmitChanges();

                    foreach (ScheduleNumber number in dialledNumbers)
                    {
                        CallEntry entry = new CallEntry();
                        entry.Number = number.Number;
                        entry.SessionId = sessionId;
                        entry.Date = DateTime.Now.Date;
                        entry.Source = "Test Dial";

                        db.CallEntries.InsertOnSubmit(entry);
                    }
                    db.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
            }
        }


        private static void WriteToEventLog(string message)
        {
            string cs = "Call Generator Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        public static void create_session(Schedule schedule)
        {
            try
            {
                string sessionId = Guid.NewGuid().ToString();
                using (CallGeneratorModelDataContext db = new CallGeneratorModelDataContext())
                {
                    ScheduleLog currentlog = db.ScheduleLogs.Where(s => s.EndDate == null && s.ScheduleId == schedule.Id).FirstOrDefault();
                    
                    if(currentlog != null)
                        WriteToEventLog("currentlog " + currentlog.Id);

                    if (currentlog != null)
                        WriteToEventLog("currentlog.StartDate " + currentlog.StartDate + " " + DateTime.Now);

                    if (currentlog != null && currentlog.StartDate <= DateTime.Now)
                    {
                        CallSession session = new CallSession();
                        List<DialledNumber> dialledNbs = new List<DialledNumber>();
                        List<TestNumber> dialledNumbers = new List<TestNumber>();

                        dialledNumbers = (from nb in db.TestNumbers
                                                       join sg in db.ScheduleGroups on nb.GroupId equals sg.GroupId
                                                       where sg.ScheduleId == schedule.Id
                                                       && nb.IsDeleted == null
                                       select nb).ToList<TestNumber>();

                        WriteToEventLog("dialledNumbers " + dialledNumbers.Count());
                        foreach (TestNumber tn in dialledNumbers)
                        {
                            DialledNumber d = new DialledNumber();
                            d.Number = tn.Number;
                            if (tn.GroupId == 196 || tn.GroupId == 197 || tn.GroupId == 198)
                                d.CLientId = 2;
                            if (tn.GroupId == 199 || tn.GroupId == 200 || tn.GroupId == 201)
                                d.CLientId = 1;
                            if (tn.GroupId == 202 || tn.GroupId == 203 || tn.GroupId == 204)
                                d.CLientId = 3;
                            dialledNbs.Add(d);
                        }
                        session.SessionId = sessionId;
                        session.SipAccountId = schedule.SipAccountId;
                        session.StartDate = DateTime.Now;
                        session.CallerId = (from sa in db.SipAccounts where sa.Id == schedule.SipAccountId select sa.DisplayName).SingleOrDefault<string>();
                        session.CheckAfter = 0;
                        //session.UserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
                        session.UserId = null;
                        db.CallSessions.InsertOnSubmit(session);
                        db.SubmitChanges();
                        WriteToEventLog("session.SessionId " + session.SessionId + " session.EndDate " + session.EndDate);
                        foreach (DialledNumber number in dialledNbs)
                        {
                            CallEntry entry = new CallEntry();

                            entry.Number = number.Number;
                            entry.ClientId = number.CLientId;
                            entry.SessionId = sessionId;
                            entry.Date = DateTime.Now;
                            entry.Source = "Test Dial";
                            entry.StartDate = DateTime.Now;
                            // we need to add here the CGPN...!
                            //entry.Cgpn = null;
                            db.CallEntries.InsertOnSubmit(entry);
                        }
                        //ScheduleLog lastlog = db.ScheduleLogs.Where(s => s.ScheduleId == schedule.Id && s.EndDate == null).FirstOrDefault();
                        //lastlog.EndDate = DateTime.Now;

                        WriteToEventLog("Schedule Repos, currentlog.ID: " + currentlog.Id);

                        currentlog.EndDate = DateTime.Now;
                        db.SubmitChanges();
                        //Logger.Log(Vanrise.FraudDetection.Logging.ActionType.Add, Vanrise.FraudDetection.Logging.ActionModule.DialSession, sessionId, "");
                    }

                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                WriteToEventLog("Error at Call Distributor: create_session: " + ex.ToString());
                //log.Error("Error at Call Distributor: create_session: " + ex.ToString());
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