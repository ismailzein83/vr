using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class SOMProcessInstanceLog
    {
        public Guid RequestId { get; set; }
        public long ProcessInstanceId { get; set; }
    }
}
