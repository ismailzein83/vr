using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class CreateBPTaskInput
    {
        public long ProcessInstanceId { get; set; }

        public string TaskName { get; set; }

        public string Title { get; set; }

        public BPTaskData TaskData { get; set; }

        public List<int> AssignedUserIds { get; set; }

        public string AssignedUserIdsDescription { get; set; }
    }
}
