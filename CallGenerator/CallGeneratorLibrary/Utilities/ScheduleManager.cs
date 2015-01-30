using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Repositories;
using System.Diagnostics;

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
                            ScheduleLog log = db.ScheduleLogs.Where(s => s.ScheduleId == schedule.Id).OrderByDescending(s => s.StartDate).FirstOrDefault();

                            if (log == null)
                            {
                                DateTime currentRunDate = DateTime.Now;
                                currentRunDate = currentRunDate.AddMinutes((double)schedule.OccursEvery);

                                currentSchedule = new ScheduleLog();
                                currentSchedule.ScheduleId = schedule.Id;
                                currentSchedule.StartDate = currentRunDate;
                                currentSchedule.EndDate = null;
                                currentSchedule.Frequency = null;
                                db.ScheduleLogs.InsertOnSubmit(currentSchedule);
                                db.SubmitChanges();
                            }
                            else
                            {
                                if (log.EndDate == null && log.StartDate <= DateTime.Now)
                                {
                                    List<ScheduleNumbers> LstShcOp = ScheduleGroupRepository.GetScheduleNumbers(schedule.Id);
                                    foreach (ScheduleNumbers SchOp in LstShcOp)
                                    {
                                        GeneratedCall GenCall = new GeneratedCall();
                                        GenCall.Number = SchOp.Number;
                                        GenCall.SipAccountId = schedule.SipAccountId;
                                        GenCall.ScheduleId = schedule.Id;
                                        GenCall.ClientId = 0;
                                        
                                        if (SchOp.GroupId == 199 || SchOp.GroupId == 200 || SchOp.GroupId == 201)
                                            GenCall.ClientId = 1;

                                        if (SchOp.GroupId == 196 || SchOp.GroupId == 197 || SchOp.GroupId == 197)
                                            GenCall.ClientId = 2;

                                        if (SchOp.GroupId == 202 || SchOp.GroupId == 203 || SchOp.GroupId == 204)
                                            GenCall.ClientId = 3;

                                        GeneratedCallRepository.Save(GenCall);
                                    }
                                    log.EndDate = DateTime.Now;
                                    ScheduleLogRepository.Save(log);

                                    ScheduleLog NewLog = new ScheduleLog();
                                    NewLog.ScheduleId = schedule.Id;

                                    DateTime dt = log.StartDate.Value.AddMinutes((double)schedule.OccursEvery);
                                    TimeSpan span = new TimeSpan();
                                    
                                    if (dt != null)
                                        span = DateTime.Now - dt;

                                    if (span.TotalSeconds > 0)
                                        NewLog.StartDate = DateTime.Now;
                                    else
                                        NewLog.StartDate = log.StartDate.Value.AddMinutes((double)schedule.OccursEvery);

                                    NewLog.EndDate = null;
                                    NewLog.Frequency = null;
                                    ScheduleLogRepository.Save(NewLog);
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

        private static void WriteToEventLogEx(string message)
        {
            string cs = "VanCGSche";
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
