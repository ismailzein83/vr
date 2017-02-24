using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Logging.SQL
{
    public class SQLLogger : LogHandler
    {        
         Timer _timer;
         ConcurrentQueue<LogEntry> _qLogEntries = new ConcurrentQueue<LogEntry>();
         string _machineName;
         string _applicationName;

         string _connectionStringKey;

        public string ConnectionStringKey
         {
             set
             {
                 _connectionStringKey = value;
             }
         }

        public SQLDataManager DataManager
        {
            get
            {
                if (String.IsNullOrEmpty(_connectionStringKey))
                    throw new Exception("Connection String is not provided for the SQLLogger. ConnectionStringKey should be set in the parameters section");
                return new SQLDataManager(_connectionStringKey);
            }
        }

        public SQLLogger()
        {
            _machineName = Environment.MachineName;
            _applicationName = Process.GetCurrentProcess().ProcessName;
            _timer = new Timer(1000);
            _timer.Elapsed += s_timer_Elapsed;
            _timer.Start();
        }

         bool s_isRunning;
         void s_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                if (s_isRunning)
                    return;
                s_isRunning = true;
            }

            try
            {
                if (_qLogEntries.Count > 0)
                {
                    LogEntry logEntry;
                    List<LogEntry> logEntries = new List<LogEntry>();
                    while (_qLogEntries.TryDequeue(out logEntry))
                    {
                        logEntries.Add(logEntry);
                    }
                    if (logEntries.Count >= 0)
                    {                        
                        this.DataManager.WriteEntries(_machineName, _applicationName, logEntries);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.HandleLoggerException.WriteException(ex);
            }

            lock (this)
            {
                s_isRunning = false;
            }
        }

         protected override void WriteEntry(string eventType, string exceptionDetail, LogEntryType entryType, string message, string callingModule, string callingType, string callingMethod)
        {
            _qLogEntries.Enqueue(new LogEntry
            {
                AssemblyName = callingModule,
                TypeName = callingType,
                MethodName = callingMethod,
                EntryType = entryType,
                EventType = eventType,
                Message = message,
                ExceptionDetail = exceptionDetail,
                EventTime = DateTime.Now
            });
        }
    }
}
