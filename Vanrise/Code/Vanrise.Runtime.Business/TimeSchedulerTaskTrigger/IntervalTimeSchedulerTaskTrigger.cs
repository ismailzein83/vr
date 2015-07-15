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
        public double Interval { get; set; }

        public IntervalType IntervalType { get; set; }

        public override DateTime CalculateNextTimeToRun()
        {
            DateTime nextRunTime = DateTime.MinValue;

            switch(IntervalType)
            {
                case Business.IntervalType.Hour:
                    nextRunTime = DateTime.Now.AddHours(Interval);
                    break;
                case Business.IntervalType.Minute:
                    nextRunTime = DateTime.Now.AddMinutes(Interval);
                    break;
            }

            return nextRunTime;
        }
    }

    
}
