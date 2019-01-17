using System;

namespace Vanrise.MobileNetwork.Entities
{
    public class MobileNetwork
    {
        public int Id { get; set; }
        public string NetworkName { get; set; }
        public MobileNetworkSettings MobileNetworkSettings { get; set; }
        public int MobileCountryId { get; set; }
    }
}