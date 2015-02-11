using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class LoggerFactory
    {
        public static Logger GetLogger()
        {
            return new ConsoleLogger();
        }

        public static ExceptionLogger GetExceptionLogger()
        {
            return new ConsoleExceptionLogger();
        }
    }

    public class ConsoleLogger : Logger
    {
        protected override void OnWriteEntry(LogEntryType entryType, string message)
        {
            Console.WriteLine("{0} - {1}: {2}", entryType, DateTime.Now, message);
        }
    }

    public class ConsoleExceptionLogger : ExceptionLogger
    {
        protected override void OnWriteException(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

}
