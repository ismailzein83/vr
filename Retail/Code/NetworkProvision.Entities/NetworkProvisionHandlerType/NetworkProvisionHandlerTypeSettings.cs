using System;

namespace NetworkProvision.Entities
{
    public class NetworkProvisionHandlerTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId => new Guid("EA229E70-D958-4777-9DAF-9FFB7BECDEE6");

        public NetworkProvisionHandlerTypeExtendedSettings ExtendedSettings { get; set; }
    }
}
