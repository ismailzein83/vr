using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.Entities
{
    public class AgentNumberRequestDetail
    {
        public AgentNumberRequest Entity { get; set; }
        public string AgentName { get; set; }
        public string StatusDescription { get; set; }
    }
}
