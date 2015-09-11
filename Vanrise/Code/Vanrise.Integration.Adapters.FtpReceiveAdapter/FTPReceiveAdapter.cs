using Rebex.Net;
using System;
using System.IO;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FTPReceiveAdapter
{
    public class FTPReceiveAdapter : BaseReceiveAdapter
    {
        #region Private Functions

        private void CreateStreamReader(Action<IImportedData> receiveData, Ftp ftp, FtpItem fileObj, String filePath)
        {
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
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

        private void EstablishConnection(Ftp ftp, FTPAdapterArgument ftpAdapterArgument)
        {
            ftp.Connect(ftpAdapterArgument.ServerIP);
            ftp.Login(ftpAdapterArgument.UserName, ftpAdapterArgument.Password);
        }

        private void AfterImport(Ftp ftp, FtpItem fileObj, String filePath, FTPAdapterArgument ftpAdapterArgument)
        {
            if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Rename)
            {
                base.LogVerbose("Renaming file {0} after import", fileObj.Name);
                ftp.Rename(filePath, filePath.ToLower().Replace(ftpAdapterArgument.Extension.ToLower(), ".Imported"));
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", fileObj.Name);
                ftp.DeleteFile(filePath);
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Move)
            {
                base.LogVerbose("Moving file {0} after import", fileObj.Name);
                if (!ftp.DirectoryExists(ftpAdapterArgument.DirectorytoMoveFile))
                    ftp.CreateDirectory(ftpAdapterArgument.DirectorytoMoveFile);

                ftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + fileObj.Name);
            }
        }
        #endregion

        public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {
            FTPAdapterArgument ftpAdapterArgument = argument as FTPAdapterArgument;
            var ftp = new Rebex.Net.Ftp();

            base.LogVerbose("Establishing FTP Connection");

            EstablishConnection(ftp, ftpAdapterArgument);
            if (ftp.GetConnectionState().Connected)
            {
                base.LogVerbose("FTP connection is established");
                ftp.ChangeDirectory(ftpAdapterArgument.Directory);
                FtpList currentItems = ftp.GetList();
                base.LogInformation("{0} files are ready to be imported", currentItems.Count);
                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(ftpAdapterArgument.Extension))
                        {
                            String filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;
                            CreateStreamReader(receiveData, ftp, fileObj, filePath);
                            AfterImport(ftp, fileObj, filePath, ftpAdapterArgument);
                        }
                    }
                }
                CloseConnection(ftp);
            }
            else
            {
                base.LogError("Could not ftp connect to server {0}", ftpAdapterArgument.ServerIP);
                throw new Exception("FTP adapter could not connect to FTP Server");
            }
        }

    }
}
