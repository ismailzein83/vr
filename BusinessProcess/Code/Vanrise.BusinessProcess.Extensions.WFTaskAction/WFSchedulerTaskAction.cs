using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Business;
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

            BPInstanceManager bpInstanceManager = new BPInstanceManager();
            BaseProcessInputArgument inputArguments = wfTaskActionArgument.ProcessInputArguments;
            inputArguments.UserId = task.OwnerId;

            bpInstanceManager.CreateNewProcess(new BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = inputArguments
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
