using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Business
{
    public class DailyTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public List<string> ScheduledHours { get; set; }

        public override DateTime CalculateNextTimeToRun()
        {
            return base.CalculateNextTimeToRun();
        }
    }
}
