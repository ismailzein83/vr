using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    public class MemoryQueue<T> : BaseQueue<T>
    {
        ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public override void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public override bool TryDequeue(Action<T> processItem)
        {
            T item;
            if (_queue.TryDequeue(out item))
            {
                try
                {
                    processItem(item);
                }
                catch
                {
                    _queue.Enqueue(item);//enqueue the item again if not processed
                    throw;
                }                
                return true;
            }
            else
                return false;
        }

        public int Count
        {
            get
            {
                return _queue.Count;
            }
        }
    }
}
