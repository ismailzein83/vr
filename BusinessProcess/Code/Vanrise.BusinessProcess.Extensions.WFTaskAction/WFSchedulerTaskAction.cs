﻿using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Runtime.Entities;
using Vanrise.Common;

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


            BPDefinitionManager bpDefinitionManager = new BPDefinitionManager();
            BPDefinition bpDefinition = bpDefinitionManager.GetDefinition(inputArguments.ProcessName);
            bpDefinition.ThrowIfNull("bpDefinition", inputArguments.ProcessName);
            bpDefinition.ThrowIfNull("bpDefinition.Configuration", inputArguments.ProcessName);
            bpDefinition.ThrowIfNull("bpDefinition.Configuration.ExtendedSettings", inputArguments.ProcessName);

            BPDefinitionShouldCreateScheduledInstanceContext context = new BPDefinitionShouldCreateScheduledInstanceContext() { BaseProcessInputArgument = inputArguments };

            if (!bpDefinition.Configuration.ExtendedSettings.ShouldCreateScheduledInstance(context))
            {
                LoggerFactory.GetLogger().WriteInformation("The scheduled task '{0}' didn't need to start a process.", task.Name);
                return null;
            }

            var createProcessOutput = bpInstanceManager.CreateNewProcess(new BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = inputArguments
            }, false);

            Console.WriteLine("WFSchedulerTaskAction finished...");

            if (createProcessOutput.Result == CreateProcessResult.Succeeded)
            {
                return new SchedulerTaskExecuteOutput()
                {
                    Result = ExecuteOutputResult.WaitingEvent,
                    ExecutionInfo = new WFSchedulerTaskActionExecInfo { BPInstanceId = createProcessOutput.ProcessInstanceId }
                };
            }
            else
            {
                return new SchedulerTaskExecuteOutput
                {
                    Result = ExecuteOutputResult.Completed
                };
            }
        }

        public override SchedulerTaskCheckProgressOutput CheckProgress(ISchedulerTaskCheckProgressContext context, int ownerId)
        {
            WFSchedulerTaskActionExecInfo execInfo = context.ExecutionInfo as WFSchedulerTaskActionExecInfo;
            BPInstanceManager bpInstanceManager = new BPInstanceManager();
            var processInstance = bpInstanceManager.GetBPInstance(execInfo.BPInstanceId);
            if (processInstance != null && !BPInstanceStatusAttribute.GetAttribute(processInstance.Status).IsClosed)
                return new SchedulerTaskCheckProgressOutput { Result = ExecuteOutputResult.WaitingEvent };
            else
                return new SchedulerTaskCheckProgressOutput { Result = ExecuteOutputResult.Completed };
        }

        private class WFSchedulerTaskActionExecInfo
        {
            public long BPInstanceId { get; set; }
        }
    }
}
