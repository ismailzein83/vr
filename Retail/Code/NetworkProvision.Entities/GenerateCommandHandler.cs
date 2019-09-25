using System;

namespace NetworkProvision.Entities
{
    public abstract class GenerateCommandHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetCode(IGenerateCommandHandlerGetCodeContect context);

        public abstract string Execute(IGenerateCommandHandlerExecuteContect context);
    }

    public interface IGenerateCommandHandlerGetCodeContect
    {

    }

    public interface IGenerateCommandHandlerExecuteContect
    {

    }
}
