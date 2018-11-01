using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Entities
{
    public enum ExecutionStatus { Failed = 0, Succeeded = 1 }

    public class CommandResult
    {
        public string Command { get; set; }

        public List<string> Output { get; set; }
    }

    #region Route Case
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
    #endregion

    #region Carrier Mapping
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
    #endregion

    #region Routes
    public interface ILogRoutesContext
    {
        List<CommandResult> CommandResults { get; }

        DateTime ExecutionDateTime { get; }

        ExecutionStatus ExecutionStatus { get; }

        string CustomerIdentifier { get; }
    }

    public class LogRoutesContext : ILogRoutesContext
    {
        public List<CommandResult> CommandResults { get; set; }

        public DateTime ExecutionDateTime { get; set; }

        public ExecutionStatus ExecutionStatus { get; set; }

        public string CustomerIdentifier { get; set; }
    }
    #endregion

    #region Commands
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
    #endregion
}