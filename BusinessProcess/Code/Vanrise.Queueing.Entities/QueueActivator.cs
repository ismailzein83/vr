﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public abstract class QueueActivator : IDisposable
    {
        public int ConfigId { get; set; }

        /// <summary>
        /// to be removed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="outputItems"></param>
        public virtual void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems){}

        public virtual void ProcessItem(IQueueActivatorExecutionContext context)
        {

        }

        public void Dispose()
        {
            OnDisposed();
        }

        public virtual void OnDisposed() { }
    }

    public interface IQueueActivatorExecutionContext
    {
        PersistentQueueItem ItemToProcess { get; }

        ItemsToEnqueue OutputItems { get; }

        QueueExecutionFlowStage CurrentStage { get; }

        QueueExecutionFlowStage GetStage(string stageName);

        QueueInstance Queue { get; }
    }
}
