using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public class TelnetCommunicator : RemoteCommunicator
    {
        Rebex.Net.Telnet client;
        Rebex.TerminalEmulation.Shell shellStream;
        TelnetCommunicatorSettings Settings { get; set; }

        public TelnetCommunicator(TelnetCommunicatorSettings telnetCommunicatorSettings)
        {
            this.Settings = telnetCommunicatorSettings;
        }

        public override void OpenConnection()
        {
            client = new Rebex.Net.Telnet(Settings.Host, Settings.Port);
            client.Timeout = Settings.ConnectionTimeOutInSeconds;
        }

        public override void OpenShell()
        {
            if (client == null)
                OpenConnection();

            if (shellStream != null)
                shellStream.Close();

            shellStream = client.StartShell();
            shellStream.Encoding = Encoding.UTF8;
            shellStream.Timeout = Settings.ConnectionTimeOutInSeconds;
            shellStream.Prompt = ">";
            LogIn();
        }

        public override void ExecuteCommand(string command)
        {
            if (shellStream == null || !shellStream.Connected)
                OpenShell();

            //string response = "";
            //response = shellStream.ReadAll();
            shellStream.SendCommand(command);
            //response = shellStream.ReadAll();
        }

        public override void ExecuteCommand(string command, string endOfResponse, out string response)
        {
            if (shellStream == null || !shellStream.Connected)
                OpenShell();

            shellStream.SendCommand(command);
            response = shellStream.ReadAll(endOfResponse);
        }

        public override void ExecuteCommand(string command, string endOfResponse, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response)
        {
            if (shellStream == null || !shellStream.Connected)
                OpenShell();

            shellStream.SendCommand(command);
            var promptsArray = GetPromptsArray(new List<string> { endOfResponse }, false);
            response = shellStream.ReadAll(promptsArray);

            if (ignoreEmptySpacesInResponse)
                response = response.Replace(" ", "");
        }

        public override void ExecuteCommand(string command, List<string> endOfResponseList, out string response)
        {
            if (endOfResponseList == null)
                throw new NullReferenceException("endOfResponseList");

            if (shellStream == null || !shellStream.Connected)
                OpenShell();

            shellStream.SendCommand(command);
            var promptsArray = GetPromptsArray(endOfResponseList, false);
            response = shellStream.ReadAll(promptsArray);
        }

        public override void ExecuteCommand(string command, List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response)
        {
            if (endOfResponseList == null)
                throw new NullReferenceException("endOfResponseList");

            if (shellStream == null || !shellStream.Connected)
                OpenShell();

            shellStream.SendCommand(command);
            var promptsArray = GetPromptsArray(endOfResponseList, isEndOfResponseCaseSensitive);
            response = shellStream.ReadAll(promptsArray);
            if (ignoreEmptySpacesInResponse)
                response = response.Replace(" ", "");
        }

        public override string ReadPrompt(string prompt)
        {
            if (shellStream == null || !shellStream.Connected)
                OpenShell();
            return shellStream.ReadAll(prompt);
        }

        public override string ReadResponse(List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse)
        {
            if (shellStream == null || !shellStream.Connected)
                OpenShell();

            string response;
            var promptsArray = GetPromptsArray(endOfResponseList, isEndOfResponseCaseSensitive);
            response = shellStream.ReadAll(promptsArray);
            if (ignoreEmptySpacesInResponse)
                response = response.Replace(" ", "");
            return response;
        }

        public override void Dispose()
        {
            if (shellStream != null)
                shellStream.Close();
        }

        private bool LogIn()
        {
            if (shellStream == null)
                OpenShell();

            string message = "";
            message = shellStream.ReadAll("login: ");
            shellStream.SendCommand(Settings.Username);

            message = shellStream.ReadAll("password: ");
            shellStream.SendCommand(Settings.Password, true);

            string loginMessage = shellStream.ReadAll(">");

            return loginMessage.Contains("Microsoft Telnet Server");
        }

        private string[] GetPromptsArray(List<string> endOfResponseList, bool isEndOfResponseCaseSensitive)
        {
            if (endOfResponseList == null || endOfResponseList.Count == 0)
                return null;
            var prompts = new List<string>();
            foreach (var endOfResponse in endOfResponseList)
            {
                if (isEndOfResponseCaseSensitive)
                {
                    prompts.Add(endOfResponse.ToLower());
                    prompts.Add(endOfResponse.ToUpper());
                }
                else
                    prompts.Add(endOfResponse);
            }
            return prompts.ToArray();

        }
    }
}
