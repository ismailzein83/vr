using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing
{
    public abstract class BaseQueue<T>
    { 
        public abstract void Enqueue(T item);

        public abstract bool TryDequeue(Action<T> processItem);
    }
}
