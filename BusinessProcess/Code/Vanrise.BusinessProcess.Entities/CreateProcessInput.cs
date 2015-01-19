using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class CreateProcessInput
    {
        public string ProcessName { get; set; }

        public long? ParentProcessID { get; set; }

        public object InputArguments { get; set; }
    }
}
