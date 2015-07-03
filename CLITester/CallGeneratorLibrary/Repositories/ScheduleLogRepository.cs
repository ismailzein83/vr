﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class ScheduleLogRepository
    {
        public static ScheduleLog Load(int LogId)
        {
            ScheduleLog log = new ScheduleLog();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.ScheduleLogs.Where(l => l.Id == LogId).FirstOrDefault<ScheduleLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static ScheduleLog GetLastLog(int ScheduleId)
        {
            ScheduleLog log = new ScheduleLog();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.ScheduleLogs.Where(l => l.ScheduleId == ScheduleId && l.EndDate == null).OrderByDescending(s => s.StartDate).FirstOrDefault<ScheduleLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<ScheduleLog> GetCurrentScheduleLog()
        {
            List<ScheduleLog> LstSchedules = new List<ScheduleLog>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    //LstSchedules = context.ScheduleLogs.Where(l => ((l.StartDate >= DateTime.Now) && (l.EndDate <= DateTime.Now))).ToList<Schedule>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstSchedules;
        }

        public static bool Save(ScheduleLog scheduleLog)
        {
            bool success = false;
            if (scheduleLog.Id == default(int))
                success = Insert(scheduleLog);
            else
                success = Update(scheduleLog);
            return success;
        }

        private static bool Insert(ScheduleLog scheduleLog)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ScheduleLogs.InsertOnSubmit(scheduleLog);
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

        private static bool Update(ScheduleLog scheduleLog)
        {
            bool success = false;
            ScheduleLog scheduleLogObj = new ScheduleLog();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    scheduleLogObj = context.ScheduleLogs.Single(l => l.Id == scheduleLog.Id);

                    scheduleLogObj.ScheduleId = scheduleLog.ScheduleId;
                    scheduleLogObj.StartDate = scheduleLog.StartDate;
                    scheduleLogObj.EndDate = scheduleLog.EndDate;
                    scheduleLogObj.Frequency = scheduleLog.Frequency;
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

        public static bool Delete(int scheduleLog)
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    ScheduleLog ScheduleLog = context.ScheduleLogs.Where(u => u.Id == scheduleLog).Single<ScheduleLog>();
                    context.ScheduleLogs.DeleteOnSubmit(ScheduleLog);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
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
