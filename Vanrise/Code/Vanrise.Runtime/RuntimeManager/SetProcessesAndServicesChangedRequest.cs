using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class SetProcessesAndServicesChangedRequest : InterRuntimeServiceRequest<Object>
    {
        public override object Execute()
        {
            RunningProcessManager.SetRunningProcessChanged();
            RuntimeServiceInstanceManager.SetServicesChanged();
            return null;
        }
    }
}
