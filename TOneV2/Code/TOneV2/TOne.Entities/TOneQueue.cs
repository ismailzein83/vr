using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public class TOneQueue<T>
    {
        ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public virtual void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        T _lastItem;
        public virtual bool TryDequeue(Action<T> processItem)
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
    }
}
