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

        public List<Guid> ExecutionFlowId { get; set; }

        public List<string> StageName { get; set; }

        public string Title { get; set;}

        public List<int> ItemTypeId { get; set; }
    }


}
