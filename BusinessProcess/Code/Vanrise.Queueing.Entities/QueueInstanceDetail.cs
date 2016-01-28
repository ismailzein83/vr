using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueInstanceDetail
    {
        public QueueInstance Entity { get; set; }

        public string ExecutionFlowName { get; set; }

        public string ItemTypeName { get; set; }

    }
}
