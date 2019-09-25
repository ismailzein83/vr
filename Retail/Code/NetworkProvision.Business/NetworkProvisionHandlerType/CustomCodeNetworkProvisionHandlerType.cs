using System;
using NetworkProvision.Entities;

namespace NetworkProvision.Business.NetworkProvisionHandlerType
{
    public class CustomCodeNetworkProvisionHandlerType : NetworkProvisionHandlerTypeExtendedSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public override string RuntimeEditor => throw new NotImplementedException();

        public string NamespaceMembers { get; set; }

        public string CustomCode { get; set; }

        public override bool Execute(INetworkProvisionHandlerTypeExecuteContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetCode(INetworkProvisionHandlerTypeGetCodeContext context)
        {
            throw new NotImplementedException();
        }
    }
}
