using System;

namespace Vanrise.Entities
{
    public enum VRIPAddressType { IPv4 = 0, IPv6 = 1 }

    public class VRIPAddress
    {
        public VRIPAddressType Type { get; set; }

        public string IPAddress { get; set; }

        public int SubnetPrefixLength { get; set; }
    }
}