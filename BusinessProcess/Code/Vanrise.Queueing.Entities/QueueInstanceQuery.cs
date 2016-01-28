using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueInstanceQuery
    {

        public string Name { get; set; }

        public int? ExecutionFlowId { get; set; }

        public string StageName { get; set; }

        public string Title { get; set;}

        public int? ItemTypeId { get; set; }
    }


}
