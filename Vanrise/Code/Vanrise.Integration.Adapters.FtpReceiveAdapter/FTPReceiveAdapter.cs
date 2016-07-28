﻿using Rebex.Net;
using System;
using System.IO;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;
using System.Text.RegularExpressions;

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

                ftp.Rename(filePath, string.Format(@"{0}_{1}.processed", filePath.ToLower().Replace(ftpAdapterArgument.Extension.ToLower(), ""), Guid.NewGuid()));
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", fileObj.Name);
                ftp.DeleteFile(filePath);
            }
            else if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Move)
            {
                base.LogVerbose("Moving file {0} after import to Directory {1}", fileObj.Name, ftpAdapterArgument.DirectorytoMoveFile, Guid.NewGuid());
                if (!ftp.DirectoryExists(ftpAdapterArgument.DirectorytoMoveFile))
                    ftp.CreateDirectory(ftpAdapterArgument.DirectorytoMoveFile);

                ftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + string.Format(@"{0}.processed", fileObj.Name));
            }
        }
        #endregion

        public override void ImportData(IAdapterImportDataContext context)
        {
            FTPAdapterArgument ftpAdapterArgument = context.AdapterArgument as FTPAdapterArgument;
            var ftp = new Rebex.Net.Ftp();
            string mask = string.IsNullOrEmpty(ftpAdapterArgument.Mask) ? "" : ftpAdapterArgument.Mask;
            Regex regEx = new Regex(mask);

            base.LogVerbose("Establishing FTP Connection");

            EstablishConnection(ftp, ftpAdapterArgument);
            if (ftp.GetConnectionState().Connected)
            {
                base.LogVerbose("FTP connection is established");

                if (!ftp.DirectoryExists(ftpAdapterArgument.Directory))
                {
                    base.LogInformation("Directory {0} not found !!", ftpAdapterArgument.Directory);
                    throw new DirectoryNotFoundException();
                }

                ftp.ChangeDirectory(ftpAdapterArgument.Directory);
                FtpList currentItems = ftp.GetList(string.Format("*{0}", ftpAdapterArgument.Extension));

                base.LogInformation("{0} files are ready to be imported", currentItems.Count);
                if (currentItems.Count > 0)
                {
                    foreach (var fileObj in currentItems)
                    {
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            String filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;
                            CreateStreamReader(context.OnDataReceived, ftp, fileObj, filePath);
                            AfterImport(ftp, fileObj, filePath, ftpAdapterArgument);
                        }
                    }
                }
                CloseConnection(ftp);
            }
            else
            {
                base.LogError("Could not find Directory {0}", ftpAdapterArgument.Directory);
                throw new Exception("FTP adapter could not connect to FTP Server");
            }
        }
    }
}
