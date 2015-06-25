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