using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class TestActivator : QueueActivator
    {
        public string DummyProp { get; set; }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();
        }

        public override void OnDisposed()
        {
            throw new NotImplementedException();
        }
    }
}
