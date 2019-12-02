using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Renci.SshNet;

namespace Vanrise.Common
{
    public class SSHCommunicator : RemoteCommunicator
    {
        private SshClient client;

        private ShellStream shellStream;

        SSHCommunicatorSettings Settings { get; set; }

        public SSHCommunicator(SSHCommunicatorSettings sshCommunicatorSettings)
        {
            this.Settings = sshCommunicatorSettings;
        }

        public override void OpenConnection()
        {
            client = new Renci.SshNet.SshClient(Settings.Host, Settings.Port, Settings.Username, Settings.Password);
            client.ConnectionInfo.Timeout = new TimeSpan(0, 0, Settings.ConnectionTimeOutInSeconds);
            client.Connect();
        }

        public override void OpenShell()
        {
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

            shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
        }

        public override void ExecuteCommand(string command)
        {
            shellStream.WriteLine(command);
        }

        public override void ExecuteCommand(string command, string endOfResponse, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadPrompt(endOfResponse);
        }

        public override void ExecuteCommand(string command, string endOfResponse, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(new List<string>() { endOfResponse }, isEndOfResponseCaseSensitive, ignoreEmptySpacesInResponse);
        }

        public override void ExecuteCommand(string command, List<string> endOfResponseList, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(endOfResponseList, true, false);
        }

        public override void ExecuteCommand(string command, List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(endOfResponseList, isEndOfResponseCaseSensitive, ignoreEmptySpacesInResponse);
        }

        public override String ReadPrompt(String prompt)
        {
            return ReadResponse(new List<string>() { prompt }, true, false);
        }

        public override String ReadResponse(List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse)
        {
            List<string> modifiedEndOfResponseList = null;
            if (endOfResponseList != null)
            {
                modifiedEndOfResponseList = new List<string>();
                foreach (string str in endOfResponseList)
                {
                    string modifiedStr = str;
                    if (!isEndOfResponseCaseSensitive)
                        modifiedStr = modifiedStr.ToLower();

                    if (ignoreEmptySpacesInResponse)
                        modifiedStr = modifiedStr.Replace(" ", "");

                    modifiedEndOfResponseList.Add(modifiedStr);
                }
            }

            String response = string.Empty;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                String data = shellStream.Read();
                response += data;

                string modifiedResponse = response;
                if (!isEndOfResponseCaseSensitive)
                    modifiedResponse = modifiedResponse.ToLower();

                if (ignoreEmptySpacesInResponse)
                    modifiedResponse = modifiedResponse.Replace(" ", "");

                if (modifiedEndOfResponseList != null && modifiedEndOfResponseList.Any(modifiedResponse.Contains))
                    break;

                if (sw.Elapsed > new TimeSpan(0, 0, Settings.ReadTimeOutInSeconds))
                {
                    sw.Stop();
                    return null;
                }
            }
            sw.Stop();
            return response;
        }

        public override void Dispose()
        {
            if (shellStream != null)
            {
                shellStream.Close();
                shellStream.Dispose();
            }

            if (client != null)
            {
                client.Disconnect();
                client.Dispose();
            }
        }
    }
}