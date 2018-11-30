using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceRuntimeService : RuntimeService
    {
        const int failedRecordIdentifiersThreshold = 100;

        DataSourceRuntimeInstanceManager _dsRuntimeInstanceManager = new DataSourceRuntimeInstanceManager();
        IDataSourceRuntimeInstanceDataManager _dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceRuntimeInstanceDataManager>();
        DataSourceManager _dataSourceManager = new DataSourceManager();

        public override Guid ConfigId { get { return new Guid("BDD330B6-3557-4AFC-8C69-A23890DA82E7"); } }

        public override void Execute()
        {
            List<DataSourceRuntimeInstance> dataSourceRuntimeInstances = _dataManager.GetAll();
            if (dataSourceRuntimeInstances != null && dataSourceRuntimeInstances.Count > 0)
            {
                IOrderedEnumerable<DataSourceRuntimeInstance> orderedDataSourceRuntimeInstances = dataSourceRuntimeInstances.OrderBy(itm => itm.CreatedTime);
                foreach (var dsRuntimeInstance in orderedDataSourceRuntimeInstances)
                {
                    bool isInstanceLockedAndExecuted = false;
                    TransactionLocker.Instance.TryLock(String.Concat("DataSourceRuntimeInstance_", dsRuntimeInstance.DataSourceRuntimeInstanceId),
                        () =>
                        {
                            if (_dataManager.IsStillExist(dsRuntimeInstance.DataSourceRuntimeInstanceId))
                            {
                                var dataSource = _dataSourceManager.GetDataSourceDetail(dsRuntimeInstance.DataSourceId);
                                if (dataSource == null)
                                    throw new ArgumentNullException(String.Format("dataSource '{0}'", dsRuntimeInstance.DataSourceId));

                                try
                                {
                                    if (dataSource.Entity.IsEnabled)
                                    {
                                        ExecuteDataSource(dataSource);
                                        isInstanceLockedAndExecuted = true;
                                    }
                                }
                                finally
                                {
                                    _dataManager.DeleteInstance(dsRuntimeInstance.DataSourceRuntimeInstanceId);
                                }
                            }
                        });
                    if (isInstanceLockedAndExecuted)
                        break;
                }
            }
        }

        private void ExecuteDataSource(DataSourceDetail dataSource)
        {
            DataSourceLogger logger = new DataSourceLogger();
            logger.DataSourceId = dataSource.Entity.DataSourceId;
            logger.WriteInformation("A new runtime instance started for the Data Source '{0}'", dataSource.Entity.Name);
            logger.WriteVerbose("Preparing queues and stages");

            QueuesByStages queuesByStages = null;
            try
            {
                Vanrise.Queueing.QueueExecutionFlowManager executionFlowManager = new Vanrise.Queueing.QueueExecutionFlowManager();
                queuesByStages = executionFlowManager.GetQueuesByStages(dataSource.Entity.Settings.ExecutionFlowId);

                if (queuesByStages == null || queuesByStages.Count == 0)
                {
                    logger.WriteError("No stages ready for use");
                    return;
                }
                else
                {
                    logger.WriteInformation("{0} stage(s) are ready for use", queuesByStages.Count);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError("An error occurred while preparing stages. Exception Details {0}", ex.ToString());
                throw;
            }

            logger.WriteVerbose("Preparing adapters to start the import process");

            BaseReceiveAdapter adapter = (BaseReceiveAdapter)Activator.CreateInstance(Type.GetType(dataSource.AdapterInfo.FQTN));
            adapter.SetLogger(logger);
            adapter.SetDataSourceManager(_dataSourceManager);

            IImportedData lastReceivedBatchData = null;
            Func<IImportedData, ImportedBatchProcessingOutput> onDataReceivedAction = (data) =>
            {
                bool logImportedBatchEntry = true;

                ImportedBatchEntry importedBatchEntry = new ImportedBatchEntry();
                importedBatchEntry.BatchSize = data.BatchSize;
                importedBatchEntry.BatchDescription = data.Description;
                importedBatchEntry.BatchState = data.BatchState;
                importedBatchEntry.IsDuplicateSameSize = data.IsDuplicateSameSize;
                importedBatchEntry.Result = MappingResult.None;

                MappingOutput mappingOutput = null;
                ImportedBatchProcessingOutput importedBatchProcessingOutput = null;

                if (data.BatchState != BatchState.Duplicated && data.BatchState != BatchState.Missing)
                {
                    logger.WriteInformation("New batch '{0}' imported", data.Description);
                    logger.WriteVerbose("Executing the custom code written for the mapper");

                    lastReceivedBatchData = data;
                    MappedBatchItemsToEnqueue outputItems = new MappedBatchItemsToEnqueue();
                    mappingOutput = this.ExecuteCustomCode(dataSource.Entity.DataSourceId, dataSource.Entity.Settings.MapperCustomCode, data, outputItems, logger);

                    importedBatchProcessingOutput = new ImportedBatchProcessingOutput();
                    importedBatchProcessingOutput.MappingOutput = mappingOutput;

                    importedBatchEntry.Result = mappingOutput.Result;
                    importedBatchEntry.MapperMessage = mappingOutput.Message;

                    if (!data.IsMultipleReadings)
                        data.OnDisposed();

                    DateTime? minBatchStart = null;
                    DateTime? maxBatchEnd = null;

                    if (mappingOutput.Result != MappingResult.Invalid)
                    {
                        List<long> queueItemsIds = new List<long>();
                        int totalRecordsCount = 0;

                        if (outputItems != null && outputItems.Count > 0)
                        {
                            foreach (var outputItem in outputItems)
                            {
                                try
                                {
                                    int currentBatchItemsCount = outputItem.Item.GetRecordCount();
                                    if (currentBatchItemsCount == 0)
                                        continue;

                                    outputItem.Item.DataSourceID = dataSource.Entity.DataSourceId;
                                    outputItem.Item.BatchDescription = data.Description;
                                    logger.WriteInformation("Enqueuing item '{0}' to stage '{1}'", outputItem.Item.GenerateDescription(), outputItem.StageName);

                                    long queueItemId = queuesByStages[outputItem.StageName].Queue.EnqueueObject(outputItem.Item);
                                    logger.WriteInformation("Enqueued the item successfully");

                                    queueItemsIds.Add(queueItemId);
                                    totalRecordsCount += currentBatchItemsCount;

                                    var batchStart = outputItem.Item.GetBatchStart();
                                    if (!minBatchStart.HasValue || batchStart < minBatchStart.Value)
                                        minBatchStart = batchStart;

                                    var batchEnd = outputItem.Item.GetBatchEnd();
                                    if (!maxBatchEnd.HasValue || batchEnd > maxBatchEnd)
                                        maxBatchEnd = batchEnd;

                                }
                                catch (Exception ex)
                                {
                                    logger.WriteError("An error occurred while enqueuing item in stage {0}. Exception details {1}", outputItem.StageName, ex.ToString());
                                    throw;
                                }
                            }

                            importedBatchEntry.BatchStart = minBatchStart;
                            importedBatchEntry.BatchEnd = maxBatchEnd;
                        }
                        else
                        {
                            if (data.IsFile)
                                logger.WriteWarning("No mapped items to enqueue, the written custom code should specify at least one output item to enqueue items to");
                        }

                        if (totalRecordsCount == 0)
                        {
                            importedBatchEntry.Result = MappingResult.Empty;

                            if (data.IsFile)
                            {
                                if (outputItems != null && outputItems.Count > 0)
                                    logger.WriteWarning("No mapped items to enqueue");
                            }
                            else
                            {
                                logger.WriteInformation("Received Empty Batch");
                                logImportedBatchEntry = false;
                            }
                        }

                        importedBatchEntry.QueueItemsIds = string.Join(",", queueItemsIds);
                        importedBatchEntry.RecordsCount = totalRecordsCount;
                    }
                }

                SendErrorNotification(dataSource, data, mappingOutput, importedBatchEntry.IsDuplicateSameSize);

                if (logImportedBatchEntry)
                {
                    long importedBatchId = logger.LogImportedBatchEntry(importedBatchEntry);
                    logger.LogEntry(Vanrise.Entities.LogEntryType.Information, importedBatchId, "Imported a new batch with Id '{0}'", importedBatchId);
                }

                return importedBatchProcessingOutput;
            };

            try
            {
                AdapterImportDataContext adapterContext = new AdapterImportDataContext(dataSource.Entity, onDataReceivedAction);
                adapter.ImportData(adapterContext);

                logger.WriteInformation("A runtime Instance is finished for the Data Source '{0}'", dataSource.Entity.Name);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Adapter failed due to error: {0}", ex.ToString());
                logger.WriteError(errorMessage);
                if (dataSource.Entity.Settings.ErrorMailTemplateId.HasValue)
                {
                    FailedBatchInfo batchInfo = new FailedBatchInfo
                    {
                        DataSourceId = dataSource.Entity.DataSourceId,
                        DataSourceName = dataSource.Entity.Name,
                        BatchDescription = lastReceivedBatchData != null ? lastReceivedBatchData.Description : null,
                        Message = errorMessage
                    };
                    SendErrorNotification(dataSource, batchInfo);
                }
                throw;
            }
        }

        private void SendErrorNotification(DataSourceDetail dataSource, IImportedData data, MappingOutput mappingOutput, bool isDuplicateSameSize)
        {
            if (dataSource.Entity.Settings.ErrorMailTemplateId.HasValue)
            {
                if (mappingOutput.Result == MappingResult.Invalid || mappingOutput.Result == MappingResult.PartialInvalid)
                {
                    SendErrorNotification(dataSource, data, mappingOutput, false, null);
                    return;
                }

                bool isEmpty = false;
                string errorMessage = null;

                if (data.IsFile)
                {
                    if (data.BatchState == BatchState.Duplicated && !isDuplicateSameSize)
                    {
                        errorMessage = "Duplicate File with different size";
                    }
                    else if (!data.BatchSize.HasValue || data.BatchSize.Value == 0)
                    {
                        isEmpty = true;
                        errorMessage = "File is empty";
                    }
                    else if (mappingOutput.Result == MappingResult.Empty)
                    {
                        isEmpty = true;
                        errorMessage = "No mapped items to enqueue";
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                    SendErrorNotification(dataSource, data, mappingOutput, isEmpty, errorMessage);
            }
        }

        private void SendErrorNotification(DataSourceDetail dataSource, IImportedData data, MappingOutput outputResult, bool isEmpty, string errorMessage)
        {
            FailedBatchInfo batchInfo = BuildFailedBatchInfo(dataSource, data, outputResult, isEmpty, errorMessage);
            SendErrorNotification(dataSource, batchInfo);
        }

        private void SendErrorNotification(DataSourceDetail dataSource, FailedBatchInfo batchInfo)
        {
            SchedulerTaskManager schedulerTaskManager = new SchedulerTaskManager();
            var task = schedulerTaskManager.GetTask(dataSource.Entity.TaskId);
            task.ThrowIfNull("task", dataSource.Entity.TaskId);
            Dictionary<string, dynamic> mailObjects = new Dictionary<string, dynamic>();
            User user = new UserManager().GetUserbyId(task.OwnerId);
            user.ThrowIfNull("user", task.OwnerId);
            mailObjects.Add("User", user);
            mailObjects.Add("Failed Batch Info", batchInfo);
            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendMail(dataSource.Entity.Settings.ErrorMailTemplateId.Value, mailObjects);
        }

        private FailedBatchInfo BuildFailedBatchInfo(DataSourceDetail dataSource, IImportedData data, MappingOutput outputResult, bool isEmpty, string errorMessage)
        {
            FailedBatchInfo batchInfo = new FailedBatchInfo
            {
                DataSourceId = dataSource.Entity.DataSourceId,
                DataSourceName = dataSource.Entity.Name,
                BatchDescription = data.Description,
                Message = !string.IsNullOrEmpty(errorMessage) ? errorMessage : outputResult.Message,
                IsEmpty = isEmpty
            };
            return batchInfo;
        }

        private MappingOutput ExecuteCustomCode(Guid dataSourceId, string customCode, IImportedData data, MappedBatchItemsToEnqueue outputItems, IDataSourceLogger logger)
        {
            MappingOutput outputResult = new MappingOutput();

            Type mapperType = GetMapperRuntimeType(dataSourceId, logger);
            DataMapper mapper = Activator.CreateInstance(mapperType) as DataMapper;
            mapper.SetLogger(logger);

            List<object> failedRecordIdentifiers = new List<object>();

            try
            {
                outputResult = mapper.MapData(dataSourceId, data, outputItems, failedRecordIdentifiers);

                if (failedRecordIdentifiers == null || failedRecordIdentifiers.Count == 0)
                {
                    logger.WriteInformation("Batch mapped successfully");
                }
                else
                {
                    int mappedRecordsCount = outputItems.Sum(itm => (itm != null && itm.Item != null) ? itm.Item.GetRecordCount() : 0);
                    if (mappedRecordsCount > 0)
                    {
                        int failedRecordsCount = failedRecordIdentifiers.Count;
                        var totalRecordsCount = mappedRecordsCount + failedRecordsCount;
                        string message;

                        if (failedRecordsCount <= failedRecordIdentifiersThreshold)
                        {
                            string concatenatedFailedRecordIdentifiers = string.Join<object>(", ", failedRecordIdentifiers);
                            message = string.Format("{0} records failed out of {1} while mapping data. Failed records identifiers: {2}", failedRecordsCount, totalRecordsCount, concatenatedFailedRecordIdentifiers);
                        }
                        else
                        {
                            message = string.Format("More than {0} records failed while mapping data", failedRecordIdentifiersThreshold);
                        }

                        outputResult.Message = message;
                        outputResult.Result = MappingResult.PartialInvalid;
                        logger.WriteWarning("Batch mapped partially");
                    }
                    else
                    {
                        outputResult.Message = "All records mapping has failed";
                        outputResult.Result = MappingResult.Invalid;
                        logger.WriteError("All records mapping has failed");
                    }
                }
            }
            catch (Exception ex)
            {
                outputResult.Message = ex.ToString();
                outputResult.Result = MappingResult.Invalid;
                logger.WriteError("An error occurred while mapping data. Error details: {0}", ex.ToString());
            }

            return outputResult;
        }

        private Type GetMapperRuntimeType(Guid dataSourceId, IDataSourceLogger logger)
        {
            string cacheName = String.Format("DataSourceRuntimeService_GetMapperRuntimeType_{0}", dataSourceId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataSourceManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    DataSourceManager dsManager = new DataSourceManager();
                    var dataSource = dsManager.GetDataSource(dataSourceId);
                    if (dataSource == null)
                        throw new NullReferenceException(String.Format("dataSource '{0}'", dataSourceId));
                    if (dataSource.Settings == null)
                        throw new NullReferenceException(String.Format("dataSource.Settings '{0}'", dataSourceId));
                    if (dataSource.Settings.MapperCustomCode == null)
                        throw new NullReferenceException(String.Format("dataSource.Settings.MapperCustomCode '{0}'", dataSourceId));

                    string fullTypeName;
                    string classDefinition = this.BuildCustomClass(dataSource.Settings.MapperCustomCode, out fullTypeName);

                    CSharpCompilationOutput compilationOutput;
                    if (CSharpCompiler.TryCompileClass(classDefinition, out compilationOutput))
                    {
                        return compilationOutput.OutputAssembly.GetType(fullTypeName);
                    }
                    else
                    {
                        logger.WriteError("Errors while building the code for the custom class. Please make sure to build the custom code bound with this data source.");
                        if (compilationOutput.ErrorMessages != null)
                        {
                            foreach (var error in compilationOutput.ErrorMessages)
                            {
                                logger.WriteError(error);
                            }
                        }
                        throw new Exception("Could not build Custom Code Mapper");
                    }
                });
        }

        private string BuildCustomClass(string customCode, out string fullTypeName)
        {
            string code = (new StringBuilder()).Append(@"public override Vanrise.Integration.Entities.MappingOutput MapData(Guid dataSourceId, Vanrise.Integration.Entities.IImportedData data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
                                                            {").Append(customCode).Append("}").ToString();

            StringBuilder classDefinitionBuilder = new StringBuilder().Append(@"
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using Vanrise.Common;
                using Vanrise.Entities;
                using Vanrise.Integration.Entities;
                using Vanrise.Integration.Mappers;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME#").Append(" : ").Append(typeof(DataMapper).FullName).Append(@"
                    {                        
                        ").Append(code).Append(@"
                    }
                }");

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Integration.Business");
            string className = "DataSourceDataMapper";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
        }
    }
}