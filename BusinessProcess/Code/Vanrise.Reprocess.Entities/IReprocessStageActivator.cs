﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace Vanrise.Reprocess.Entities
{
    public interface IReprocessStageActivator
    {
        BaseQueue<IReprocessBatch> GetQueue();

        List<string> GetOutputStages(List<string> stageNames);

        void ExecuteStage(IReprocessStageActivatorExecutionContext context);

        void FinalizeStage(IReprocessStageActivatorFinalizingContext context);

        List<BatchRecord> GetStageBatchRecords(IReprocessStageActivatorPreparingContext context);
    }

    public interface IReprocessStageActivatorExecutionContext
    {
        BaseQueue<IReprocessBatch> InputQueue { get; }

        void DoWhilePreviousRunning(Action actionToDo);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);

        void WriteTrackingMessage(LogEntryType severity, string messageFormat);

        bool ShouldStop();

        void EnqueueBatch(string stageName, IReprocessBatch batch);

        DateTime From { get; }

        DateTime To { get; }

        List<string> StageNames { get; }

        string CurrentStageName { get; }

        long ProcessInstanceId { get; }
    }

    public interface IReprocessStageActivatorFinalizingContext 
    {
        long ProcessInstanceId { get; }

        string CurrentStageName { get; }

        BatchRecord BatchRecord { get; }

        void WriteTrackingMessage(LogEntryType severity, string messageFormat);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);
    }

    public interface IReprocessStageActivatorPreparingContext
    {
        long ProcessInstanceId { get; }

        string CurrentStageName { get; }
    }

    public abstract class BatchRecord
    {
        public abstract string GetBatchTitle();
    }
}
