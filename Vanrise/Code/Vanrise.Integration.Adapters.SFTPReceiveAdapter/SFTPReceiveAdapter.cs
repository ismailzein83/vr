﻿using Rebex.Net;
using System;
using System.IO;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SFTPReceiveAdapter
{
    public class SFTPReceiveAdapter : BaseReceiveAdapter
    {
        #region Private Functions

        private void CreateStreamReader(Action<IImportedData> receiveData, Sftp sftp, SftpItem fileObj, String filePath)
        {
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
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
                base.LogVerbose("Renaming file {0} after import", fileObj.Name);

                sftp.Rename(filePath, string.Format(@"{0}_{1}.processed", filePath.ToLower().Replace(ftpAdapterArgument.Extension.ToLower(), ""), Guid.NewGuid()));
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", fileObj.Name);

                sftp.DeleteFile(filePath);
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Move)
            {
                base.LogVerbose("Moving file {0} after import", fileObj.Name);

                if (!sftp.DirectoryExists(ftpAdapterArgument.DirectorytoMoveFile))
                    sftp.CreateDirectory(ftpAdapterArgument.DirectorytoMoveFile);

                sftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + string.Format(@"{0}_{1}.processed", filePath.ToLower().Replace(ftpAdapterArgument.Extension.ToLower(), ""), Guid.NewGuid()));

            }
        }


        #endregion

        public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {

            FTPAdapterArgument ftpAdapterArgument = argument as FTPAdapterArgument;

            var sftp = new Rebex.Net.Sftp();

            base.LogVerbose("Establishing SFTP Connection");

            EstablishConnection(sftp, ftpAdapterArgument);
            if (sftp.GetConnectionState().Connected)
            {
                base.LogVerbose("FTP connection is established");

                sftp.ChangeDirectory(ftpAdapterArgument.Directory);
                SftpItemCollection currentItems = sftp.GetList();


                if (!sftp.DirectoryExists(ftpAdapterArgument.Directory))
                    base.LogInformation("Directory {0} not found !!", ftpAdapterArgument.Directory);


                base.LogInformation("{0} files are ready to be imported", currentItems.Count);

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
            else
            {
                base.LogError("Could not ftp connect to server {0}", ftpAdapterArgument.ServerIP);
                throw new Exception("FTP adapter could not connect to FTP Server");
            }

        }

    }
}
