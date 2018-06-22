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

    public class FTPCommunicator : IDisposable
    {
        FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }
        FTPCommunicatorClient CommunicatorClient { get; set; }

        public FTPCommunicator(FTPCommunicatorSettings ftpCommunicatorSettings)
        {
            this.FTPCommunicatorSettings = ftpCommunicatorSettings;

            switch (FTPCommunicatorSettings.FTPType)
            {
                case Common.FTPType.FTP: this.CommunicatorClient = new FTPClient(); break;
                case Common.FTPType.SFTP: this.CommunicatorClient = new SFTPClient(); break;
                default: throw new NotSupportedException(string.Format("FTPType '{0}' is not supported", FTPCommunicatorSettings.FTPType));
            }
        }

        public bool TryWriteFile(Stream stream, string fileName, string subDirectory, out string errorMessage)
        {
            errorMessage = null;

            TryOpenConnection(out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return false;

            string directory = this.FTPCommunicatorSettings.Directory;
            if (!string.IsNullOrEmpty(subDirectory))
            {
                directory += "/" + subDirectory;
                this.CreateDirectoryIfNotExists(directory);
            }

            this.CreateFile(stream, fileName, directory);
            return true;
        }

        public void Dispose()
        {
            CloseConnection();
        }

        private void TryOpenConnection(out string errorMessage)
        {
            errorMessage = null;
            bool tryOpenConnection = CommunicatorClient.TryOpenConnection(FTPCommunicatorSettings.ServerIP, FTPCommunicatorSettings.Username, FTPCommunicatorSettings.Password);
            if (!tryOpenConnection)
            {
                errorMessage = string.Format("Unable to open connection ServerIP: {0}, Username: {1}, Password: {2}",
                    FTPCommunicatorSettings.ServerIP, FTPCommunicatorSettings.Username, FTPCommunicatorSettings.Password);
            }
        }

        private void CreateFile(Stream stream, string fileName, string directory)
        {
            CommunicatorClient.CreateFile(stream, string.Format("{0}/{1}", directory, fileName));
        }

        private void CreateDirectoryIfNotExists(string directory)
        {
            CommunicatorClient.CreateDirectoryIfNotExists(directory);
        }

        private void CloseConnection()
        {
            CommunicatorClient.CloseConnection();
        }

        #region Private Classes

        private abstract class FTPCommunicatorClient
        {
            public abstract bool TryOpenConnection(string serverIP, string userName, string password);

            public abstract void CreateFile(Stream stream, string remotePath);

            public abstract void CreateDirectoryIfNotExists(string remotePath);

            public abstract void CloseConnection();
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
                if (Ftp.GetConnectionState().Connected)
                    return true;

                Ftp.Connect(serverIP);
                Ftp.Login(userName, password);
                return Ftp.GetConnectionState().Connected;
            }

            public override void CreateFile(Stream stream, string remotePath)
            {
                Ftp.PutFile(stream, remotePath);
            }

            public override void CreateDirectoryIfNotExists(string remotePath)
            {
                if (Ftp.DirectoryExists(remotePath))
                    return;

                Ftp.CreateDirectory(remotePath);
            }

            public override void CloseConnection()
            {
                Ftp.Dispose();
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
                if (Sftp.GetConnectionState().Connected)
                    return true;

                Sftp.Connect(serverIP);
                Sftp.Login(userName, password);
                return Sftp.GetConnectionState().Connected;
            }

            public override void CreateFile(Stream stream, string remotePath)
            {
                Sftp.PutFile(stream, remotePath);
            }

            public override void CreateDirectoryIfNotExists(string remotePath)
            {
                Sftp.CreateDirectory(remotePath);
            }

            public override void CloseConnection()
            {
                Sftp.Dispose();
            }
        }

        #endregion
    }
}