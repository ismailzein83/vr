using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinitionSummary
    {

        public Guid BPDefinitionID { get; set; }

        public int RunningProcessNumber { get; set; }

        public DateTime LastProcessCreatedTime { get; set; }
    }
}
