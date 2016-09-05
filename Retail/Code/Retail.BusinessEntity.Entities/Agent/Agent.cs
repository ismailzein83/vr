using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Agent
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public AgentSetting Settings { get; set; }
        public string SourceId { get; set; }
    }

    
}
