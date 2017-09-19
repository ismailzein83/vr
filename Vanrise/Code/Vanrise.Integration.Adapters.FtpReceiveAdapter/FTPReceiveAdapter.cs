using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Rebex.Net;
using Vanrise.Common;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FTPReceiveAdapter
{
    public class FTPReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            FTPAdapterArgument ftpAdapterArgument = context.AdapterArgument as FTPAdapterArgument;

            FTPAdapterState ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument);

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
                    DateTime? localLastRetrievedFileTime = null;
                    foreach (var fileObj in currentItems.OrderBy(c => c.Modified).ThenBy(c => c.Name))
                    {
                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            if (ftpAdapterArgument.BasedOnLastModifiedTime)
                            {
                                if ((!localLastRetrievedFileTime.HasValue || DateTime.Compare(localLastRetrievedFileTime.Value, fileObj.Modified) != 0)
                                    && DateTime.Compare(ftpAdapterState.LastRetrievedFileTime, fileObj.Modified) >= 0)
                                    continue;
                            }

                            if (!string.IsNullOrEmpty(ftpAdapterArgument.LastImportedFile) && ftpAdapterArgument.LastImportedFile.CompareTo(fileObj.Name) >= 0)
                                continue;

                            String filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;
                            if (ftpAdapterState.LastRetrievedFileTime != fileObj.Modified)
                            {
                                ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument, fileObj.Modified);
                                localLastRetrievedFileTime = fileObj.Modified;
                            }
                            CreateStreamReader(context.OnDataReceived, ftp, fileObj, filePath, ftpAdapterArgument);
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

        private FTPAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, FTPAdapterArgument ftpAdapterArgument, DateTime? fileModifiedDate = null)
        {
            FTPAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as FTPAdapterState;

                if (adapterState == null)
                    adapterState = new FTPAdapterState();

                if (fileModifiedDate != null && fileModifiedDate.HasValue)
                {
                    adapterState.LastRetrievedFileTime = fileModifiedDate.Value;
                }
                return adapterState;
            });

            return adapterState;
        }

        #region Private Functions

        private void CreateStreamReader(Action<IImportedData> receiveData, Ftp ftp, FtpItem fileObj, String filePath, FTPAdapterArgument argument)
        {
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
            var stream = new MemoryStream();
            ftp.GetFile(filePath, stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var ms = GetStream(stream, argument.CompressedFiles, argument.CompressionType))
            {
                ms.Position = 0;
                receiveData(new StreamReaderImportedData()
                {
                    Stream = ms,
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size
                });
            }
            stream.Close();

        }

        MemoryStream GetStream(MemoryStream stream, bool isCompressed, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.CompressionTypes compressionType)
        {
            if (isCompressed)
            {
                switch (compressionType)
                {
                    case Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.CompressionTypes.GZip:
                        return new MemoryStream(ZipUtility.DecompressGZ(stream));
                    case Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.CompressionTypes.Zip:
                        return new MemoryStream(ZipUtility.UnZip(stream.ToArray()));
                }
            }

            return stream;
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
            if (ftpAdapterArgument.ActionAfterImport.HasValue)
                if (ftpAdapterArgument.ActionAfterImport == (int)Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.Actions.Rename)
                {
                    base.LogVerbose("Renaming file {0} after import", fileObj.Name);

                    ftp.Rename(filePath, string.Format(@"{0}.processed", filePath.Replace(ftpAdapterArgument.Extension, "")));
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

                    ftp.Rename(filePath, ftpAdapterArgument.DirectorytoMoveFile + "/" + string.Format(@"{0}.processed", fileObj.Name.Replace(ftpAdapterArgument.Extension, "")));
                }
        }

        #endregion

    }
}
