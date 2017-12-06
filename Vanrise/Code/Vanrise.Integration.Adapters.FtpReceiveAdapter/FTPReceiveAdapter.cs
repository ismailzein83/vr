using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
                FtpList ftpList = ftp.GetList(string.Format("*{0}", ftpAdapterArgument.Extension));

                base.LogInformation("{0} files are ready to be imported", ftpList.Count);
                if (ftpList.Count > 0)
                {
                    short numberOfFilesRead = 0;
                    DateTime? localLastRetrievedFileTime = null;

                    FtpList ftpListToPreccess = CheckandGetFinalFtpCollection(ftpAdapterArgument, ftp, ftpList);

                    foreach (var fileObj in ftpListToPreccess.OrderBy(c => c.Modified).ThenBy(c => c.Name))
                    {

                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            if (ftpAdapterArgument.BasedOnLastModifiedTime)
                            {
                                if ((!localLastRetrievedFileTime.HasValue || DateTime.Compare(localLastRetrievedFileTime.Value, fileObj.Modified) != 0)
                                    && DateTime.Compare(ftpAdapterState.LastRetrievedFileTime, fileObj.Modified) >= 0)
                                {
                                    if (!string.IsNullOrEmpty(ftpAdapterState.LastRetrievedFileName) && ftpAdapterState.LastRetrievedFileName.CompareTo(fileObj.Name) >= 0)
                                        continue;
                                }
                            }

                            if (!string.IsNullOrEmpty(ftpAdapterArgument.LastImportedFile) && ftpAdapterArgument.LastImportedFile.CompareTo(fileObj.Name) >= 0)
                                continue;


                            ftpAdapterState.LastRetrievedFileName = fileObj.Name;
                            ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument, fileObj.Name);

                            String filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;
                            if (ftpAdapterState.LastRetrievedFileTime != fileObj.Modified)
                            {
                                ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument, fileObj.Name, fileObj.Modified);
                                localLastRetrievedFileTime = fileObj.Modified;
                            }

                            ImportedBatchProcessingOutput output = CreateStreamReader(context.OnDataReceived, ftp, fileObj, filePath, ftpAdapterArgument);
                            AfterImport(ftp, fileObj, filePath, ftpAdapterArgument, output);

                            numberOfFilesRead++;

                            if (ftpAdapterArgument.NumberOfFiles.HasValue && ftpAdapterArgument.NumberOfFiles.Value == numberOfFilesRead)
                            {
                                base.LogInformation("Max number of files {0} reached", numberOfFilesRead);
                                break;
                            }
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


        #region Private Functions

        FTPAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, FTPAdapterArgument ftpAdapterArgument, string fileName = "", DateTime? fileModifiedDate = null)
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

                if (!string.IsNullOrEmpty(fileName))
                    adapterState.LastRetrievedFileName = fileName;

                return adapterState;
            });

            return adapterState;
        }

        ImportedBatchProcessingOutput CreateStreamReader(Func<IImportedData, ImportedBatchProcessingOutput> receiveData, Ftp ftp, FtpItem fileObj, String filePath, FTPAdapterArgument argument)
        {
            ImportedBatchProcessingOutput output = null;
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
            var stream = new MemoryStream();
            ftp.GetFile(filePath, stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var ms = GetStream(stream, argument.CompressedFiles, argument.CompressionType))
            {
                ms.Position = 0;
                output = receiveData(new StreamReaderImportedData()
                     {
                         Stream = ms,
                         Modified = fileObj.Modified,
                         Name = fileObj.Name,
                         Size = fileObj.Size
                     });
            }
            stream.Close();
            return output;
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

        static void CloseConnection(Ftp ftp)
        {
            ftp.Dispose();
        }

        void EstablishConnection(Ftp ftp, FTPAdapterArgument ftpAdapterArgument)
        {
            ftp.Connect(ftpAdapterArgument.ServerIP);
            ftp.Login(ftpAdapterArgument.UserName, ftpAdapterArgument.Password);
        }

        void AfterImport(Ftp ftp, FtpItem fileObj, String filePath, FTPAdapterArgument ftpAdapterArgument, ImportedBatchProcessingOutput output)
        {
            if (output != null && output.OutputResult.Result == MappingResult.Invalid && !string.IsNullOrEmpty(ftpAdapterArgument.InvalidFilesDirectory))
            {
                MoveFile(ftp, fileObj, filePath, ftpAdapterArgument.InvalidFilesDirectory, ftpAdapterArgument.Extension, "invalid");

            }
            else if (ftpAdapterArgument.ActionAfterImport.HasValue)
            {
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
                    MoveFile(ftp, fileObj, filePath, ftpAdapterArgument.DirectorytoMoveFile, ftpAdapterArgument.Extension, "processed");
                }
            }
        }

        void MoveFile(Ftp ftp, FtpItem fileObj, String filePath, string directorytoMoveFile, string extension, string newExtension)
        {
            base.LogVerbose("Moving file {0} after import to Directory {1}", fileObj.Name, directorytoMoveFile);
            if (!ftp.DirectoryExists(directorytoMoveFile))
                ftp.CreateDirectory(directorytoMoveFile);
            ftp.Rename(filePath, directorytoMoveFile + "/" + string.Format(@"{0}.{1}", fileObj.Name.Replace(extension, ""), newExtension));
        }

        FtpList CheckandGetFinalFtpCollection(FTPAdapterArgument ftpAdapterArgument, Ftp ftp, FtpList ftpList)
        {
            FtpList ftpListToPreccess = new FtpList();
            if (ftpAdapterArgument.FileCompletenessCheckInterval.HasValue)
            {
                Thread.Sleep(ftpAdapterArgument.FileCompletenessCheckInterval.Value * 1000);
                FtpList currentItems = ftp.GetList(string.Format("*{0}", ftpAdapterArgument.Extension));
                foreach (var ftpFile in ftpList.OrderBy(c => c.Modified))
                {
                    FtpItem ftpItem = currentItems.FindRecord(itm => itm.Name == ftpFile.Name);
                    if (ftpItem.Size != ftpFile.Size)
                        break;

                    ftpListToPreccess.Add(ftpItem);
                }
            }
            else
            {
                ftpListToPreccess = ftpList;
            }
            return ftpListToPreccess;
        }

        #endregion

    }
}
