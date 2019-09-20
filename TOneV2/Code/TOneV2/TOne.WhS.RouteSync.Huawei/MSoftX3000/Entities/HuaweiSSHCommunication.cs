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

        public HuaweiSSLSettings SSLSettings { get; set; }
    }

    public class HuaweiSSLSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string InterfaceIP { get; set; }
    }
}