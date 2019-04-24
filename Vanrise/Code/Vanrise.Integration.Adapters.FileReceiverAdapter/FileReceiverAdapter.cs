using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FileReceiveAdapter
{
    public class FileReceiveAdapter : BaseFileReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            FileAdapterArgument fileAdapterArgument = context.AdapterArgument as FileAdapterArgument;

            if (fileAdapterArgument.ActionAfterImport == (int)FileAdapterArgument.Actions.NoAction
                && fileAdapterArgument.FileCheckCriteria == FileAdapterArgument.FileCheckCriteriaEnum.None)
                throw new VRBusinessException(string.Format("'None' Check file criteria is not allowed in case of no action after import"));

            FileAdapterState fileAdapterState = SaveOrGetAdapterState(context, fileAdapterArgument);

            string mask = string.IsNullOrEmpty(fileAdapterArgument.Mask) ? "" : fileAdapterArgument.Mask;
            Regex regEx = new Regex(mask);
            base.LogVerbose("Checking the following directory {0}", fileAdapterArgument.Directory);

            if (System.IO.Directory.Exists(fileAdapterArgument.Directory))
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(fileAdapterArgument.Directory);//Assuming Test is your Folder
                    base.LogVerbose("Getting all files with extenstion {0}", fileAdapterArgument.Extension);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*" + fileAdapterArgument.Extension); //Getting Text files

                    List<FileInfo> fileInfosToProcess = CheckAndGetFinalFileCollection(fileAdapterArgument, directoryInfo, fileInfos.ToList());

                    if (fileInfosToProcess == null || fileInfosToProcess.Count == 0)
                        return;

                    Dictionary<string, List<DataSourceImportedBatch>> dataSourceImportedBatchByFileNames = null;

                    FileDataSourceDefinition fileDataSourceDefinition = base.GetFileDataSourceDefinition(fileAdapterArgument.FileDataSourceDefinitionId);
                    if (fileDataSourceDefinition != null && fileDataSourceDefinition.DuplicateCheckInterval.HasValue)
                        dataSourceImportedBatchByFileNames = base.GetDataSourceImportedBatchByFileNames(context.DataSourceId, fileDataSourceDefinition.DuplicateCheckInterval.Value);

                    short numberOfFilesRead = 0;
                    bool newFilesStarted = false;

                    IEnumerable<FileInfo> fileInfosOrderedToProcess = null;
                    switch (fileAdapterArgument.FileCheckCriteria)
                    {
                        case Arguments.FileAdapterArgument.FileCheckCriteriaEnum.NameCheck:
                            fileInfosOrderedToProcess = fileInfosToProcess.OrderBy(c => c.Name); break;
                        case Arguments.FileAdapterArgument.FileCheckCriteriaEnum.DateAndNameCheck:
                            fileInfosOrderedToProcess = fileInfosToProcess.OrderBy(c => c.LastWriteTime).ThenBy(c => c.Name); break;
                        default: fileInfosOrderedToProcess = fileInfosToProcess; break;
                    }

                    foreach (FileInfo file in fileInfosOrderedToProcess)
                    {
                        if (context.ShouldStopImport())
                            break;

                        if (regEx.IsMatch(file.Name))
                        {
                            switch (fileAdapterArgument.FileCheckCriteria)
                            {
                                case FileAdapterArgument.FileCheckCriteriaEnum.NameCheck:
                                    {
                                        if (!newFilesStarted)
                                        {
                                            if (!string.IsNullOrEmpty(fileAdapterState.LastRetrievedFileName) && fileAdapterState.LastRetrievedFileName.CompareTo(file.Name) >= 0)
                                                continue;
                                            newFilesStarted = true;
                                        }

                                        break;
                                    }

                                case FileAdapterArgument.FileCheckCriteriaEnum.DateAndNameCheck:
                                    {
                                        if (!newFilesStarted)
                                        {
                                            if (DateTime.Compare(fileAdapterState.LastRetrievedFileTime, file.LastWriteTime) > 0)
                                            {
                                                continue;
                                            }
                                            else if (DateTime.Compare(fileAdapterState.LastRetrievedFileTime, file.LastWriteTime) == 0)
                                            {
                                                if (!string.IsNullOrEmpty(fileAdapterState.LastRetrievedFileName) && fileAdapterState.LastRetrievedFileName.CompareTo(file.Name) >= 0)
                                                    continue;
                                            }
                                            newFilesStarted = true;
                                        }
                                        break;
                                    }
                                default: break;
                            }

                            bool? isDuplicateSameSize = null;
                            BatchState fileState = BatchState.Normal;
                            String filePath = fileAdapterArgument.Directory + "/" + file.Name;

                            if (fileDataSourceDefinition != null)
                            {
                                if (base.IsDuplicate(file.Name, file.Length, dataSourceImportedBatchByFileNames, out isDuplicateSameSize))
                                {
                                    fileState = BatchState.Duplicated;
                                }
                                else
                                {
                                    if (base.IsDelayed(fileDataSourceDefinition.FileDelayChecker, fileAdapterState.LastRetrievedFileTime))
                                        fileState = BatchState.Delayed;

                                    base.CheckMissingFiles(fileDataSourceDefinition.FileMissingChecker, file.Name, fileAdapterState.LastRetrievedFileName, context.OnDataReceived);
                                }
                            }

                            ImportedBatchProcessingOutput output = null;

                            if (fileState != BatchState.Duplicated)
                            {
                                output = CreateStreamReader(context.OnDataReceived, file, fileAdapterArgument, fileState);
                            }
                            else
                            {
                                output = context.OnDataReceived(new StreamReaderImportedData()
                                {
                                    Modified = file.LastWriteTime,
                                    Name = file.Name,
                                    Size = file.Length,
                                    BatchState = fileState,
                                    IsDuplicateSameSize = isDuplicateSameSize
                                });
                            }

                            fileAdapterState = SaveOrGetAdapterState(context, fileAdapterArgument, file.Name, file.LastWriteTime);

                            AfterImport(fileAdapterArgument, file, filePath, fileState);

                            numberOfFilesRead++;
                        }
                    }
                    base.LogInformation("{0} files have been imported", numberOfFilesRead);
                }
                catch (Exception ex)
                {
                    base.LogError("An error occurred in File Adapter while importing data. Exception Details: {0}", ex.ToString());
                }
            }
            else
            {
                base.LogError("Could not find Directory {0}", fileAdapterArgument.Directory);
                throw new DirectoryNotFoundException();
            }
        }

        #region Private Functions

        private FileAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, FileAdapterArgument fileAdapterArgument, string fileName = null, DateTime? fileModifiedDate = null)
        {
            FileAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as FileAdapterState;

                if (adapterState == null)
                    adapterState = new FileAdapterState();

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

        private List<FileInfo> CheckAndGetFinalFileCollection(FileAdapterArgument fileAdapterArgument, DirectoryInfo directoryInfo, List<FileInfo> fileList)
        {
            if (fileList == null || fileList.Count() == 0)
                return null;

            List<FileInfo> fileListToProccess = new List<FileInfo>();
            if (fileAdapterArgument.FileCompletenessCheckInterval.HasValue)
            {
                Dictionary<string, FileInfo> firstReadFilesByName = fileList.ToDictionary(itm => itm.Name, itm => itm);
                Thread.Sleep(fileAdapterArgument.FileCompletenessCheckInterval.Value * 1000);
                FileInfo[] currentItems = directoryInfo.GetFiles("*" + fileAdapterArgument.Extension);

                foreach (var file in currentItems.OrderBy(c => c.LastWriteTime))
                {
                    FileInfo fileReadFirstTime = firstReadFilesByName.GetRecord(file.Name);
                    if (fileReadFirstTime == null || fileReadFirstTime.Length != file.Length)
                        break;

                    fileListToProccess.Add(file);
                }
            }
            else
            {
                fileListToProccess = fileList;
            }

            return fileListToProccess.Count > 0 ? fileListToProccess : null;
        }

        private ImportedBatchProcessingOutput CreateStreamReader(Func<IImportedData, ImportedBatchProcessingOutput> onDataReceived, FileInfo file, FileAdapterArgument argument, BatchState fileState)
        {
            ImportedBatchProcessingOutput output = null;
            base.LogVerbose("Creating stream reader for file with name {0}", file.Name);

            StreamReaderImportedData data = new StreamReaderImportedData()
            {
                Stream = new FileStream(argument.Directory + "/" + file.Name, FileMode.Open),
                Modified = file.LastWriteTime,
                Name = file.Name,
                Size = file.Length,
                BatchState = fileState
            };

            output = onDataReceived(data);
            return output;
        }

        private void AfterImport(FileAdapterArgument fileAdapterArgument, FileInfo file, String filePath, BatchState fileState)
        {
            if (fileState == BatchState.Duplicated && !string.IsNullOrEmpty(fileAdapterArgument.DuplicatedFilesDirectory))
            {
                MoveFile(fileAdapterArgument, file, filePath, fileAdapterArgument.DuplicatedFilesDirectory, fileAdapterArgument.Extension, "duplicate");
                return;
            }

            if (fileAdapterArgument.ActionAfterImport == (int)Actions.Rename)
            {
                base.LogVerbose("Renaming file {0} after import", file.Name);

                string fileNameWithoutExtension = file.Name.ToLower().Replace(fileAdapterArgument.Extension.ToLower(), "");
                file.MoveTo(Path.Combine(file.DirectoryName, string.Format(@"{0}_{1}.processed", fileNameWithoutExtension, Guid.NewGuid())));
            }
            else if (fileAdapterArgument.ActionAfterImport == (int)Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", file.Name);
                file.Delete();
            }
            else if (fileAdapterArgument.ActionAfterImport == (int)Actions.Move)
            {
                MoveFile(fileAdapterArgument, file, filePath, fileAdapterArgument.DirectorytoMoveFile, fileAdapterArgument.Extension, "processed");
            }
        }

        private void MoveFile(FileAdapterArgument fileAdapterArgument, FileInfo file, String filePath, string directorytoMoveFile, string extension, string newExtension)
        {
            base.LogVerbose("Moving file {0} after import to Directory {1}", file.Name, directorytoMoveFile);

            if (!System.IO.Directory.Exists(fileAdapterArgument.DirectorytoMoveFile))
                System.IO.Directory.CreateDirectory(fileAdapterArgument.DirectorytoMoveFile);

            string fileObjWithoutExtension = file.Name.Replace(extension, "");
            string newFilePath = Path.Combine(directorytoMoveFile, string.Format(@"{0}.{1}", fileObjWithoutExtension, newExtension));
            if (File.Exists(newFilePath))
                newFilePath = newFilePath.Replace(fileObjWithoutExtension, string.Format(@"{0}_{1}", fileObjWithoutExtension, Guid.NewGuid()));

            file.MoveTo(newFilePath);
        }

        #endregion

        public enum Actions
        {
            NoAction = -1,
            Rename = 0,
            Delete = 1,
            Move = 2 // Move to Folder
        }
    }
}