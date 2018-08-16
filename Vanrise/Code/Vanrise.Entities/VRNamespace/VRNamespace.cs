using System;

namespace Vanrise.Entities
{
    public class VRNamespace
    {
        public Guid VRNamespaceId { get; set; }
        public string Name { get; set; }
        public VRNamespaceSettings Settings { get; set; }
    }

    public class VRNamespaceSettings
    {
        public string Code { get; set; }
    }
}
