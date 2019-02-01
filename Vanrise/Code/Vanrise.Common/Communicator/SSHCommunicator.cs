using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Renci.SshNet;

namespace Vanrise.Common
{
    public class SSHCommunicator : IDisposable
    {
        private SshClient client;

        private ShellStream shellStream;

        SSHCommunicatorSettings SSHCommunicatorSettings { get; set; }

        public SSHCommunicator(SSHCommunicatorSettings sshCommunicatorSettings)
        {
            this.SSHCommunicatorSettings = sshCommunicatorSettings;
        }

        public void OpenConnection()
        {
            client = new Renci.SshNet.SshClient(SSHCommunicatorSettings.Host, SSHCommunicatorSettings.Port, SSHCommunicatorSettings.Username, SSHCommunicatorSettings.Password);
            client.ConnectionInfo.Timeout = new TimeSpan(0, 0, SSHCommunicatorSettings.ConnectionTimeOutInSeconds);
            client.Connect();
        }

        public void OpenShell()
        {
            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            termkvp.Add(Renci.SshNet.Common.TerminalModes.ECHO, 53);

            shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024, termkvp);
        }

        public void ExecuteCommand(string command)
        {
            shellStream.WriteLine(command);
        }

        public void ExecuteCommand(string command, string endOfResponse, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadPrompt(endOfResponse);
        }

        public void ExecuteCommand(string command, string endOfResponse, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(new List<string>() { endOfResponse }, isEndOfResponseCaseSensitive, ignoreEmptySpacesInResponse);
        }

        public void ExecuteCommand(string command, List<string> endOfResponseList, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(endOfResponseList, true, false);
        }

        public void ExecuteCommand(string command, List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(endOfResponseList, isEndOfResponseCaseSensitive, ignoreEmptySpacesInResponse);
        }

        public String ReadPrompt(String prompt)
        {
            return ReadResponse(new List<string>() { prompt }, true, false);
        }

        public String ReadResponse(List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse)
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

                if (sw.Elapsed > new TimeSpan(0, 0, SSHCommunicatorSettings.ReadTimeOutInSeconds))
                {
                    sw.Stop();
                    return null;
                }
            }
            sw.Stop();
            return response;
        }

        public void Dispose()
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

    public class SSHCommunicatorSettings
    {
        public String Host { get; set; }

        public int Port { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

        public int ConnectionTimeOutInSeconds { get; set; }

        public int ReadTimeOutInSeconds { get; set; }
    }
}