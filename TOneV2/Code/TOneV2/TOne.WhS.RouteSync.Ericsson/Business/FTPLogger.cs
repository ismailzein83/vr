using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            string errorMessage;
            FTPCommunicator communicator = new FTPCommunicator(this.FTPCommunicatorSettings);
            communicator.TryWriteFile(ms, "", out errorMessage);
        }

        public override void LogCarrierMappings(ILogCarrierMappingsContext context)
        {
            throw new NotImplementedException();
        }

        public override void LogRoutes(ILogRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void LogCommands(ILogCommandsContext context)
        {
            throw new NotImplementedException();
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
                    {
                        strBuilder.AppendLine(commandOutput);
                    }
                }
                strBuilder.AppendLine();
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(strBuilder.ToString());
            return new MemoryStream(byteArray);
        }
    }
}
