using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProvision.Entities
{
    public abstract class ProvisionHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetCode(IProvisionHandlerGetCodeContect context);

        public abstract string Execute(IProvisionHandlerExecuteContect context);
    }

    public interface IProvisionHandlerGetCodeContect { }
    public class ProvisionHandlerGetCodeContect : IProvisionHandlerGetCodeContect { }

    public interface IProvisionHandlerExecuteContect { }
    public class ProvisionHandlerExecuteContect : IProvisionHandlerExecuteContect { }

}
