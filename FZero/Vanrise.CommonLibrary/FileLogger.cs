using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

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


        public static void WriteDebug(string title, string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Debug(title + ": " + message);
        }

        public static void WriteWarn(string title, string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Warn(title + ": " + message);
        }

        public static void WriteError(string title, string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Error(title + ": " + message);
        }


        public static void WriteFatal(string title, string message)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Fatal(title + ": " + message);
        }

        
    }
}
