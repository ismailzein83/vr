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
            if (_queue.TryPeek(out item))
            {
                processItem(item);
                _queue.TryDequeue(out item);
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
