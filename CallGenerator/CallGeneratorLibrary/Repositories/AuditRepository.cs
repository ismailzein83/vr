﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;
using CallGeneratorLibrary;

namespace CallGeneratorLibrary.Repositories
{
    public class AuditRepository
    {
        public static List<NewActionLog> GetNewActionLogs()
        {
            List<NewActionLog> logs = new List<NewActionLog>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    logs = context.NewActionLogs.ToList<NewActionLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return logs;
        }

        public static NewActionLog Load(int logId)
        {
            NewActionLog log = new NewActionLog();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.NewActionLogs.Where(l => l.Id == logId).FirstOrDefault<NewActionLog>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static void Save(NewActionLog action)
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
                    context.NewActionLogs.InsertOnSubmit(action);
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
            string cs = "VanCallGen";
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
