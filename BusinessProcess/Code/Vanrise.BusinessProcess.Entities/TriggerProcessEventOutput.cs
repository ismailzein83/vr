using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public enum TriggerProcessEventResult {  Succeeded, ProcessInstanceNotExists}
    public class TriggerProcessEventOutput
    {
        public TriggerProcessEventResult Result { get; set; }
    }
}
