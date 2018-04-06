using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Entities
{
    public class LogEntryQuery
    {
        public List<int> MachineIds { get; set; }

        public List<int> ApplicationIds { get; set; }

        public List<int> TypeIds { get; set; }

        public List<int> MethodIds { get; set; }

        public List<int> AssemblyIds { get; set; }

        public List<int> EntryType { get; set; }

        public List<int> EventType { get; set; }

        public string Message { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int Top { get; set; }
    }
}
