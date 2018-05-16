using System;
using Vanrise.Security.Entities;

namespace Vanrise.Security.MainExtensions.SecurityProviderDefinition
{
    public class LocalSecurityProviderDefinitionExtendedSettings : SecurityProviderDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("4BBAD5A1-81E6-4999-9A13-48CFB52065A5"); } }

        public override string RuntimeEditor { get { return "vr-sec-securityproviderdefinition-local"; } }
    }
}