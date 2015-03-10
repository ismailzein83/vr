using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.LoggingConfiguration
{
    public class LoggingConfig : ConfigurationSection
    {
        public static LoggingConfig GetConfig()
        {
            return System.Configuration.ConfigurationManager.GetSection("vanriseLogging") as LoggingConfig;
        }

        [System.Configuration.ConfigurationProperty("loggers")]
        [ConfigurationCollection(typeof(LoggerCollection), AddItemName = "logger")]
        public LoggerCollection Loggers
        {
            get
            {
                return this["loggers"] as LoggerCollection;
            }
        }

        [ConfigurationProperty("maxLogLevel", IsRequired = true)]
        public LogEntryType MaxLogLevel
        {
            get
            {
                return (LogEntryType)this["maxLogLevel"];
            }
        }

        [System.Configuration.ConfigurationProperty("exceptionLoggers")]
        [ConfigurationCollection(typeof(ExceptionLoggerCollection), AddItemName = "exceptionLogger")]
        public ExceptionLoggerCollection ExceptionLoggers
        {
            get
            {
                return this["exceptionLoggers"] as ExceptionLoggerCollection;
            }
        }
    }
}
