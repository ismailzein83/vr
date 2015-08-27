using Rebex.Net;
using System;
using System.IO;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SFTPReceiveAdapter
{
    public class SFTPReceiveAdapter : BaseReceiveAdapter
    {
        #region Private Functions

        private static void CreateStreamReader(Func<IImportedData, bool> receiveData, Sftp sftp, SftpItem fileObj, String filePath)
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
        
            private static void CloseConnection(Sftp sftp)
            {
                sftp.Dispose();
            }

            private void EstablishConnection(Sftp sftp, FTPAdapterArgument ftpAdapterArgument)
            {
                sftp.Connect(ftpAdapterArgument.ServerIP);
                sftp.Login(ftpAdapterArgument.UserName, ftpAdapterArgument.Password);
            }

            private void AfterImport(Sftp sftp, SftpItem fileObj, String filePath, FTPAdapterArgument ftpAdapterArgument)
            {
                if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Rename)
                {
                    sftp.Rename(filePath, filePath.Replace(ftpAdapterArgument.Extension, ".Imported"));
                }
                else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Delete)
                {
                    sftp.DeleteFile(filePath);
                }
                else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Move)
                {
                    if (!sftp.DirectoryExists(ftpAdapterArgument.DirectorytoMoveFile))
                        sftp.CreateDirectory(ftpAdapterArgument.DirectorytoMoveFile);

                    sftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + fileObj.Name);
                }
            }

       
        #endregion

            public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Func<IImportedData, bool> receiveData)
        {

            FTPAdapterArgument ftpAdapterArgument = argument as FTPAdapterArgument;

            var sftp = new Rebex.Net.Sftp();

            EstablishConnection(sftp, ftpAdapterArgument);
            if (sftp.GetConnectionState().Connected)
            {
                sftp.ChangeDirectory(ftpAdapterArgument.Directory);
                SftpItemCollection currentItems = sftp.GetList();
                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(ftpAdapterArgument.Extension))
                        {
                            String filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;
                            CreateStreamReader(receiveData, sftp, fileObj, filePath);
                            AfterImport(sftp, fileObj, filePath, ftpAdapterArgument);
                        }
                    }
                }
                CloseConnection(sftp);
            }

        }

    }
}
