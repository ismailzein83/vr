using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Business
{
    public static class SwitchFTPLoggerHelper
    {
        public static bool TryLogCommnadResults(List<CommandResult> commandResults, string fileNamePrefix, FTPCommunicatorSettings ftpCommunicatorSettings, DateTime executionDateTime, out string errorMessage)
        {
            MemoryStream ms = GenerateStream(commandResults);

            string executionDateTimeAsString = executionDateTime.ToString("yyyy-MM-dd HH-mm-ss-fff");
            string executionDateAsString = executionDateTime.ToString("yyyy-MM-dd");
            string fileName = string.Format("{0}_{1}", fileNamePrefix, executionDateTimeAsString);

            bool result;
            using (FTPCommunicator communicator = new FTPCommunicator(ftpCommunicatorSettings))
            {
                result = communicator.TryWriteFile(ms, fileName, executionDateAsString, out errorMessage);
            }

            return result;
        }

        private static MemoryStream GenerateStream(List<CommandResult> commandResults)
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
            }
            strBuilder.Remove(strBuilder.Length - 2, 2);

            byte[] byteArray = Encoding.UTF8.GetBytes(strBuilder.ToString());
            return new MemoryStream(byteArray);
        }
    }
}
