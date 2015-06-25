using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class TimeSchedulerTaskTrigger : SchedulerTaskTrigger
    {
        public DateTime DateToRun { get; set; }

        public string TimeToRun { get; set; }

        public override bool CheckIfTimeToRun()
        {
            return true;
        }
    }
}
