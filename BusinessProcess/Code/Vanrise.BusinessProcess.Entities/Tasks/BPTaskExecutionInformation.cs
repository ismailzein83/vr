using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskExecutionInformation
    {
        public BPTaskAction TakenAction { get; set; }

        public List<BPTaskComment> Comments { get; set; }
    }
}
