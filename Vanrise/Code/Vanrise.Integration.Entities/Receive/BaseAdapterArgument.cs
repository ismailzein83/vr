using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class BaseAdapterArgument
    {
        public virtual int MaxParallelRuntimeInstances
        {
            get
            {
                return 1;
            }
        }     
    }
}
