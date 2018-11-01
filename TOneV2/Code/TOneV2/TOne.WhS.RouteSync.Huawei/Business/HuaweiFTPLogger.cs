using System;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.Business
{
    public class HuaweiFTPLogger : SwitchLogger
    {
        public override Guid ConfigId { get { return new Guid("6F5E6051-DDAB-411C-88F5-C8927417FD3C"); } }

        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }

        public override void LogRouteCases(ILogRouteCasesContext context)
        {
            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, "RouteAnalysis", this.FTPCommunicatorSettings, context.ExecutionDateTime, out errorMessage);
        }

        public override void LogRoutes(ILogRoutesContext context)
        {
            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, string.Format("RouteRSSN{0}", context.CustomerIdentifier), this.FTPCommunicatorSettings, context.ExecutionDateTime, out errorMessage);
        }

        public override void LogCommands(ILogCommandsContext context)
        {
            string errorMessage;
            SwitchFTPLoggerHelper.TryLogCommnadResults(context.CommandResults, "CommandExecution", this.FTPCommunicatorSettings, context.ExecutionDateTime, out errorMessage);
        }
    }
}