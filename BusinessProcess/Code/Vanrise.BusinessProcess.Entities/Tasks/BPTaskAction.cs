using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPTaskAction
    {
        public abstract string GetResult(IBPTaskActionContext context);
    }
}
