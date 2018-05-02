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

        public string StartedTimeFormatted { get; set; }

        public string LastHeartBeatTimeFormatted { get; set; }

        public Double NbOfSecondsHeartBeatReceived { get; set; }
    }

}
