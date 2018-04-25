using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RuntimeNodeStateDetails
    {
        public Guid RuntimeNodeId { get; set; }

        public Guid InstanceId { get; set; }

        public string MachineName { get; set; }

        public int OSProcessId { get; set; }

        public string OSProcessName { get; set; }

        public string ServiceURL { get; set; }

        public DateTime StartedTime { get; set; }

        public DateTime LastHeartBeatTime { get; set; }

        public Double NbOfSecondsHeartBeatReceived { get; set; }
    }

}
