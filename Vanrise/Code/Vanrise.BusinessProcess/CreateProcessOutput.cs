using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public class CreateProcessOutput
    {
        public bool IsCreated { get; set; }
        public Guid ProcessInstanceId { get; set; }
    }
}
