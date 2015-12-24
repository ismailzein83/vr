using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class ValidationMessage
    {
        public long ProcessInstanceId { get; set; }

        public long? ParentProcessId { get; set; }

        public string TargetType { get; set; }

        public object TargetKey { get; set; }

        public ActionSeverity Severity { get; set; }

        public string Message { get; set; }
    }
}
