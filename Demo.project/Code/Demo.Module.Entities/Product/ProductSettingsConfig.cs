using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class ProductSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_Module_ProductSettings";

        public string Editor { get; set; }
    }
}