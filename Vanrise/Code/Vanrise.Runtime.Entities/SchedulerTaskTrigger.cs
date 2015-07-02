﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public abstract class SchedulerTaskTrigger
    {
        public abstract DateTime CalculateNextTimeToRun();

        public abstract Dictionary<string, string> EvaluateExpressions(SchedulerTask task);
    }
}
