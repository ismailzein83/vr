using System;
using System.Collections.Generic;
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
        DataSourceRuntimeInstanceManager _dsRuntimeInstanceManager = new DataSourceRuntimeInstanceManager();
        IDataSourceRuntimeInstanceDataManager _dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceRuntimeInstanceDataManager>();
        DataSourceManager _dataSourceManager = new DataSourceManager();
        public override void Execute()
        {
            List<DataSourceRuntimeInstance> dataSourceRuntimeInstances = _dataManager.GetAll();
            if(dataSourceRuntimeInstances != null && dataSourceRuntimeInstances.Count > 0)
            {
                foreach(var dsRuntimeInstance in dataSourceRuntimeInstances)
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
                                    ExecuteDataSource(dataSource);
                                }
                                finally
                                {
                                    _dataManager.DeleteInstance(dsRuntimeInstance.DataSourceRuntimeInstanceId);
                                }

                                isInstanceLockedAndExecuted = true;
                            }
                        });
                    if (isInstanceLockedAndExecuted)
                        break;
                }
            }
        }

        void ExecuteDataSource(DataSourceDetail dataSource)
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
                lastReceivedBatchData = data;
                logger.WriteVerbose("Executing the custom code written for the mapper");
                MappedBatchItemsToEnqueue outputItems = new MappedBatchItemsToEnqueue();
                MappingOutput outputResult = this.ExecuteCustomCode(dataSource.Entity.DataSourceId, dataSource.Entity.Settings.MapperCustomCode, data, outputItems, logger);

                ImportedBatchProcessingOutput batchProcessingOutput = new ImportedBatchProcessingOutput
                {
                    OutputResult = outputResult
                };

                if (SendErrorNotification(dataSource, data, outputResult))
                {
                    return batchProcessingOutput;
                }

                if (!data.IsMultipleReadings)
                    data.OnDisposed();

                if (data.IsEmpty)
                {
                    logger.WriteInformation("Received Empty Batch");
                    return null;
                }

                ImportedBatchEntry importedBatchEntry = new ImportedBatchEntry();
                importedBatchEntry.BatchSize = data.BatchSize;
                importedBatchEntry.BatchDescription = data.Description;

                importedBatchEntry.Result = outputResult.Result;
                importedBatchEntry.MapperMessage = outputResult.Message;

                List<long> queueItemsIds = new List<long>();
                int totalRecordsCount = 0;

                if (outputItems != null && outputItems.Count > 0)
                {
                    foreach (var outputItem in outputItems)
                    {
                        try
                        {
                            outputItem.Item.DataSourceID = dataSource.Entity.DataSourceId;
                            outputItem.Item.BatchDescription = data.Description;
                            logger.WriteInformation("Enqueuing item '{0}' to stage '{1}'", outputItem.Item.GenerateDescription(), outputItem.StageName);

                            long queueItemId = queuesByStages[outputItem.StageName].Queue.EnqueueObject(outputItem.Item);
                            logger.WriteInformation("Enqueued the item successfully");

                            queueItemsIds.Add(queueItemId);
                            totalRecordsCount += outputItem.Item.GetRecordCount();
                        }
                        catch (Exception ex)
                        {
                            logger.WriteError("An error occurred while enqueuing item in stage {0}. Exception details {1}", outputItem.StageName, ex.ToString());
                            throw;
                        }
                    }
                }
                else
                {
                    logger.WriteWarning("No mapped items to enqueue, the written custom code should specify at least one output item to enqueue items to");
                }

                importedBatchEntry.QueueItemsIds = string.Join(",", queueItemsIds);
                importedBatchEntry.RecordsCount = totalRecordsCount;

                long importedBatchId = logger.LogImportedBatchEntry(importedBatchEntry);
                logger.LogEntry(Vanrise.Entities.LogEntryType.Information, importedBatchId, "Imported a new batch with Id '{0}'", importedBatchId);

                return batchProcessingOutput;
            };

            try
            {
                AdapterImportDataContext adapterContext = new AdapterImportDataContext(dataSource.Entity, onDataReceivedAction);
                adapter.ImportData(adapterContext);

                logger.WriteInformation("A runtime Instance is finished for the Data Source '{0}'", dataSource.Entity.Name);
            }
            catch(Exception ex)
            {
                string errorMessage = string.Format("Adapter failed due to error: {0}", ex.ToString());
                logger.WriteError(errorMessage);
                if (dataSource.Entity.Settings.ErrorMailTemplateId.HasValue)
                {
                    FailedBatchInfo batchInfo = new FailedBatchInfo
                    {
                        Message = errorMessage,
                        DataSourceId = dataSource.Entity.DataSourceId,
                        DataSourceName = dataSource.Entity.Name,
                        BatchDescription = lastReceivedBatchData != null ? lastReceivedBatchData.Description : null
                    };
                    SendErrorNotification(dataSource, batchInfo);
                }
                throw;
            }
        }

        bool SendErrorNotification(DataSourceDetail dataSource, IImportedData data, MappingOutput outputResult)
        {
            if (dataSource.Entity.Settings.ErrorMailTemplateId.HasValue)
            {
                bool isEmptyFile = data.IsFile && (!data.BatchSize.HasValue || data.BatchSize.Value == 0);
                if (isEmptyFile || outputResult.Result == MappingResult.Invalid)
                {
                    FailedBatchInfo batchInfo = BuildFailedBatchInfo(dataSource, data, outputResult);
                    if (isEmptyFile)
                    {
                        batchInfo.IsEmpty = true;
                    }
                    SendErrorNotification(dataSource, batchInfo);
                    return true;
                }
            }
            return false;
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

        FailedBatchInfo BuildFailedBatchInfo(DataSourceDetail dataSource, IImportedData data, MappingOutput outputResult)
        {
            FailedBatchInfo batchInfo = new FailedBatchInfo
            {
                Message = outputResult.Message,
                DataSourceId = dataSource.Entity.DataSourceId,
                BatchDescription = data.Description,
                DataSourceName = dataSource.Entity.Name
            };
            return batchInfo;
        }
        private MappingOutput ExecuteCustomCode(Guid dataSourceId, string customCode, IImportedData data, MappedBatchItemsToEnqueue outputItems, IDataSourceLogger logger)
        {
            MappingOutput outputResult = new MappingOutput();

            Type mapperType = GetMapperRuntimeType(dataSourceId, logger);
            DataMapper mapper = Activator.CreateInstance(mapperType) as DataMapper;
            mapper.SetLogger(logger);

            try
            {
                outputResult = mapper.MapData(dataSourceId, data, outputItems);
                logger.WriteInformation("Mapped data successfully with output result {0}", outputResult.Result);
            }
            catch (Exception ex)
            {
                logger.WriteError("An error occurred while mapping data. Error details: {0}", ex.ToString());
                outputResult.Result = MappingResult.Invalid;
                outputResult.Message = ex.ToString();
            }

            return outputResult;
        }

        Type GetMapperRuntimeType(Guid dataSourceId, IDataSourceLogger logger)
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
            string code = (new StringBuilder()).Append(@"public override Vanrise.Integration.Entities.MappingOutput MapData(Guid dataSourceId, Vanrise.Integration.Entities.IImportedData data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
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
