using log4net;
using System;

namespace Vanrise.CommonLibrary
{
    public static class FileLogger
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FileLogger));

        public static void Write(string title, Exception err)
        {
            WriteError(title, err.Message);      
        }

        public static void WriteInfo(string title, string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Info(title + ": " + message );
        }

        public static void WriteError(string title, string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Error(title + ": " + message);
        }
        
    }
}
