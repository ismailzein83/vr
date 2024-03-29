﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public interface IQueueExecutionFlowStageFilter
    {
        bool IsMatch(IQueueExecutionFlowStageFilterContext context);
    }

    public interface IQueueExecutionFlowStageFilterContext
    {
        QueueExecutionFlowStage Stage { get; }
    }
}
