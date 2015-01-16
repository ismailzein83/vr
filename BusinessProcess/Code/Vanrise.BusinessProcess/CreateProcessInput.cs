using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public class CreateProcessInput
    {
        public object InputArguments { get; set; }
        public Guid? ParentProcessID { get; set; }
    }
}
