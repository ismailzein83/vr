using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Business
{
    public enum IntervalType { Minute = 0, Hour = 1 };

    public class IntervalTimeSchedulerTaskTrigger : TimeSchedulerTaskTrigger
    {
        public int Interval { get; set; }

        public IntervalType IntervalType { get; set; }

        public override DateTime CalculateNextTimeToRun()
        {
            return base.CalculateNextTimeToRun();
        }
    }

    
}
