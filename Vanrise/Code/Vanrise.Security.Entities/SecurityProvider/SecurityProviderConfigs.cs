using System;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class SecurityProviderConfigs : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Sec_SecurityProviderSettings";
        public string Editor { get; set; }
    }
}
