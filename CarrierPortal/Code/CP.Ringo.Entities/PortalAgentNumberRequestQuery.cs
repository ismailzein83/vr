using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.Ringo.Entities
{
    public class PortalAgentNumberRequestQuery
    {
        public IEnumerable<long> AgentIds { get; set; }
        public List<int> Status { get; set; }
        public string Number { get; set; }
    }
}
