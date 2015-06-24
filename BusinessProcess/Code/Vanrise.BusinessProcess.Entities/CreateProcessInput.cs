using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public class CreateProcessInput
    {
        public long? ParentProcessID { get; set; }

        public BaseProcessInputArgument InputArguments { get; set; }
    }
}
