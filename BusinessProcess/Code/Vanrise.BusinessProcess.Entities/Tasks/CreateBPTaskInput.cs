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

        public int TypeId { get; set; }

        public string Title { get; set; }

        public BPTaskInformation TaskInformation { get; set; }
    }
}
