using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class CallSessionRepository
    {
        public static string AddNumbers(List<CallEntry> ListNumbers, string CallerId)
        {
            try
            {
                //string[] dialledNumbers = ListNumbers.Split(',');
                string sessionId = Guid.NewGuid().ToString();

                CallSession session = new CallSession();
                session.SessionId = sessionId;
                session.SipAccountId = 2;
                session.StartDate = DateTime.Now;
                session.CallerId = CallerId;
                int minutes = 0;
                int.TryParse("2", out minutes);
                session.CheckAfter = 0;
                //session.UserId = Guid.Parse(Membership.GetUser().ProviderUserKey.ToString());
                bool saved = CallSessionRepository.Insert(session);
                WriteToEventLogAn("Saved CSRepos" + saved.ToString());
                foreach (CallEntry entry in ListNumbers)
                {
                    CallEntry en = new CallEntry();
                    en.Number = entry.Number;
                    en.SessionId = sessionId;
                    en.Date = DateTime.Now;
                    en.Source = "Monty";
                    CallEntryRepository.Save(en);
                }
                return sessionId;
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                return null;
            }
        }

        public static CallSession GetCallSession(string sessionId)
        {
            CallSession Sessions = new CallSession();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<CallSession>(c => c.SipAccount);
                    context.LoadOptions = options;

                    Sessions = context.CallSessions.Where(x => x.SessionId == sessionId).SingleOrDefault<CallSession>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Sessions;
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

        public static List<CallSession> GetCallSessions()
        {
            List<CallSession> LstSessions = new List<CallSession>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<CallSession>(c => c.SipAccount);
                    context.LoadOptions = options;

                    LstSessions = context.CallSessions.Where(x => x.EndDate == null).ToList<CallSession>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                WriteToEventLog("ex::: " + ex.ToString());
                Logger.LogException(ex);
            }
            return LstSessions;
        }

        public static bool Save(CallSession callSession)
        {
            bool success = false;
            if (callSession.SessionId == default(string))
                success = Insert(callSession);
            else
                success = Update(callSession);
            return success;
        }

        private static bool Insert(CallSession callSession)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    WriteToEventLogEx(callSession.SessionId);
                    WriteToEventLogEx(callSession.StartDate.ToString());
                    WriteToEventLogEx(callSession.SipAccountId.ToString());
                    WriteToEventLogEx(callSession.UserId.ToString());
                    WriteToEventLogEx(callSession.TransactionId.ToString());


                    context.CallSessions.InsertOnSubmit(callSession);
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

        public static bool Update(CallSession callSession)
        {
            bool success = false;
            CallSession look = new CallSession();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.CallSessions.Single(l => l.SessionId == callSession.SessionId);

                    look.StartDate = callSession.StartDate;
                    look.ScheduleId = callSession.ScheduleId;
                    look.SipAccountId = callSession.SipAccountId;
                    look.EndDate = callSession.EndDate;
                    look.CheckAfter = callSession.CheckAfter;
                    look.UserId = callSession.UserId;
                    look.CallerId = callSession.CallerId;
                    look.TransactionId = callSession.TransactionId;
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("CSRep Update" + ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        private static void WriteToEventLogEx(string message)
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

        private static void WriteToEventLogAn(string message)
        {
            string cs = "Android Service";
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
