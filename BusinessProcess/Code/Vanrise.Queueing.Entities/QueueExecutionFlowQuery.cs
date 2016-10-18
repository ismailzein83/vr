using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlowQuery
    {
        public List<Guid> DefinitionId { get; set; }

        public string Name { get; set; }
    }
}
