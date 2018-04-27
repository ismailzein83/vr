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

    public interface ILogRouteCasesContext 
    {
        List<CommandResult> CommandResults { get; }

        DateTime ExecutionDateTime { get; }

        ExecutionStatus ExecutionStatus { get; }
    }

    public class LogRouteCasesContext : ILogRouteCasesContext
    {
        public List<CommandResult> CommandResults { get; set; }

        public DateTime ExecutionDateTime { get; set; }

        public ExecutionStatus ExecutionStatus { get; set; }
    }

    public interface ILogCarrierMappingsContext 
    {
        List<CommandResult> CommandResults { get; }

        DateTime ExecutionDateTime { get; }

        ExecutionStatus ExecutionStatus { get; } 
    }

    public class LogCarrierMappingsContext : ILogCarrierMappingsContext
    {
        public List<CommandResult> CommandResults { get; set; }

        public DateTime ExecutionDateTime { get; set; }

        public ExecutionStatus ExecutionStatus { get; set; }
    }

    public interface ILogRoutesContext
    {
        List<CommandResult> CommandResults { get; }

        DateTime ExecutionDateTime { get; }

        ExecutionStatus ExecutionStatus { get; }

        int BONumber { get; }
    }

    public class LogRoutesContext : ILogRoutesContext
    {
        public List<CommandResult> CommandResults { get; set; }

        public DateTime ExecutionDateTime { get; set; }

        public ExecutionStatus ExecutionStatus { get; set; }

        public int BONumber { get; set; }
    }

    public interface ILogCommandsContext
    {
        List<CommandResult> CommandResults { get; }

        DateTime ExecutionDateTime { get; }
    }

    public class LogCommandsContext : ILogCommandsContext
    {
        public List<CommandResult> CommandResults { get; set; }

        public DateTime ExecutionDateTime { get; set; }
    }

    public enum ExecutionStatus { Failed = 0, Succeeded = 1 }
}
