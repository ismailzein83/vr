using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class ServiceInstance
    {
        public Guid ServiceInstanceId { get; set; }

        public Guid ServiceType { get; set; }

        public int ProcessId { get; set; }

        public ServiceInstanceInfo InstanceInfo { get; set; }
    }

    public class ServiceInstanceInfo
    {
    }
}
