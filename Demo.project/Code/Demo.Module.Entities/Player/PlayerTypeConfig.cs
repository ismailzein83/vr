using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class PlayerTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_Module_PlayerType";

        public string Editor { get; set; }
    }
}