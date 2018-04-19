using Rebex.Net;
using System;
using System.IO;

namespace Vanrise.Common
{
    public enum FTPType { FTP = 0, SFTP = 1 }
    public class FTPCommunicatorSettings
    {
        public FTPType FTPType { get; set; }
        public string Directory { get; set; }
        public string ServerIP { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class FTPCommunicator
    {
        FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }
        FTPCommunicatorClient CommunicatorClient { get; set; }

        public FTPCommunicator(FTPCommunicatorSettings ftpCommunicatorSettings)
        {
            this.FTPCommunicatorSettings = ftpCommunicatorSettings;

            switch (FTPCommunicatorSettings.FTPType)
            {
                case Common.FTPType.FTP: CommunicatorClient = new FTPClient(); break;
                case Common.FTPType.SFTP: CommunicatorClient = new SFTPClient(); break;
                default: throw new NotSupportedException(string.Format("FTPType '{0}' is not supported", FTPCommunicatorSettings.FTPType));
            }
        }

        public bool TryOpenConnection()
        {
            return CommunicatorClient.TryOpenConnection(FTPCommunicatorSettings.ServerIP, FTPCommunicatorSettings.Username, FTPCommunicatorSettings.Password);
        }

        public void CloseConnection()
        {
            CommunicatorClient.CloseConnection();
        }

        public void CreateFile(Stream stream, string fileName)
        {
            CommunicatorClient.CreateFile(stream, string.Format("{0}/{1}", FTPCommunicatorSettings.Directory, fileName));
        }

        private abstract class FTPCommunicatorClient
        {
            public abstract bool TryOpenConnection(string serverIP, string userName, string password);

            public abstract void CloseConnection();

            public abstract void CreateFile(Stream stream, string remotePath);
        }

        private class FTPClient : FTPCommunicatorClient
        {
            Ftp Ftp { get; set; }

            public FTPClient()
            {
                Ftp = new Ftp();
            }

            public override bool TryOpenConnection(string serverIP, string userName, string password)
            {
                Ftp.Connect(serverIP);
                Ftp.Login(userName, password);
                return Ftp.GetConnectionState().Connected;
            }

            public override void CloseConnection()
            {
                Ftp.Dispose();
            }

            public override void CreateFile(Stream stream, string remotePath)
            {
                Ftp.PutFile(stream, remotePath);
            }
        }

        private class SFTPClient : FTPCommunicatorClient
        {
            Sftp Sftp { get; set; }

            public SFTPClient()
            {
                Sftp = new Sftp();
            }

            public override bool TryOpenConnection(string serverIP, string userName, string password)
            {
                Sftp.Connect(serverIP);
                Sftp.Login(userName, password);
                return Sftp.GetConnectionState().Connected;
            }

            public override void CloseConnection()
            {
                Sftp.Dispose();
            }

            public override void CreateFile(Stream stream, string remotePath)
            {
                Sftp.PutFile(stream, remotePath);
            }
        }
    }
}