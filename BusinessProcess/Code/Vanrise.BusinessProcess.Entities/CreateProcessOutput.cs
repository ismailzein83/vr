using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public enum CreateProcessResult {  Succeeded = 0, Failed = 1 }
    public class CreateProcessOutput
    {
        public CreateProcessResult Result { get; set; }
        public long ProcessInstanceId { get; set; }
    }
}
