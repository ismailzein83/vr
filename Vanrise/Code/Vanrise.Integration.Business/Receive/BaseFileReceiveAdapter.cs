using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public abstract class BaseFileReceiveAdapter : BaseReceiveAdapter
    {
        protected FileDataSourceDefinition GetFileDataSourceDefinition(Guid? fileDataSourceDefinitionId)
        {
            if (!fileDataSourceDefinitionId.HasValue)
                return null;

            return new ConfigManager().GetFileDataSourceDefinition(fileDataSourceDefinitionId.Value);
        }

        protected Dictionary<string, List<DataSourceImportedBatch>> GetDataSourceImportedBatchByFileNames(Guid dataSourceId, TimeSpan duplicateCheckInterval)
        {
            DateTime from = DateTime.Now.Subtract(duplicateCheckInterval);
            List<DataSourceImportedBatch> dataSourceImportedBatches = new DataSourceImportedBatchManager().GetDataSourceImportedBatches(dataSourceId, from);
            if (dataSourceImportedBatches == null || dataSourceImportedBatches.Count == 0)
                return null;

            Dictionary<string, List<DataSourceImportedBatch>> results = new Dictionary<string, List<DataSourceImportedBatch>>();

            foreach (var dataSourceImportedBatch in dataSourceImportedBatches)
            {
                if (dataSourceImportedBatch.BatchState == BatchState.Missing)
                    continue;

                if (dataSourceImportedBatch.MappingResult == MappingResult.Empty || dataSourceImportedBatch.MappingResult == MappingResult.Invalid)
                    continue;

                List<DataSourceImportedBatch> currentDataSourceImportedBatches = results.GetOrCreateItem(dataSourceImportedBatch.BatchDescription, () =>
                {
                    return new List<DataSourceImportedBatch>();
                });
                currentDataSourceImportedBatches.Add(dataSourceImportedBatch);
            }

            return results.Count > 0 ? results : null;
        }

        protected void CheckMissingFiles(FileMissingChecker fileMissingChecker, string fileName, string lastRetrievedFileName, Func<IImportedData, ImportedBatchProcessingOutput> onDataReceived)
        {
            if (fileMissingChecker == null)
                return;

            var checkMissingFilesContext = new CheckMissingFilesContext() { CurrentFileName = fileName, LastRetrievedFileName = lastRetrievedFileName };
            fileMissingChecker.CheckMissingFiles(checkMissingFilesContext);
            if (checkMissingFilesContext.MissingFileNames != null)
            {
                foreach (string missingFileName in checkMissingFilesContext.MissingFileNames)
                    onDataReceived(new StreamReaderImportedData() { Name = missingFileName, Size = 0, BatchState = BatchState.Missing });
            }
        }

        protected bool IsDuplicate(string fileName, long fileSize, Dictionary<string, List<DataSourceImportedBatch>> dataSourceImportedBatchByFileNames, out bool isDuplicateSameSize)
        {
            isDuplicateSameSize = false;

            if (dataSourceImportedBatchByFileNames == null)
                return false;

            List<DataSourceImportedBatch> dataSourceImportedBatches;
            if (!dataSourceImportedBatchByFileNames.TryGetValue(fileName, out dataSourceImportedBatches))
                return false;

            foreach (var dataSourceImportedBatch in dataSourceImportedBatches)
            {
                if (fileSize == dataSourceImportedBatch.BatchSize)
                {
                    isDuplicateSameSize = true;
                    break;
                }
            }

            return true;
        }

        protected bool IsDelayed(FileDelayChecker fileDelayChecker, DateTime lastRetrievedFileTime)
        {
            if (fileDelayChecker == null)
                return false;

            return fileDelayChecker.IsDelayed(new FileDelayCheckerIsDelayedContext() { LastRetrievedFileTime = lastRetrievedFileTime });
        }
    }
}