using Rebex.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FtpReceiverAdapter
{

    public class ServerInfo
    {
        public ServerInfo(string serverName, int serverPort)
        {
            ServerName = serverName;
            ServerPort = serverPort;
        }
        public string ServerName { get; set; }
        public int ServerPort { get; set; }
    }

    public class FileReceiveAdapter : BaseReceiveAdapter
    {
        public string Extension { get; set; }

        public string Directory { get; set; }

        public string ServerIP { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool AllowSSH { get; set; }

        public string DirectorytoMoveFile { get; set; }

        public Actions ActionAfterImport { get; set; }

        enum Actions
        {
            Rename = 0,
            Delete = 1,
            Move = 2 // Move to Folder
        }

        public ServerInfo ServerInfo { get; set; }

        public override void ImportData(Action<IImportedData> receiveData)
        {

            if (AllowSSH)
            {
                var sftp = new Rebex.Net.Sftp();

                EstablishConnection(sftp);
                if (sftp.GetConnectionState().Connected)
                {
                    GetServerInfo(sftp);
                    sftp.ChangeDirectory(Directory);
                    SftpItemCollection currentItems = sftp.GetList();
                    if (currentItems.Count > 0)
                    {
                        foreach (var fileObj in currentItems)
                        {
                            if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(Extension))
                            {
                                String filePath = Directory + "/" + fileObj.Name;
                                CreateStreamReader(receiveData, sftp, fileObj, filePath);
                                AfterImport(sftp, fileObj, filePath);
                            }
                        }
                    }
                    CloseConnection(sftp);
                }
            }
            else
            {
                var ftp = new Rebex.Net.Ftp();

                EstablishConnection(ftp);
                if (ftp.GetConnectionState().Connected)
                {
                    GetServerInfo(ftp);
                    ftp.ChangeDirectory(Directory);
                    FtpList currentItems = ftp.GetList();
                    if (currentItems.Count > 0)
                    {
                        foreach (var fileObj in currentItems)
                        {
                            if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(Extension))
                            {
                                String filePath = Directory + "/" + fileObj.Name;
                                CreateStreamReader(receiveData, ftp, fileObj, filePath);
                                AfterImport(ftp, fileObj, filePath);
                            }
                        }
                    }
                    CloseConnection(ftp);
                }
            }






        }

        private void GetServerInfo(Ftp ftp)
        {
            ServerInfo.ServerName = ftp.ServerName;
            ServerInfo.ServerPort = ftp.ServerPort;
        }

        private void GetServerInfo(Sftp sftp)
        {
            ServerInfo.ServerName = sftp.ServerName;
            ServerInfo.ServerPort = sftp.ServerPort;
        }

        private static void CreateStreamReader(Action<IImportedData> receiveData, Sftp sftp, SftpItem fileObj, String filePath)
        {
            var stream = new MemoryStream();
            sftp.GetFile(filePath, stream);
            byte[] data = stream.ToArray();
            using (var ms = stream)
            {
                ms.Position = 0;
                var sr = new StreamReader(ms);
                receiveData(new StreamReaderImportedData()
                {
                    StreamReader = new StreamReader(ms),
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size
                });
            }
        }

        private static void CreateStreamReader(Action<IImportedData> receiveData, Ftp ftp, FtpItem fileObj, String filePath)
        {
            var stream = new MemoryStream();
            ftp.GetFile(filePath, stream);
            byte[] data = stream.ToArray();
            using (var ms = stream)
            {
                ms.Position = 0;
                var sr = new StreamReader(ms);
                receiveData(new StreamReaderImportedData()
                {
                    StreamReader = new StreamReader(ms),
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size
                });
            }
        }

        private static void CloseConnection(Ftp ftp)
        {
            ftp.Dispose();
        }

        private static void CloseConnection(Sftp sftp)
        {
            sftp.Dispose();
        }

        private void EstablishConnection(Ftp ftp)
        {
            ftp.Connect(ServerIP);
            ftp.Login(UserName, Password);
        }

        private void EstablishConnection(Sftp sftp)
        {
            sftp.Connect(ServerIP);
            sftp.Login(UserName, Password);
        }

        private void AfterImport(Sftp sftp, SftpItem fileObj, String filePath)
        {
            if (ActionAfterImport == Actions.Rename)
            {
                sftp.Rename(filePath, filePath.Replace(Extension, ".Imported"));
            }
            else if (ActionAfterImport == Actions.Delete)
            {
                sftp.DeleteFile(filePath);
            }
            else if (ActionAfterImport == Actions.Move)
            {
                if (!sftp.DirectoryExists(DirectorytoMoveFile))
                    sftp.CreateDirectory(DirectorytoMoveFile);

                sftp.Rename(filePath, DirectorytoMoveFile + "/" + fileObj.Name);
            }
        }

        private void AfterImport(Ftp ftp, FtpItem fileObj, String filePath)
        {
            if (ActionAfterImport == Actions.Rename)
            {
                ftp.Rename(filePath, filePath.Replace(Extension, ".Imported"));
            }
            else if (ActionAfterImport == Actions.Delete)
            {
                ftp.DeleteFile(filePath);
            }
            else if (ActionAfterImport == Actions.Move)
            {
                if (!ftp.DirectoryExists(DirectorytoMoveFile))
                    ftp.CreateDirectory(DirectorytoMoveFile);

                ftp.Rename(filePath, DirectorytoMoveFile + "/" + fileObj.Name);
            }
        }





    }
}
