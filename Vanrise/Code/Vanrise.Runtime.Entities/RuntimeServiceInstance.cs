using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeServiceInstance
    {
        public Guid ServiceInstanceId { get; set; }

        public int ServiceTypeId { get; set; }

        public int ProcessId { get; set; }

        public ServiceInstanceInfo InstanceInfo { get; set; }
    }

    public class ServiceInstanceInfo
    {
    }
}
