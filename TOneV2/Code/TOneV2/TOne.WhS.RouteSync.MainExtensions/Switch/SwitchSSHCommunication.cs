using System;
using Vanrise.Common;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.Switch
{
    public class SwitchSSHCommunication : SwitchCommunication
    {
        public override Guid ConfigId { get { return new Guid("6829DADF-DEFB-4F87-9931-8D68AAADAE71"); } }

        public SSHCommunicatorSettings SSHCommunicatorSettings { get; set; }

    }
}