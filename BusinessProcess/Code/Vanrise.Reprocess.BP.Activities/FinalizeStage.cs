﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public class FinalizeStageInput
    {
        public IReprocessStageActivator StageActivator { get; set; }

        public string StageName { get; set; }

        public DateTime BatchStart { get; set; }

        public long CurrentProcessId { get; set; }
    }

    public class FinalizeStageOutput
    {
    }

    public sealed class FinalizeStage : DependentAsyncActivity<FinalizeStageInput, FinalizeStageOutput>
    {
        [RequiredArgument]
        public InArgument<IReprocessStageActivator> StageActivator { get; set; }

        [RequiredArgument]
        public InArgument<string> StageName { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> BatchStart { get; set; }

        protected override FinalizeStageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FinalizeStageInput()
            {
                StageActivator = this.StageActivator.Get(context),
                StageName = this.StageName.Get(context),
                BatchStart = this.BatchStart.Get(context),
                CurrentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID
            };
        }

        protected override FinalizeStageOutput DoWorkWithResult(FinalizeStageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var executionContext = new ReprocessStageActivatorFinalizingContext(inputArgument.StageName, inputArgument.CurrentProcessId, inputArgument.BatchStart,
                (logEntryType, message) =>
                {
                    handle.SharedInstanceData.WriteTrackingMessage(logEntryType, message, null);
                }, (previousActivityStatus_Internal, actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus_Internal, handle, actionToDo));

            inputArgument.StageActivator.FinalizeStage(executionContext);
            return new FinalizeStageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeStageOutput result)
        {

        }

        private class ReprocessStageActivatorFinalizingContext : IReprocessStageActivatorFinalizingContext
        {
            string _currentStageName;
            long _processInstanceId;
            DateTime _batchStart;
            Action<LogEntryType, string> _writeTrackingMessage;
            Action<AsyncActivityStatus, Action> _doWhilePreviousRunningAction;

            public ReprocessStageActivatorFinalizingContext(string currentStageName, long processInstanceId, DateTime batchStart, Action<LogEntryType, string> writeTrackingMessage,
                Action<AsyncActivityStatus, Action> doWhilePreviousRunningAction)
            {
                _currentStageName = currentStageName;
                _processInstanceId = processInstanceId;
                _batchStart = batchStart;
                _writeTrackingMessage = writeTrackingMessage;
                _doWhilePreviousRunningAction = doWhilePreviousRunningAction;
            }

            public long ProcessInstanceId
            {
                get { return _processInstanceId; }
            }

            public string CurrentStageName
            {
                get { return _currentStageName; }
            }

            public DateTime BatchStart
            {
                get { return _batchStart; }
            }

            public void WriteTrackingMessage(LogEntryType severity, string messageFormat)
            {
                _writeTrackingMessage(severity, messageFormat);
            }

            public void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo)
            {
                _doWhilePreviousRunningAction(previousActivityStatus, actionToDo);
            }
        }
    }
}