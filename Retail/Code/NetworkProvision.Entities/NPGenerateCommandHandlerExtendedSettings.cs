using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProvision.Entities
{
    public abstract class NPGenerateCommandHandlerExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract Guid GenerateCommandHandlerTypeId { get; }

        public abstract string RuntimeEditor { get; }

        public abstract string GetCode(IGenerateCommandHandlerGetCodeContext context);

        public abstract string Execute(IGenerateCommandHandlerExecuteContext context);
    }

    public interface IGenerateCommandHandlerGetCodeContext
    {

    }

    public interface IGenerateCommandHandlerExecuteContext
    {

    }
}
