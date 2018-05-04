using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            MemoryStream ms = GenerateStream(context.CommandResults);

            string fileName;
			string executionDateTimeAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd HH-mm-ss-fff");
			string executionDateAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd");

            switch (context.ExecutionStatus)
            {
                case ExecutionStatus.Succeeded: fileName = string.Format("RouteCase_Successful_{0}", executionDateTimeAsString); break;
                case ExecutionStatus.Failed: fileName = string.Format("RouteCase_Unsuccessful_{0}", executionDateTimeAsString); break;
                default: throw new NotSupportedException(string.Format("context.ExecutionStatus '{0}' is not supported", context.ExecutionStatus));
            }

            string errorMessage;
            FTPCommunicator communicator = new FTPCommunicator(this.FTPCommunicatorSettings);
            communicator.TryWriteFile(ms, fileName, executionDateAsString, out errorMessage);
        }

        public override void LogCarrierMappings(ILogCarrierMappingsContext context)
        {
            MemoryStream ms = GenerateStream(context.CommandResults);

            string fileName;
			string executionDateTimeAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd HH-mm-ss-fff");
			string executionDateAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd");

			switch (context.ExecutionStatus)
            {
                case ExecutionStatus.Succeeded: fileName = string.Format("PreTableScript_Successful_{0}", executionDateTimeAsString); break;
                case ExecutionStatus.Failed: fileName = string.Format("PreTableScript_Unsuccessful_{0}", executionDateTimeAsString); break;
                default: throw new NotSupportedException(string.Format("context.ExecutionStatus '{0}' is not supported", context.ExecutionStatus));
            }

            string errorMessage;
            FTPCommunicator communicator = new FTPCommunicator(this.FTPCommunicatorSettings);
            communicator.TryWriteFile(ms, fileName, executionDateAsString, out errorMessage);
        }

        public override void LogRoutes(ILogRoutesContext context)
        {
            MemoryStream ms = GenerateStream(context.CommandResults);

            string fileName;
			string executionDateTimeAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd HH-mm-ss-fff");
			string executionDateAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd");

			switch (context.ExecutionStatus)
            {
                case ExecutionStatus.Succeeded: fileName = string.Format("RouteBO{0}_Successful_{1}", context.BONumber, executionDateTimeAsString); break;
                case ExecutionStatus.Failed: fileName = string.Format("RouteBO{0}_Unsuccessful_{1}", context.BONumber, executionDateTimeAsString); break;
                default: throw new NotSupportedException(string.Format("context.ExecutionStatus '{0}' is not supported", context.ExecutionStatus));
            }

            string errorMessage;
            FTPCommunicator communicator = new FTPCommunicator(this.FTPCommunicatorSettings);
            communicator.TryWriteFile(ms, fileName, executionDateAsString, out errorMessage);
        }

        public override void LogCommands(ILogCommandsContext context)
        {
            MemoryStream ms = GenerateStream(context.CommandResults);

			string executionDateTimeAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd HH-mm-ss-fff");
			string executionDateAsString = context.ExecutionDateTime.ToString("yyyy-MM-dd");
			string fileName = string.Format("CommandExecution_{0}", executionDateTimeAsString);

            string errorMessage;
            FTPCommunicator communicator = new FTPCommunicator(this.FTPCommunicatorSettings);
            communicator.TryWriteFile(ms, fileName, executionDateAsString, out errorMessage);
        }

        private MemoryStream GenerateStream(List<CommandResult> commandResults)
        {
            if (commandResults == null)
                return null;

            StringBuilder strBuilder = new StringBuilder();
            foreach (CommandResult commandResult in commandResults)
            {
                strBuilder.AppendLine(commandResult.Command);
				if (commandResult.Output != null && commandResult.Output.Any())
				{
					foreach (string commandOutput in commandResult.Output)
						strBuilder.AppendLine(commandOutput);
				}
				strBuilder.AppendLine();
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(strBuilder.ToString());
            return new MemoryStream(byteArray);
        }
    }
}