using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Triggers.TimeTaskTrigger.Arguments
{
    public class TimeTaskTriggerArgument : BaseTaskTriggerArgument
    {
        public string TimerTriggerTypeFQTN { get; set; }

        public bool IgnoreSkippedIntervals { get; set; }
    }
}
