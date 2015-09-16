using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

    public class Logger: ExceptionLogger
    {
        List<LogHandler> _handlers;
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
        
        void PrivateWriteEntry(LogEntryType entryType, string messageFormat, params object[] args)
        {
            if (entryType <= _maxLogLevel)
            {
                if (_handlers != null && _handlers.Count > 0)
                {
                    StackFrame frame = new StackFrame(2);
                    var method = frame.GetMethod();
                    var type = method.DeclaringType;
                    foreach (var handler in _handlers)
                    {
                        if (handler.LogLevel == null || entryType <= handler.LogLevel)
                        {
                            try
                            {
                                handler.WriteEntry(entryType, String.Format(messageFormat, args), type.Assembly.GetName().Name, type.FullName, method.Name);
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

        protected override void OnWriteException(Exception e)
        {
            if (_exceptionLoggers != null && _exceptionLoggers.Count > 0)
            {
                foreach (var exceptionLogger in _exceptionLoggers)
                {
                    try
                    {
                        exceptionLogger.WriteException(e);
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
            PrivateWriteEntry(entryType, messageFormat, args);
        }

        public void WriteVerbose(string messageFormat, params object[] args)
        {
            PrivateWriteEntry(LogEntryType.Verbose, messageFormat, args);
        }

        public void WriteInformation(string messageFormat, params object[] args)
        {
            PrivateWriteEntry(LogEntryType.Information, messageFormat, args);
        }

        public void WriteWarning(string messageFormat, params object[] args)
        {
            PrivateWriteEntry(LogEntryType.Warning, messageFormat, args);
        }

        public void WriteError(string messageFormat, params object[] args)
        {
            PrivateWriteEntry(LogEntryType.Error, messageFormat, args);
        }

        #endregion

    }
}
