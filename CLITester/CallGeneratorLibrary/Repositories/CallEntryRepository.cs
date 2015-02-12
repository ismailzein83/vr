using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class CallEntryRepository
    {
        public static CallEntry TopCallEntry()
        {
            CallEntry log = new CallEntry();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.CallEntries.OrderByDescending(l => l.Id).FirstOrDefault<CallEntry>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static CallEntry Load(int entryId)
        {
            CallEntry log = new CallEntry();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.CallEntries.Where(l => l.Id == entryId).FirstOrDefault<CallEntry>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<CallEntry> GetCallEntries()
        {
            List<CallEntry> LstEntries = new List<CallEntry>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstEntries = context.CallEntries.Where(x => x.EndDate == null).ToList<CallEntry>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstEntries;
        }

        public static CallEntry GetCallEntryByNumberSession(string number, string sessionId)
        {
           CallEntry entry = new CallEntry();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //WriteToEventLogEx("ENTRY REP 1");
                    entry = context.CallEntries.Where(x => ((number == null || x.Number == number) && (sessionId == null || x.SessionId == sessionId))).SingleOrDefault<CallEntry>();
                   // WriteToEventLogEx("ENTRY REP 2");
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return entry;
        }

        public static List<CallEntry> GetCallEntryBySession(string sessionId)
        {
            List<CallEntry> LstEntries = new List<CallEntry>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<CallEntry>(c => c.CallEntryStatus);
                    context.LoadOptions = options;

                    LstEntries = context.CallEntries.Where(x => (sessionId == null || x.SessionId == sessionId)).ToList<CallEntry>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstEntries;
        }

        public static List<CallEntry> GetCallEntries(string sessionId)
        {
            List<CallEntry> LstEntries = new List<CallEntry>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.CommandTimeout = 18000;
                    
                    LstEntries = context.CallEntries.Where(x => ((sessionId == null || x.SessionId == sessionId) && x.EndDate == null)).ToList<CallEntry>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstEntries;
        }

        public static bool Save(CallEntry callEntry)
        {
            bool success = false;
            if (callEntry.Id == default(int))
                success = Insert(callEntry);
            else
                success = Update(callEntry);
            return success;
        }   

        private static bool Insert(CallEntry callEntry)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.CallEntries.InsertOnSubmit(callEntry);
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

        private static bool Update(CallEntry callEntry)
        {
            bool success = false;
            CallEntry look = new CallEntry();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //WriteToEventLogEx("CE REP callEntry.Id: " + callEntry.Id);

                    context.CommandTimeout = 18000;

                    look = context.CallEntries.Single(l => l.Id == callEntry.Id);

                    look.Number = callEntry.Number;
                    look.Date = callEntry.Date;
                    look.StatusId = callEntry.StatusId;
                    look.StartDate = callEntry.StartDate;
                    look.EndDate = callEntry.EndDate;
                    look.Source = callEntry.Source;
                    look.IsProcessed = callEntry.IsProcessed;
                    look.SessionId = callEntry.SessionId;
                    look.Cgpn = callEntry.Cgpn;
                    look.ResponseCode = callEntry.ResponseCode;
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
    }
}
