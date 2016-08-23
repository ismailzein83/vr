using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPDefinitionDetail
    {
        public BPDefinition Entity { get; set; }

        public bool StartNewInstanceAccess { get; set; }

        public bool ScheduleTaskAccess { get; set; }
    }
}
