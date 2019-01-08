using System;
using System.Collections.Generic;

namespace Vanrise.Entities
{
    public class VRNamespace
    {
        public Guid VRNamespaceId { get; set; }
        public string Name { get; set; }
    }

    public class VRNamespaceSettings
    {
        public List<VRDynamicCode> Codes { get; set; }
    }
    public class VRDynamicCodeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_DynamicCode";
        public string Editor { get; set; }
    }
}
