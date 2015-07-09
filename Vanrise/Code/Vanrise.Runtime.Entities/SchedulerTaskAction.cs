using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public abstract class SchedulerTaskAction
    {
        public Dictionary<string, object> RawExpressions { get; set; }

        public abstract void Execute(SchedulerTask task, Dictionary<string, object> evaluatedExpressions);
    }
}
