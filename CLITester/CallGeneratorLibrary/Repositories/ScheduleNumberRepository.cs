using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class ScheduleNumberRepository
    {
        public static bool Save(ScheduleNumber scheduleNumber)
        {
            bool success = false;
            if (scheduleNumber.Id == default(int))
                success = Insert(scheduleNumber);
            else
                success = Update(scheduleNumber);
            return success;
        }

        private static bool Insert(ScheduleNumber scheduleNumber)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ScheduleNumbers.InsertOnSubmit(scheduleNumber);
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

        private static bool Update(ScheduleNumber scheduleNumber)
        {
            bool success = false;
            ScheduleNumber look = new ScheduleNumber();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.ScheduleNumbers.Single(l => l.Id == scheduleNumber.Id);

                    look.ScheduleId = scheduleNumber.ScheduleId;
                    look.Number = scheduleNumber.Number;
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
