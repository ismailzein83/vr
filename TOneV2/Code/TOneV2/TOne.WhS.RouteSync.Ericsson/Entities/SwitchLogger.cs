using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class SwitchLogger
    {
        public abstract Guid ConfigId { get; }
        public bool IsActive { get; set; }

        public abstract void LogRouteCases(ILogRouteCasesContext context);

        public abstract void LogCarrierMappings(ILogCarrierMappingsContext context);

        public abstract void LogRoutes(ILogRoutesContext context);

        public abstract void LogCommands(ILogCommandsContext context);
    }

    public class CommandResult
    {
        public string Command { get; set; }

        public List<string> Output { get; set; }
    }

    public interface ILogRouteCasesContext { List<CommandResult> CommandResults { get; } }

    public interface ILogCarrierMappingsContext { List<CommandResult> CommandResults { get; } }

    public interface ILogRoutesContext { List<CommandResult> CommandResults { get; } }

    public interface ILogCommandsContext { List<CommandResult> CommandResults { get; } }

    public class SwitchLoggerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonSwitchLogger";

        public string Editor { get; set; }
    }
}
