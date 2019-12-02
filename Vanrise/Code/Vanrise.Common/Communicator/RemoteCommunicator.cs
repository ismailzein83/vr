using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public abstract class RemoteCommunicator : IDisposable
    {
        public abstract void OpenConnection();
        public abstract void OpenShell();
        public abstract void ExecuteCommand(string command);
        public abstract void ExecuteCommand(string command, string endOfResponse, out string response);
        public abstract void ExecuteCommand(string command, string endOfResponse, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response);
        public abstract void ExecuteCommand(string command, List<string> endOfResponseList, out string response);
        public abstract void ExecuteCommand(string command, List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, out string response);
        public abstract String ReadPrompt(String prompt);
        public abstract String ReadResponse(List<string> endOfResponseList, bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse);
        public abstract void Dispose();
    }

    public abstract class RemoteCommunicatorSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract RemoteCommunicator GetCommunicator();
    }

    public class SSHRemoteCommunicatorSettings : RemoteCommunicatorSettings
    {
        public override Guid ConfigId { get { return new Guid("26A5CB09-10E5-4C56-9BD1-ECBDEC47984F"); } }
        public SSHCommunicatorSettings Settings { get; set; }
        public override RemoteCommunicator GetCommunicator()
        {
            return new SSHCommunicator(new SSHCommunicatorSettings { Host = this.Settings.Host, Port = this.Settings.Port, Username = this.Settings.Username, Password = this.Settings.Password, ConnectionTimeOutInSeconds = this.Settings.ConnectionTimeOutInSeconds, ReadTimeOutInSeconds = this.Settings.ReadTimeOutInSeconds });
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

    public class TelnetRemoteCommunicatorSettings : RemoteCommunicatorSettings
    {
        public override Guid ConfigId { get { return new Guid("76458FE8-3489-416C-9A6D-C2E0C9B11BEA"); } }
        public TelnetCommunicatorSettings Settings { get; set; }
        public override RemoteCommunicator GetCommunicator()
        {
            return new TelnetCommunicator(new TelnetCommunicatorSettings { Host = this.Settings.Host, Port = this.Settings.Port, Username = this.Settings.Username, Password = this.Settings.Password, ConnectionTimeOutInSeconds = this.Settings.ConnectionTimeOutInSeconds });
        }
    }

    public class TelnetCommunicatorSettings
    {
        public String Host { get; set; }
        public int Port { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public int ConnectionTimeOutInSeconds { get; set; }
    }

    public class RemoteCommunicatorSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_RemoteCommunicatorSettings";
        public string Editor { get; set; }
    }
}
