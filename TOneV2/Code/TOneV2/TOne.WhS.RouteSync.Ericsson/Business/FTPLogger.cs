using System;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class FTPLogger : SwitchLogger
    {
        public override Guid ConfigId { get { return new Guid("4B424B30-083C-4999-B883-AF4555ECC819"); } }

        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }


        public override void LogRouteCases(ILogRouteCasesContext context)
        {
            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, "RouteCase", this.FTPCommunicatorSettings, context.ExecutionDateTime, context.ExecutionStatus, out errorMessage);
        }

        public override void LogCarrierMappings(ILogCarrierMappingsContext context)
        {
            context.CommandResults.Insert(0, new CommandResult() { Command = string.Format("{0};", EricssonCommands.PNBZI_Command) });
            context.CommandResults.Insert(1, new CommandResult() { Command = string.Format("{0};", EricssonCommands.PNBCI_Command) });
            context.CommandResults.Add(new CommandResult() { Command = string.Format("{0};", EricssonCommands.PNBAI_Command) });

            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, "PreTableScript", this.FTPCommunicatorSettings, context.ExecutionDateTime, context.ExecutionStatus, out errorMessage);
        }

        public override void LogRoutes(ILogRoutesContext context)
        {
            context.CommandResults.Insert(0, new CommandResult() { Command = string.Format("{0};", EricssonCommands.ANBZI_Command) });
            context.CommandResults.Insert(1, new CommandResult() { Command = string.Format("{0};", EricssonCommands.ANBCI_Command) });
            context.CommandResults.Add(new CommandResult() { Command = string.Format("{0};", EricssonCommands.ANBAI_Command) });

            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, string.Format("RouteBO{0}", context.CustomerIdentifier), this.FTPCommunicatorSettings, context.ExecutionDateTime, context.ExecutionStatus, out errorMessage);
        }

        public override void LogCommands(ILogCommandsContext context)
        {
            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, "CommandExecution", this.FTPCommunicatorSettings, context.ExecutionDateTime, null, out errorMessage);
        }
    }
}