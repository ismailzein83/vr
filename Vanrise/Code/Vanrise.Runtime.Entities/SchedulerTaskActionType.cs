using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskActionType
    {
        public int ActionTypeId { get; set; }

        public string Name { get; set; }

        public ActionTypeInfo Info { get; set; }
    }

    public class ActionTypeInfo
    {
        public string URL { get; set; }

        public bool SystemType { get; set; }
    }
}
