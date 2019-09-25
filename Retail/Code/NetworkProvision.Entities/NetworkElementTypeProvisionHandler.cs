using System;

namespace NetworkProvision.Entities
{
    public class NetworkElementTypeProvisionHandler
    {
        public long Id { get; set; }
        public long ActionId { get; set; }
        public Guid NetworkElementTypeId { get; set; }
        public ProvisionHandler GenerateCodeHandler { get; set; }
        public ProvisionHandler ProvisionHandler { get; set; }
    }
}
