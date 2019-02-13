using System;
using System.Collections.Generic;
using Vanrise.Common;
using RecordAnalysis.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch
{
    public class HuaweiC4SwitchSettings : C4SwitchSettings
    {
        public override Guid ConfigId { get { return new Guid("D5D7B8A3-3004-46FC-A22D-B060B9BD40D2"); } }

        public List<HuaweiSSHCommunication> HuaweiSSHCommunicationList { get; set; }
    }

    public class HuaweiSSHCommunication
    {
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