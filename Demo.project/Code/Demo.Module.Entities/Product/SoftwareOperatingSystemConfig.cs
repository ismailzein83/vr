using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class SoftwareOperatingSystemConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_Module_OperatingSystem";

        public string Editor { get; set; }
    }
}