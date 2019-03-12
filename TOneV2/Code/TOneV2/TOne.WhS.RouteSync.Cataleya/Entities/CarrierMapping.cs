using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public enum TransportProtocol { UDP = 0, TCP = 1, TLS = 2, UDP_TCP = 3, WS = 4, WSS = 5 }

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<CustomerMapping> CustomerMappings { get; set; }

        public List<SupplierMapping> SupplierMappings { get; set; }
    }

    public class CustomerMapping
    {
        public VRIPAddress IPAddress { get; set; }
    }

    public class SupplierMapping
    {
        public VRIPAddress IPAddress { get; set; }

        public string Domain { get; set; }

        public int Priority { get; set; }

        public int Weight { get; set; }

        public TransportProtocol TransportProtocol { get; set; }

        public string NationalCountryCode { get; set; }

        public bool ReuseConnection { get; set; }

        public bool IsSwitch { get; set; }
    }
}