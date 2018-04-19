using System;
using Vanrise.Common;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.MainExtensions.Switch
{
    public class SwitchFTPCommunication : SwitchCommunication
    {
        public override Guid ConfigId { get { return new Guid("4B424B30-083C-4999-B883-AF4555ECC819"); } }

        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }
    }
}