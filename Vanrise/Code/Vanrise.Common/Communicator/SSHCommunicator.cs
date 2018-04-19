using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Renci.SshNet;

namespace Vanrise.Common
{
    public class SSHCommunicatorSettings
    {
        public String Host { get; set; }

        public int Port { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }

        public int ConnectionTimeOutInSeconds { get; set; }

        public int ReadTimeOutInSeconds { get; set; }
    }

    public class SSHCommunicator
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

        public void ExecuteCommand(string command, string prompt, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadPrompt(prompt);
        }

        public void ExecuteCommand(string command, List<string> endOfResultList, out string response)
        {
            shellStream.WriteLine(command);
            response = ReadResponse(endOfResultList);
        }

        public String ReadPrompt(String prompt)
        {
            return ReadResponse(new List<string>() { prompt });
        }

        public String ReadResponse(List<string> endOfResultList)
        {
            String result = string.Empty;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                String data = shellStream.Read();
                result += data;
                if (endOfResultList != null && endOfResultList.Any(result.Contains))//; result.Contains("EXECUTED") || result.Contains("NOT ACCEPTED"))
                    break;

                if (sw.Elapsed > new TimeSpan(0, 0, SSHCommunicatorSettings.ReadTimeOutInSeconds))
                {
                    sw.Stop();
                    return null;
                }
            }
            sw.Stop();
            return result;
        }

        public void Disconnect()
        {
            if (client != null) client.Disconnect();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (client != null) client.Disconnect();
        }

        #endregion
    }
}
