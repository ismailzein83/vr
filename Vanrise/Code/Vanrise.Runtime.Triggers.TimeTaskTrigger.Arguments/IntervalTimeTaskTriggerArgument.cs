using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public enum IntervalType { Minute = 0, Hour = 1 };

    public class IntervalTimeTaskTriggerArgument : TimeTaskTriggerArgument
    {
        public double Interval { get; set; }

        public IntervalType IntervalType { get; set; }
    }
}
