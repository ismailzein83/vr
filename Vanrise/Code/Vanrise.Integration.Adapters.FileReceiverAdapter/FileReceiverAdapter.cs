using System;
using System.IO;
using System.Text.RegularExpressions;
using Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.FileReceiveAdapter
{
    public class FileReceiveAdapter : BaseReceiveAdapter
    {

        public enum Actions
        {
            Rename = 0,
            Delete = 1,
            Move = 2 // Move to Folder
        }

        #region Private Functions

        private void CreateStreamReader(FileAdapterArgument fileAdapterArgument, Func<IImportedData, ImportedBatchProcessingOutput> receiveData, FileInfo file)
        {
            base.LogVerbose("Creating stream reader for file with name {0}", file.Name);
            StreamReaderImportedData data = new StreamReaderImportedData()
            {
                Stream = new FileStream(fileAdapterArgument.Directory + "/" + file.Name, FileMode.Open),
                Modified = file.LastWriteTime,
                Name = file.Name,
                Size = file.Length
            };

            receiveData(data);
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

        public override void ImportData(IAdapterImportDataContext context)
        {
            FileAdapterArgument fileAdapterArgument = context.AdapterArgument as FileAdapterArgument;
            string mask = string.IsNullOrEmpty(fileAdapterArgument.Mask) ? "" : fileAdapterArgument.Mask;
            Regex regEx = new Regex(mask);
            base.LogVerbose("Checking the following directory {0}", fileAdapterArgument.Directory);

            if (System.IO.Directory.Exists(fileAdapterArgument.Directory))
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(fileAdapterArgument.Directory);//Assuming Test is your Folder
                    base.LogVerbose("Getting all files with extenstion {0}", fileAdapterArgument.Extension);
                    FileInfo[] Files = d.GetFiles("*" + fileAdapterArgument.Extension); //Getting Text files
                    
                    short numberOfFilesRead = 0;
                    foreach (FileInfo file in Files)
                    {
                        if (context.ShouldStopImport())
                            break;

                        if (regEx.IsMatch(file.Name))
                        {
                            CreateStreamReader(fileAdapterArgument, context.OnDataReceived, file);
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
    }
}
