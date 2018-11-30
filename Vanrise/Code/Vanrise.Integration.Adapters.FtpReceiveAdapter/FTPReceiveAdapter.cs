using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Rebex.Net;
using Vanrise.Common;
using Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Integration.Business;

namespace Vanrise.Integration.Adapters.FTPReceiveAdapter
{
    public class FTPReceiveAdapter : BaseFileReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            FTPAdapterArgument ftpAdapterArgument = context.AdapterArgument as FTPAdapterArgument;
            if (ftpAdapterArgument.ActionAfterImport.HasValue && ftpAdapterArgument.ActionAfterImport.Value == (int)FTPAdapterArgument.Actions.NoAction
                && ftpAdapterArgument.FileCheckCriteria != FTPAdapterArgument.FileCheckCriteriaEnum.DateAndNameCheck)
                throw new VRBusinessException(string.Format("Check By Date and Name is required in case of no action after import"));

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
                FtpList ftpListToProccess = CheckAndGetFinalFtpCollection(ftpAdapterArgument, ftp, ftpList);
                if (ftpListToProccess != null && ftpListToProccess.Count > 0)
                {
                    Dictionary<string, List<DataSourceImportedBatch>> dataSourceImportedBatchByFileNames = null;

                    FileDataSourceDefinition fileDataSourceDefinition = base.GetFileDataSourceDefinition(ftpAdapterArgument.FileDataSourceDefinitionId);
                    if (fileDataSourceDefinition != null)
                        dataSourceImportedBatchByFileNames = base.GetDataSourceImportedBatchByFileNames(context.DataSourceId, fileDataSourceDefinition.DuplicateCheckInterval);

                    short numberOfFilesRead = 0;
                    bool newFilesStarted = false;

                    IEnumerable<FtpItem> ftpItems = null;
                    switch (ftpAdapterArgument.FileCheckCriteria)
                    {
                        case Arguments.FTPAdapterArgument.FileCheckCriteriaEnum.DateAndNameCheck:
                            ftpItems = ftpListToProccess.OrderBy(c => c.Modified).ThenBy(c => c.Name); break;
                        case Arguments.FTPAdapterArgument.FileCheckCriteriaEnum.NameCheck:
                            ftpItems = ftpListToProccess.OrderBy(c => c.Name); break;
                        default:
                            ftpItems = ftpListToProccess; break;
                    }

                    foreach (var fileObj in ftpItems)
                    {
                        if (context.ShouldStopImport())
                            break;

                        if (!fileObj.IsDirectory && regEx.IsMatch(fileObj.Name))
                        {
                            switch (ftpAdapterArgument.FileCheckCriteria)
                            {
                                case FTPAdapterArgument.FileCheckCriteriaEnum.DateAndNameCheck:
                                    {
                                        if (!newFilesStarted)
                                        {
                                            if (DateTime.Compare(ftpAdapterState.LastRetrievedFileTime, fileObj.Modified) > 0)
                                            {
                                                continue;
                                            }
                                            else if (DateTime.Compare(ftpAdapterState.LastRetrievedFileTime, fileObj.Modified) == 0)
                                            {
                                                if (!string.IsNullOrEmpty(ftpAdapterState.LastRetrievedFileName) && ftpAdapterState.LastRetrievedFileName.CompareTo(fileObj.Name) >= 0)
                                                    continue;
                                            }
                                            newFilesStarted = true;
                                        }

                                        break;
                                    }
                                case FTPAdapterArgument.FileCheckCriteriaEnum.NameCheck:
                                    {
                                        if (!newFilesStarted)
                                        {
                                            if (!string.IsNullOrEmpty(ftpAdapterState.LastRetrievedFileName) && ftpAdapterState.LastRetrievedFileName.CompareTo(fileObj.Name) >= 0)
                                                continue;
                                            newFilesStarted = true;
                                        }

                                        break;
                                    }
                                default: break;
                            }

                            if (!string.IsNullOrEmpty(ftpAdapterArgument.LastImportedFile) && ftpAdapterArgument.LastImportedFile.CompareTo(fileObj.Name) >= 0)
                                continue;

                            bool isDuplicateSameSize = false;
                            BatchState fileState = BatchState.Normal;
                            string filePath = ftpAdapterArgument.Directory + "/" + fileObj.Name;

                            if (fileDataSourceDefinition != null)
                            {
                                base.CheckMissingFiles(fileDataSourceDefinition.FileMissingChecker, fileObj.Name, ftpAdapterState.LastRetrievedFileName, context.OnDataReceived);

                                if (base.IsDuplicate(fileObj.Name, fileObj.Size, dataSourceImportedBatchByFileNames, out isDuplicateSameSize))
                                    fileState = BatchState.Duplicated;
                                else if (base.IsDelayed(fileDataSourceDefinition.FileDelayChecker, ftpAdapterState.LastRetrievedFileTime))
                                    fileState = BatchState.Delayed;
                            }

                            ImportedBatchProcessingOutput output = null;

                            if (fileState != BatchState.Duplicated)
                            {
                                output = CreateStreamReader(context.OnDataReceived, ftp, fileObj, filePath, ftpAdapterArgument, fileState);
                            }
                            else
                            {
                                output = context.OnDataReceived(new StreamReaderImportedData()
                                {
                                    Modified = fileObj.Modified,
                                    Name = fileObj.Name,
                                    Size = fileObj.Size,
                                    BatchState = fileState,
                                    IsDuplicateSameSize = isDuplicateSameSize
                                });
                            }

                            ftpAdapterState = SaveOrGetAdapterState(context, ftpAdapterArgument, fileObj.Name, fileObj.Modified);

                            AfterImport(ftp, fileObj, filePath, ftpAdapterArgument, output, fileState);

                            numberOfFilesRead++;

                            if (ftpAdapterArgument.NumberOfFiles.HasValue && ftpAdapterArgument.NumberOfFiles.Value == numberOfFilesRead)
                            {
                                base.LogInformation("Max number of files {0} reached", numberOfFilesRead);
                                break;
                            }
                        }
                    }

                    base.LogInformation("{0} files have been imported", numberOfFilesRead);
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

        private FTPAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, FTPAdapterArgument ftpAdapterArgument, string fileName = null, DateTime? fileModifiedDate = null)
        {
            FTPAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as FTPAdapterState;

                if (adapterState == null)
                    adapterState = new FTPAdapterState();

                if (fileModifiedDate.HasValue)
                {
                    adapterState.LastRetrievedFileTime = fileModifiedDate.Value;
                }

                if (!string.IsNullOrEmpty(fileName))
                    adapterState.LastRetrievedFileName = fileName;

                return adapterState;
            });

            return adapterState;
        }

        private ImportedBatchProcessingOutput CreateStreamReader(Func<IImportedData, ImportedBatchProcessingOutput> onDataReceived, Ftp ftp, FtpItem fileObj, String filePath,
            FTPAdapterArgument argument, BatchState fileState)
        {
            ImportedBatchProcessingOutput output = null;
            base.LogVerbose("Creating stream reader for file with name {0}", fileObj.Name);
            var stream = new MemoryStream();
            ftp.GetFile(filePath, stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var ms = GetStream(stream, argument.CompressedFiles, argument.CompressionType))
            {
                ms.Position = 0;
                output = onDataReceived(new StreamReaderImportedData()
                {
                    Stream = ms,
                    Modified = fileObj.Modified,
                    Name = fileObj.Name,
                    Size = fileObj.Size,
                    BatchState = fileState
                });
            }
            stream.Close();
            return output;
        }

        private MemoryStream GetStream(MemoryStream stream, bool isCompressed, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument.CompressionTypes compressionType)
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

        private void EstablishConnection(Ftp ftp, FTPAdapterArgument ftpAdapterArgument)
        {
            ftp.Connect(ftpAdapterArgument.ServerIP);
            ftp.Login(ftpAdapterArgument.UserName, ftpAdapterArgument.Password);
        }

        private void CloseConnection(Ftp ftp)
        {
            ftp.Dispose();
        }

        private void AfterImport(Ftp ftp, FtpItem fileObj, String filePath, FTPAdapterArgument ftpAdapterArgument, ImportedBatchProcessingOutput output, BatchState fileState)
        {
            if (fileState == BatchState.Duplicated && !string.IsNullOrEmpty(ftpAdapterArgument.DuplicateFilesDirectory))
            {
                MoveFile(ftp, fileObj, filePath, ftpAdapterArgument.DuplicateFilesDirectory, ftpAdapterArgument.Extension, "duplicate");
            }
            else if (output != null && output.MappingOutput.Result == MappingResult.Invalid && !string.IsNullOrEmpty(ftpAdapterArgument.InvalidFilesDirectory))
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

        private void MoveFile(Ftp ftp, FtpItem fileObj, String filePath, string directorytoMoveFile, string extension, string newExtension)
        {
            base.LogVerbose("Moving file {0} after import to Directory {1}", fileObj.Name, directorytoMoveFile);
            if (!ftp.DirectoryExists(directorytoMoveFile))
                ftp.CreateDirectory(directorytoMoveFile);
            ftp.Rename(filePath, directorytoMoveFile + "/" + string.Format(@"{0}.{1}", fileObj.Name.Replace(extension, ""), newExtension));
        }

        private FtpList CheckAndGetFinalFtpCollection(FTPAdapterArgument ftpAdapterArgument, Ftp ftp, FtpList ftpList)
        {
            if (ftpList == null || ftpList.Count == 0)
                return null;

            FtpList ftpListToPreccess = new FtpList();
            if (ftpAdapterArgument.FileCompletenessCheckInterval.HasValue)
            {
                Dictionary<string, FtpItem> firstReadFilesByName = ftpList.ToDictionary(itm => itm.Name, itm => itm);
                Thread.Sleep(ftpAdapterArgument.FileCompletenessCheckInterval.Value * 1000);
                FtpList currentItems = ftp.GetList(string.Format("*{0}", ftpAdapterArgument.Extension));
                foreach (var ftpFile in currentItems.OrderBy(c => c.Modified))
                {
                    FtpItem fileReadFirstTime = firstReadFilesByName.GetRecord(ftpFile.Name);
                    if (fileReadFirstTime == null || fileReadFirstTime.Size != ftpFile.Size)
                        break;

                    ftpListToPreccess.Add(ftpFile);
                }
            }
            else
            {
                ftpListToPreccess = ftpList;
            }

            return ftpListToPreccess.Count > 0 ? ftpListToPreccess : null;
        }

        #endregion
    }
}