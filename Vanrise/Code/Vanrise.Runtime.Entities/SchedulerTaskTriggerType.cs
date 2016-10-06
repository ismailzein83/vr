using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskTriggerType
    {
        public Guid TriggerTypeId { get; set; }

        public string Name { get; set; }

        public TriggerTypeInfo Info { get; set; }
    }

    public class TriggerTypeInfo
    {
        public string URL { get; set; }

        public string FQTN { get; set; }

        public string Editor { get; set; }
    }

}
