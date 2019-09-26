using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class TeamSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_Module_TeamSettings";

        public string Editor { get; set; }
    }
}