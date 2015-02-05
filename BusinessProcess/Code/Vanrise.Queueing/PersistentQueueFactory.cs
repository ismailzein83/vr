using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    public static class PersistentQueueFactory
    {
        public static PersistentQueue<T> GetQueue<T>(string queueName) where T : PersistentQueueItem
        {
            return new PersistentQueue<T>(queueName);
        }
    }
}
