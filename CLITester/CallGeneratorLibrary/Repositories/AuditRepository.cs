using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;

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

        private static void WriteToEventLogEx(string message)
        {
            string cs = "Call Generator Lib Excep";
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
