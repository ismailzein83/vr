using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class StageQueue
    {
        public IPersistentQueue Queue { get; set; }
    }

    /// <summary>
    /// key is StageName
    /// </summary>
    public class QueuesByStages : Dictionary<string, StageQueue>
    {
        public void Add(string stageName, IPersistentQueue queue)
        {
            this.Add(stageName, new StageQueue
                {
                    Queue = queue
                });
        }
    }
}
