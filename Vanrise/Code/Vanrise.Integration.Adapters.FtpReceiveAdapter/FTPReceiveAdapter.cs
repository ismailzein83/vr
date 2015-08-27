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

        private static void CreateStreamReader(Func<IImportedData, bool> receiveData, Ftp ftp, FtpItem fileObj, String filePath)
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

        private void EstablishConnection(Ftp ftp, FTPAdapterArgument ftpAdapterArgument)
        {
            ftp.Connect(ftpAdapterArgument.ServerIP);
            ftp.Login(ftpAdapterArgument.UserName, ftpAdapterArgument.Password);
        }

        private void AfterImport(Ftp ftp, FtpItem fileObj, String filePath, FTPAdapterArgument ftpAdapterArgument)
        {
            if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Rename)
            {
                ftp.Rename(filePath, filePath.ToLower().Replace(ftpAdapterArgument.Extension.ToLower(), ".Imported"));
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Delete)
            {
                ftp.DeleteFile(filePath);
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Move)
            {
                if (!ftp.DirectoryExists(ftpAdapterArgument.DirectorytoMoveFile))
                    ftp.CreateDirectory(ftpAdapterArgument.DirectorytoMoveFile);

                ftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + fileObj.Name);
            }
        }
        #endregion

        public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Func<IImportedData, bool> receiveData)
        {
            FTPAdapterArgument ftpAdapterArgument = argument as FTPAdapterArgument;
            var ftp = new Rebex.Net.Ftp();

            EstablishConnection(ftp, ftpAdapterArgument);
            if (ftp.GetConnectionState().Connected)
            {
                ftp.ChangeDirectory(ftpAdapterArgument.Directory);
                FtpList currentItems = ftp.GetList();
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
        }

    }
}
