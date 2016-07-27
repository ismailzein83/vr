using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public abstract class QueueActivator : IDisposable
    {
        static ConcurrentDictionary<int, int> s_nbOfMaxConcurrentActivators = new ConcurrentDictionary<int, int>();
        public virtual int NbOfMaxConcurrentActivators
        {
            get
            {
                int nbOfConcurrentActivators;
                if(!s_nbOfMaxConcurrentActivators.TryGetValue(this.ConfigId, out nbOfConcurrentActivators))
                {
                    if (!int.TryParse(ConfigurationManager.AppSettings[String.Format("Queue_NbOfMaxConcurrentActivators_{0}", this.ConfigId)], out nbOfConcurrentActivators))
                        nbOfConcurrentActivators = 5;
                    s_nbOfMaxConcurrentActivators.TryAdd(this.ConfigId, nbOfConcurrentActivators);
                }
                return nbOfConcurrentActivators;
            }
        }

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
