using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.Entities
{
    public class AgentNumberRequestQuery
    {
        public IEnumerable<long> AgentIds { get; set; }
        public Status? Status { get; set; }

        public List<NumberStatus> NumberStatuses { get; set; }
    }
}
