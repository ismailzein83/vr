using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskType
    {
        public int BPTaskTypeId { get; set; }

        public BPTaskTypeSettings Settings { get; set; }
    }
}
