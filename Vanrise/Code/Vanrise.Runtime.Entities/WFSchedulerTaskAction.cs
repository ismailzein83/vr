using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class WFSchedulerTaskAction : SchedulerTaskAction
    {
        public int BPDefinitionID { get; set; }

        public override void Execute()
        {
            Console.WriteLine("WFSchedulerTaskAction is running...");
        }
    }
}
