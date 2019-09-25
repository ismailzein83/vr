using System;

namespace NetworkProvision.Entities
{
    public class NetworkProvisionGenerateCommandHandlerTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId => new Guid("93E2192D-7C3F-4914-9B03-F58195E71BD7");

        public NPGenerateCommandHandlerExtendedSettings ExtendedSettings { get; set; }
    }
}
