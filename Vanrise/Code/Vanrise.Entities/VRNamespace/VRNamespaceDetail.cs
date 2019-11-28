using System;

namespace Vanrise.Entities
{
    public class VRNamespaceDetail
    {
        public Guid VRNamespaceId { get; set; }
        public string Name { get; set; }
        public string DevProjectName { get; set; }
        public Guid? DevProjectId { get; set; }
    }
}