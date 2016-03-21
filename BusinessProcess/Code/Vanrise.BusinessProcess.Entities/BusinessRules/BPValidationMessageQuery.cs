using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPValidationMessageQuery
    {
        public long ProcessInstanceId { get; set; }
        public List<ActionSeverity> Severities { get; set; }
    }
}
