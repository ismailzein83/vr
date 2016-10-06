using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskActionType
    {
        public Guid ActionTypeId { get; set; }

        public string Name { get; set; }

        public ActionTypeInfo Info { get; set; }
    }

    public class ActionTypeInfo
    {
        public string URL { get; set; }

        public bool SystemType { get; set; }

        public string FQTN { get; set; }

        public string Editor { get; set; }

        public string RequiredPermissions { get; set; }

    }
}
