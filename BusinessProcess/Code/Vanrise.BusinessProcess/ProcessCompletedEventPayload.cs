using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class ProcessCompletedEventPayload
    {
        public BPInstanceStatus ProcessStatus { get; set; }
        public string LastProcessMessage { get; set; }

        public string ErrorBusinessMessage { get; set; }

        public string ExceptionDetail { get; set; }

        public object ProcessOutput { get; set; }
    }
}
