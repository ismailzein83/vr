using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public abstract class SchedulerTaskTrigger
    {

        public abstract bool CheckIfTimeToRun();

        public abstract Dictionary<string, string> EvaluateExpressions(Dictionary<string, string> rawExpressions);
    }
}
