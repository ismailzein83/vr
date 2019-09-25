using System;

namespace NetworkProvision.Entities
{
    public abstract class NetworkProvisionHandlerTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string RuntimeEditor { get; }

        public abstract string GetCode(INetworkProvisionHandlerTypeGetCodeContext context);

        public abstract bool Execute(INetworkProvisionHandlerTypeExecuteContext context);
    }

    public interface INetworkProvisionHandlerTypeGetCodeContext
    {
    }

    public interface INetworkProvisionHandlerTypeExecuteContext
    {
    }
}
