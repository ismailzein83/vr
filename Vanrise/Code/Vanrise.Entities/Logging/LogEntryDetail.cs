using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Entities
{
    public class LogEntryDetail
    {
        public Vanrise.Entities.LogEntry Entity { get; set; }

        public string MachineName { get; set; }

        public string ApplicationName { get; set; }

        public string AssemblyName { get; set; }

        public string TypeName { get; set; }

        public string MethodName { get; set; }

        public string EntryTypeName { get; set; }

        public string EventTypeName { get; set; }
    }
}
