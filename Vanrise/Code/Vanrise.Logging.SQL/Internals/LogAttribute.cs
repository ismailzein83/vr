using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Logging.SQL
{
    public enum LogAttributeType { MachineName = 1, ApplicationName = 2, AssemblyName = 3, TypeName = 4, MethodName = 5, EventType = 6 }

    internal class LogAttributesByType : ConcurrentDictionary<LogAttributeType, LogAttributesByDescription>
    {
    }

    internal class LogAttributesByDescription : ConcurrentDictionary<string, int>
    {
    }

    public class LogAttribute
    {
        public int LogAttributeID { get; set; }
        public int AttributeType { get; set; }
        public string Description { get; set; }
    }
}