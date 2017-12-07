using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.BP.Activities
{
    public class ThrowTimeoutException : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            throw new Vanrise.Entities.VRBusinessException("Task timed out");
        }
    }
}
