using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPTaskInformation
    {
        public BPTaskAssignee AssignedTo { get; set; }

        public List<BPTaskAction> Actions { get; set; }
    }
}
