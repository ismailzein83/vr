using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

            FileAdapterState fileAdapterState = SaveOrGetAdapterState(context, fileAdapterArgument);

            string mask = string.IsNullOrEmpty(fileAdapterArgument.Mask) ? "" : fileAdapterArgument.Mask;
            Regex regEx = new Regex(mask);
            base.LogVerbose("Checking the following directory {0}", fileAdapterArgument.Directory);

            if (System.IO.Directory.Exists(fileAdapterArgument.Directory))
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(fileAdapterArgument.Directory);//Assuming Test is your Folder
                    base.LogVerbose("Getting all files with extenstion {0}", fileAdapterArgument.Extension);
                    FileInfo[] fileInfos = d.GetFiles("*" + fileAdapterArgument.Extension); //Getting Text files

                    if (fileInfos == null || fileInfos.Length == 0)
                        return;

                    Dictionary<string, List<DataSourceImportedBatch>> dataSourceImportedBatchByFileNames = null;

                    FileDataSourceDefinition fileDataSourceDefinition = base.GetFileDataSourceDefinition(fileAdapterArgument.FileDataSourceDefinitionId);
                    if (fileDataSourceDefinition != null)
                        dataSourceImportedBatchByFileNames = base.GetDataSourceImportedBatchByFileNames(context.DataSourceId, fileDataSourceDefinition.DuplicateCheckInterval);

                    short numberOfFilesRead = 0;
                    bool newFilesStarted = false;

                    foreach (FileInfo file in fileInfos.OrderBy(c => c.LastWriteTime).ThenBy(c => c.Name))
                    {
                        if (context.ShouldStopImport())
                            break;

                        if (regEx.IsMatch(file.Name))
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

                            bool isDuplicateSameSize = false;
                            BatchState fileState = BatchState.Normal;
                            String filePath = fileAdapterArgument.Directory + "/" + file.Name;

                            if (fileDataSourceDefinition != null)
                            {
                                base.CheckMissingFiles(fileDataSourceDefinition.FileMissingChecker, file.Name, fileAdapterState.LastRetrievedFileName, context.OnDataReceived);

                                if (base.IsDuplicate(file.Name, file.Length, dataSourceImportedBatchByFileNames, out isDuplicateSameSize))
                                    fileState = BatchState.Duplicated;
                                else if (base.IsDelayed(fileDataSourceDefinition.FileDelayChecker, fileAdapterState.LastRetrievedFileTime))
                                    fileState = BatchState.Delayed;
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

                            AfterImport(fileAdapterArgument, file);

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

        private void AfterImport(FileAdapterArgument fileAdapterArgument, FileInfo file)
        {
            if (fileAdapterArgument.ActionAfterImport == (int)Actions.Rename)
            {
                base.LogVerbose("Renaming file {0} after import", file.Name);
                file.MoveTo(Path.Combine(file.DirectoryName, string.Format(@"{0}_{1}.processed", file.Name.ToLower().Replace(fileAdapterArgument.Extension.ToLower(), ""), Guid.NewGuid())));
            }
            else if (fileAdapterArgument.ActionAfterImport == (int)Actions.Delete)
            {
                base.LogVerbose("Deleting file {0} after import", file.Name);
                file.Delete();
            }
            else if (fileAdapterArgument.ActionAfterImport == (int)Actions.Move)
            {
                base.LogVerbose("Moving file {0} after import to Directory {1}", file.Name, fileAdapterArgument.DirectorytoMoveFile);
                if (System.IO.Directory.Exists(fileAdapterArgument.Directory))
                {

                    file.MoveTo(Path.Combine(fileAdapterArgument.DirectorytoMoveFile, string.Format(@"{0}.processed", file.Name)));
                }
            }
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