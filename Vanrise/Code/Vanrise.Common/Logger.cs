using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public enum LogEntryType
    {
        Error = 1,
        Warning = 2,
        Information = 4,
        Verbose = 8
    }

    public abstract class Logger
    {
        static Logger()
        {
            if (!Enum.TryParse(ConfigurationManager.AppSettings["LogLevel"], true, out s_LogLevel))
                s_LogLevel = LogEntryType.Information;
        }

        static LogEntryType s_LogLevel;

        public void WriteEntry(LogEntryType entryType, string messageFormat, params object[] args)
        {
            if (entryType <= s_LogLevel)
                OnWriteEntry(entryType, String.Format(messageFormat, args));
        }

        public void WriteVerbose(string messageFormat, params object[] args)
        {
            WriteEntry(LogEntryType.Verbose, messageFormat, args);
        }

        public void WriteInformation(string messageFormat, params object[] args)
        {
            WriteEntry(LogEntryType.Information, messageFormat, args);
        }

        public void WriteWarning(string messageFormat, params object[] args)
        {
            WriteEntry(LogEntryType.Warning, messageFormat, args);
        }

        public void WriteError(string messageFormat, params object[] args)
        {
            WriteEntry(LogEntryType.Error, messageFormat, args);
        }

        protected abstract void OnWriteEntry(LogEntryType entryType, string message);
    }
}
