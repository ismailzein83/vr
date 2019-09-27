using NetworkProvision.Entities;
using System;

namespace NetworkProvision.Business
{
    public class CustomCodeNetworkProvisionHandlerType : NetworkProvisionHandlerTypeExtendedSettings
    {
        public override Guid ConfigId => new Guid("41442404-2C88-4646-9F92-9ADE92A972D5");

        public override string RuntimeEditor => "networkprovision-handlertype-extendedsettings-customcode-runtime";

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
