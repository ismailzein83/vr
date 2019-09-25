using System;
using Vanrise.Entities;

namespace NetworkProvision.Entities
{
    public class NetworkProvisionHandlerTypeExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_NetworkProvisionType_ExtendedSettings";

        public string DefinitionEditor { get; set; }
    }
}
