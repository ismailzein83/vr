using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public class SwitchCommunication
    {

    }

    public class EricssonSSHCommunication
    {
        public bool IsActive { get; set; }

        public SSHCommunicatorSettings SSHCommunicatorSettings { get; set; }
    }
}
