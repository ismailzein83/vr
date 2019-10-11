﻿using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Entities
{
    public enum ExecutionStatus { Failed = 0, Succeeded = 1 }

    public class CommandResult
    {
        public string Command { get; set; }

        public List<string> Output { get; set; }
    }

    public interface ILogBaseContext
    {
        List<CommandResult> CommandResults { get; }

        DateTime ExecutionDateTime { get; }

        ExecutionStatus ExecutionStatus { get; }
    }

    public class LogBaseContext : ILogBaseContext
    {
        public List<CommandResult> CommandResults { get; set; }

        public DateTime ExecutionDateTime { get; set; }

        public ExecutionStatus ExecutionStatus { get; set; }
    }

    #region Route Options
    public interface ILogRouteOptionsContext : ILogBaseContext
    {
    }

    public class LogRouteOptionsContext : LogBaseContext, ILogRouteOptionsContext
    {
    }
    #endregion

    #region Route Case
    public interface ILogRouteCasesContext : ILogBaseContext
    {
    }

    public class LogRouteCasesContext : LogBaseContext, ILogRouteCasesContext
    {
    }
    #endregion

    #region Carrier Mapping

    public interface ILogCarrierMappingsContext : ILogBaseContext
    {
    }

    public class LogCarrierMappingsContext : LogBaseContext, ILogCarrierMappingsContext
    {
       
    }
    #endregion

    #region Routes
    public interface ILogRoutesContext : ILogBaseContext
    {
        string CustomerIdentifier { get; }
    }

    public class LogRoutesContext : LogBaseContext, ILogRoutesContext
    {
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