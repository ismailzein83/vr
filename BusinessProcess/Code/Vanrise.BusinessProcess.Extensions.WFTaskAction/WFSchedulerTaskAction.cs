using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction
{
    public class WFSchedulerTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            Console.WriteLine("WFSchedulerTaskAction started...");

            WFTaskActionArgument wfTaskActionArgument = (WFTaskActionArgument)taskActionArgument;

            if (evaluatedExpressions != null)
                wfTaskActionArgument.ProcessInputArguments.MapExpressionValues(evaluatedExpressions);

            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = wfTaskActionArgument.ProcessInputArguments
            });

            Console.WriteLine("WFSchedulerTaskAction finished...");

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
