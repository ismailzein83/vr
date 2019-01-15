using System;
using System.Collections.Generic;

namespace Vanrise.MobileNetwork.Entities
{
    public class MobileNetworkSettings
    {
        public List<MobileNetworkCode> Codes { get; set; }
    }

    public class MobileNetworkCode
    {
        public string Code { get; set; }
    }
}