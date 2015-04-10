using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;
using System.Data.Linq;

namespace CallGeneratorLibrary.Repositories
{
    public class AuditRepository
    {
        public static List<ActionLog> GetActionLogs()
        {
            List<ActionLog> logs = new List<ActionLog>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    logs = context.ActionLogs.ToList<ActionLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return logs;
        }

        public static List<ActionLogFeed> GetActionLogs(int userId, string ObjectType)
        {
            List<ActionLogFeed> logs = new List<ActionLogFeed>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    logs = context.GetActionLogs1(ObjectType, userId).GetResult<ActionLogFeed>().ToList<ActionLogFeed>();
                    //DataLoadOptions options = new DataLoadOptions();
                    //options.LoadWith<ActionLog>(c => c.User);
                    //context.LoadOptions = options;

                    //logs = context.ActionLogs.Where(l => l.LogDate.Value.Month == DateTime.Now.Month && l.UserId == userId).ToList<ActionLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return logs;
        }

        public static ActionLog Load(int logId)
        {
            ActionLog log = new ActionLog();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.ActionLogs.Where(l => l.Id == logId).FirstOrDefault<ActionLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static void Save(ActionLog action)
        {
            //action.Username = Current.User.Username;
            action.LogDate = DateTime.Now;
            action.IPAddress = ActionClass.GetIPAddress();
            action.RemoteAddress = ActionClass.GetRemoteAddress();
            action.ComputerName = ActionClass.GetComputerName();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ActionLogs.InsertOnSubmit(action);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
        }

        public static void Save2(ActionLog action)
        {
            action.LogDate = DateTime.Now;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.ActionLogs.InsertOnSubmit(action);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
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
