﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceRuntimeService : Vanrise.Runtime.RuntimeService
    {
        DataSourceRuntimeInstanceManager _dsRuntimeInstanceManager = new DataSourceRuntimeInstanceManager();

        protected override void Execute()
        {
            DataSourceRuntimeInstance dsRuntimeInstance = _dsRuntimeInstanceManager.TryGetOneAndLock();
            if(dsRuntimeInstance != null)
            {
                Console.WriteLine("{0}: DataSourceRuntimeService Data Source is started", DateTime.Now);
                var dataSourceManager = new DataSourceManager();
                var dataSource = dataSourceManager.GetDataSourceDetail(dsRuntimeInstance.DataSourceId);
                if (dataSource == null)
                    throw new ArgumentNullException(String.Format("dataSource '{0}'", dsRuntimeInstance.DataSourceId));
                
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

                try
                {
                    BaseReceiveAdapter adapter = (BaseReceiveAdapter)Activator.CreateInstance(Type.GetType(dataSource.AdapterInfo.FQTN));
                    adapter.SetLogger(logger);
                    adapter.SetDataSourceManager(dataSourceManager);
                    Action<IImportedData> onDataReceivedAction = data =>
                    {
                        logger.WriteVerbose("Executing the custom code written for the mapper");
                        MappedBatchItemsToEnqueue outputItems = new MappedBatchItemsToEnqueue();
                        MappingOutput outputResult = this.ExecuteCustomCode(dataSource.Entity.DataSourceId, dataSource.Entity.Settings.MapperCustomCode, data, outputItems, logger);

                        if (!data.IsMultipleReadings)
                            data.OnDisposed();

                        if (data.IsEmpty)
                        {
                            logger.WriteInformation("Received Empty Batch");
                            return;
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
                                    logger.WriteInformation("Enqueuing item '{0}' to stage '{1}'", outputItem.Item.GenerateDescription(), outputItem.StageName);

                                    long queueItemId = queuesByStages[outputItem.StageName].Queue.EnqueueObject(outputItem.Item);
                                    logger.WriteInformation("Enqueued the item successfully");

                                    queueItemsIds.Add(queueItemId);
                                    totalRecordsCount += outputItem.Item.GetRecordCount();
                                }
                                catch (Exception ex)
                                {
                                    logger.WriteError("An error occured while enqueuing item in stage {0}. Exception details {1}", outputItem.StageName, ex.ToString());
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
                    };

                    AdapterImportDataContext adapterContext = new AdapterImportDataContext(dataSource.Entity, onDataReceivedAction);
                    adapter.ImportData(adapterContext);
                }
                finally
                {
                    _dsRuntimeInstanceManager.SetInstanceCompleted(dsRuntimeInstance.DataSourceRuntimeInstanceId);

                    logger.WriteInformation("A runtime Instance is finished for the Data Source '{0}'", dataSource.Entity.Name);
                    Console.WriteLine("{0}: DataSourceRuntimeService Data Source is Done", DateTime.Now);
                }
            }            
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
                logger.WriteError("An error occured while mapping data. Error details: {0}", ex.ToString());
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
                    if(dataSource.Settings == null)
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
            string code = (new StringBuilder()).Append(@"public override Vanrise.Integration.Entities.MappingOutput MapData(int dataSourceId, Vanrise.Integration.Entities.IImportedData data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
                                                            {").Append(customCode).Append("}").ToString();

            StringBuilder classDefinitionBuilder = new StringBuilder().Append(@"
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using Vanrise.Integration.Entities;
                using Vanrise.Integration.Mappers;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME#").Append(" : ").Append(typeof(DataMapper).FullName).Append(@"
                    {                        
                        ").Append(code).Append(@"
                    }
                }
                ");

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Integration.Business");
            string className = "DataSourceDataMapper";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);  

            return classDefinitionBuilder.ToString();
        }
    }
}
