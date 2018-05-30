using System;
using Vanrise.Security.Entities;

namespace Vanrise.Security.MainExtensions.SecurityProviderDefinition
{
    public class RemoteSecurityProviderDefinitionExtendedSettings : SecurityProviderDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("F711C66D-131B-48A8-8F48-E5B130963748"); } }

        public override string RuntimeEditor { get { return "vr-sec-securityproviderdefinition-remote"; } }
    }
}