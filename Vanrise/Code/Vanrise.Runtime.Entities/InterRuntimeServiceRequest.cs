using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public abstract class InterRuntimeServiceRequest
    {
        public abstract dynamic CallExecute();
    }

    public abstract class InterRuntimeServiceRequest<T> : InterRuntimeServiceRequest
    {
        public sealed override dynamic CallExecute()
        {
            return Execute();
        }

        public abstract T Execute();
    }
}
