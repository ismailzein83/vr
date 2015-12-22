using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public interface ISchedulerTaskCheckProgressContext
    {
        Object ExecutionInfo { get; }
    }
}
