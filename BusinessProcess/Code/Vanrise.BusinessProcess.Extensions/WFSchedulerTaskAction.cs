using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Extensions
{
    public class WFSchedulerTaskAction : SchedulerTaskAction
    {
        public int BPDefinitionID { get; set; }

        public BaseProcessInputArgument BaseProcessInputArgument { get; set; }

        public override void Execute()
        {
            Console.WriteLine("WFSchedulerTaskAction is running...");
        }
    }
}
