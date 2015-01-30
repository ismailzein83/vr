using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class CallGeneratorLogEntryRepository
    {
        public static bool Save(CallGeneratorLogEntry callGeneratorLogEntry)
        {
            bool success = false;
            if (callGeneratorLogEntry.Id == default(int))
                success = Insert(callGeneratorLogEntry);
            else
                success = Update(callGeneratorLogEntry);
            return success;
        }

        private static bool Insert(CallGeneratorLogEntry callGeneratorLogEntry)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.CallGeneratorLogEntries.InsertOnSubmit(callGeneratorLogEntry);
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

        private static bool Update(CallGeneratorLogEntry callGeneratorLogEntry)
        {
            bool success = false;
            CallGeneratorLogEntry look = new CallGeneratorLogEntry();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.CallGeneratorLogEntries.Single(l => l.Id == callGeneratorLogEntry.Id);

                    look.ScheduleLogId = callGeneratorLogEntry.ScheduleLogId;
                    look.Message = callGeneratorLogEntry.Message;
                    look.Number = callGeneratorLogEntry.Number;
                    look.Status = callGeneratorLogEntry.Status;
                    look.ConnectionId = callGeneratorLogEntry.ConnectionId;
                    look.LineId = callGeneratorLogEntry.LineId;
                    look.Date = callGeneratorLogEntry.Date;
                    look.SessionId = callGeneratorLogEntry.SessionId;
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
