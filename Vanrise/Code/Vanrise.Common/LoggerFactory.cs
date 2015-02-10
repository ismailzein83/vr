using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class LoggerFactory
    {
        public static ILogger GetLogger()
        {
            return new Logger();
        }
    }

    public class Logger : ILogger
    {
        public void LogException(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

}
