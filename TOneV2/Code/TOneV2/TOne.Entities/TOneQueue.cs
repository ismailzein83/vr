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
        public virtual bool TryDequeue(out T result)
        {
            if (_lastItem == null || _lastItem.Equals(default(T)))
                _queue.TryDequeue(out _lastItem);
            result = _lastItem;
            return _lastItem != null && !_lastItem.Equals(default(T));
        }

        public virtual void GoNext()
        {
            _lastItem = default(T);
        }
    }
}
