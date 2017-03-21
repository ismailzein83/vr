using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public static class LoggerFactory
    {
        public const string LOGGING_REQUIREDPERMISSIONSET_MODULENAME = "VR_GeneralLoggingReqPerm";
        static Logger s_logger;
        
        public static Logger GetLogger()
        {
            InitializeLogger();
            return s_logger;
        }

        public static ExceptionLogger GetExceptionLogger()
        {
            InitializeLogger();
            return s_logger;
        }

        private static void InitializeLogger()
        {
            if (s_logger == null)
            {
                lock (typeof(LoggerFactory))
                {
                    if (s_logger == null)
                    {
                        LoggingConfiguration.LoggingConfig config = LoggingConfiguration.LoggingConfig.GetConfig();
                        s_logger = new Logger(config);
                    }
                }
            }
        }
    }

    public class ConsoleLogger : LogHandler
    {
        protected internal override void WriteEntry(string eventType, int? viewRequiredPermissionSetId, string exceptionDetail, LogEntryType entryType, string message, string callingModule, string callingType, string callingMethod)
        {
            Console.WriteLine("{0} - {1}: {2}", entryType, DateTime.Now, message);
        }
    }

    public class ConsoleExceptionLogger : ExceptionLogger
    {
        protected override void OnWriteException(string eventType, int? viewRequiredPermissionSetId, Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public class DefaultExceptionLogger : ExceptionLogger
    {
        protected override void OnWriteException(string eventType, int? viewRequiredPermissionSetId, Exception ex)
        {
            LoggerFactory.GetLogger().WriteException(eventType, viewRequiredPermissionSetId, ex);
        }
    }

}
