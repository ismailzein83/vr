using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class HuaweiSSHCommunication
    {
        public bool IsActive { get; set; }

        public SSHCommunicatorSettings SSHCommunicatorSettings { get; set; }
    }
}
