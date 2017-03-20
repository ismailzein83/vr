using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskDetail
    {
        public SchedulerTask Entity { get; set; }

        public bool AllowEdit { get; set; }

        public bool AllowRun { get; set; }
    }
}
