﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace Vanrise.Reprocess.Entities
{
    public interface IReprocessStageActivator
    {
        BaseQueue<IReprocessBatch> GetQueue();

        List<string> GetOutputStages();

        void ExecuteStage(IReprocessStageActivatorExecutionContext context);

        void FinalizeStage(IReprocessStageActivatorFinalizingContext context);
    }

    public interface IReprocessStageActivatorExecutionContext
    {
        BaseQueue<IReprocessBatch> InputQueue { get; }

        void DoWhilePreviousRunning(Action actionToDo);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);

        bool ShouldStop();
    }

    public interface IReprocessStageActivatorFinalizingContext
    {

    }
}
