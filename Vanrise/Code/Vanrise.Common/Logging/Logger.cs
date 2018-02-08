using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    

    public class Logger: ExceptionLogger
    {
        List<LogHandler> _handlers;
        public List<LogHandler> LogHandlers
        {
            get
            {
                return _handlers;
            }
        }


        List<ExceptionLogger> _exceptionLoggers;
        LogEntryType _maxLogLevel;
        public Logger(LoggingConfiguration.LoggingConfig config)
        {
            LoadHandlers(config);
        }

        private void LoadHandlers(LoggingConfiguration.LoggingConfig config)
        {
            if (config != null)
            {
                var exceptionLoggers = new List<ExceptionLogger>();
                if (config.ExceptionLoggers != null)
                {
                    foreach (LoggingConfiguration.ExceptionLogger exceptionLoggerConfig in config.ExceptionLoggers)
                    {
                        Type exceptionLoggerType = Type.GetType(exceptionLoggerConfig.Type);
                        ExceptionLogger exceptionLogger = Activator.CreateInstance(exceptionLoggerType) as ExceptionLogger;
                        if (exceptionLogger == null)
                            throw new Exception(String.Format("invalid Exception Logger type {0}", exceptionLoggerConfig.Type));
                        if (exceptionLoggerConfig.Parameters != null)
                        {
                            foreach (LoggingConfiguration.ConfigurationParameter prm in exceptionLoggerConfig.Parameters)
                            {
                                PropertyInfo prop = exceptionLoggerType.GetProperty(prm.Name);
                                if (prop == null)
                                    throw new Exception(String.Format("Property {0} not found on Logger {1}", prm.Name, exceptionLoggerConfig.Type));
                                prop.SetValue(exceptionLogger, prm.Value);
                            }
                        }
                        exceptionLoggers.Add(exceptionLogger);
                    }
                }
                _exceptionLoggers = exceptionLoggers;

                var handlers = new List<LogHandler>();
                if (config.Loggers != null)
                {
                    foreach (LoggingConfiguration.Logger loggerConfig in config.Loggers)
                    {
                        Type handlerType = Type.GetType(loggerConfig.Type);
                        LogHandler handler = Activator.CreateInstance(handlerType) as LogHandler;
                        if (handler == null)
                            throw new Exception(String.Format("invalid logger type {0}", loggerConfig.Type));
                        if (loggerConfig.Parameters != null)
                        {
                            foreach (LoggingConfiguration.ConfigurationParameter prm in loggerConfig.Parameters)
                            {
                                PropertyInfo prop = handlerType.GetProperty(prm.Name);
                                if (prop == null)
                                    throw new Exception(String.Format("Property {0} not found on Logger {1}", prm.Name, loggerConfig.Type));
                                prop.SetValue(handler, prm.Value);
                            }
                        }
                        handler.LogLevel = loggerConfig.LogLevel;
                        handlers.Add(handler);
                    }
                }
                _handlers = handlers;
                _maxLogLevel = config.MaxLogLevel;
            }
        }
        

        #region Private Methods

        void PrivateWriteEntry(string eventType, int? viewRequiredPermissionSetId, string exceptionDetail, LogEntryType entryType, string messageFormat, params object[] args)
        {
            if (entryType <= _maxLogLevel)
            {
                if (_handlers != null && _handlers.Count > 0)
                {
                    if (eventType == null)
                    {
                        eventType = "Technical";
                        viewRequiredPermissionSetId = SecBusinessManagerFactory.GetManager<ISecurityRequiredPermissionSetManager>().GetRequiredPermissionSetId(LoggerFactory.LOGGING_REQUIREDPERMISSIONSET_MODULENAME, "VRCommon_System_Log:View Technical Logs");
                    }
                    StackFrame frame = new StackFrame(2);
                    var method = frame.GetMethod();
                    var type = method.DeclaringType;
                    foreach (var handler in _handlers)
                    {
                        if (handler.LogLevel == null || entryType <= handler.LogLevel)
                        {
                            try
                            {
                                string message = args != null && args.Length > 0 ? String.Format(messageFormat, args) : messageFormat;
                                handler.WriteEntry(eventType, viewRequiredPermissionSetId, exceptionDetail, entryType, message, type.Assembly.GetName().Name, type.FullName, method.Name);
                            }
                            catch(Exception ex)
                            {
                                HandleLoggerException.WriteException(ex);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnWriteException(string eventType, int? viewRequiredPermissionSetId, Exception e)
        {
            if (_exceptionLoggers != null && _exceptionLoggers.Count > 0)
            {
                foreach (var exceptionLogger in _exceptionLoggers)
                {
                    try
                    {
                        exceptionLogger.WriteException(eventType, viewRequiredPermissionSetId, e);
                    }
                    catch(Exception ex)
                    {
                        LogExceptionIfNoHandler(ex);
                    }
                }
            }
        }

        void LogExceptionIfNoHandler(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        #endregion


        #region Public Methods

        public void WriteEntry(LogEntryType entryType, string messageFormat, params object[] args)
        {
            WriteEntry(null, null, entryType, messageFormat, args);
        }

        public void WriteEntry(string eventType, int? viewRequiredPermissionSetId, LogEntryType entryType, string messageFormat, params object[] args)
        {
            PrivateWriteEntry(eventType, viewRequiredPermissionSetId, null, entryType, messageFormat, args);
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

        internal void WriteException(string eventType, int? viewRequiredPermissionSetId, Exception ex)
        {
            string message = Utilities.GetExceptionBusinessMessage(ex);
            PrivateWriteEntry(eventType, viewRequiredPermissionSetId, ex.ToString(), LogEntryType.Error, message);
        }

        #endregion

    }
}
