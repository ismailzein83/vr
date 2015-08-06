using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Integration.Business
{
    public class DSSchedulerTaskAction : SchedulerTaskAction
    {
        DataSourceLogger _logger = new DataSourceLogger();

        public override void Execute(SchedulerTask task, Dictionary<string, object> evaluatedExpressions)
        {
            DataSourceManager dataManager = new DataSourceManager();
            Vanrise.Integration.Entities.DataSource dataSource = dataManager.GetDataSourcebyTaskId(task.TaskId);

            _logger.DataSourceId = dataSource.DataSourceId;
            _logger.WriteInformation("Started running data source with name '{0}'", dataSource.Name);

            _logger.WriteVerbose("Preparing queues and stages");

            QueuesByStages queuesByStages = null;

            try
            {
                Vanrise.Queueing.QueueExecutionFlowManager executionFlowManager = new Vanrise.Queueing.QueueExecutionFlowManager();
                queuesByStages = executionFlowManager.GetQueuesByStages(dataSource.Settings.ExecutionFlowId);

                if (queuesByStages == null || queuesByStages.Count == 0)
                {
                    _logger.WriteError("No stages ready for use");
                    return;
                }
                else
                {
                    _logger.WriteInformation("{0} stage(s) are ready for use", queuesByStages.Count); 
                }
            }
            catch(Exception ex)
            {
                _logger.WriteError("An error occurred while preparing stages. Exception Details {0}", ex.Message);
                throw;
            }

            _logger.WriteVerbose("Preparing adapters to start the import process");

            BaseReceiveAdapter adapter = (BaseReceiveAdapter)Activator.CreateInstance(Type.GetType(dataSource.AdapterInfo.FQTN));
            adapter.SetLogger(_logger);
            adapter.ImportData(dataSource.Settings.AdapterArgument, data =>
                {
                    ImportedBatchEntry importedBatchEntry = new ImportedBatchEntry();
                    importedBatchEntry.BatchSize = data.BatchSize;
                    importedBatchEntry.BatchDescription = data.Description;
                    
                    _logger.WriteVerbose("Import Process is done, preparing for mapping data with description {0}", data.Description);

                    _logger.WriteVerbose("Executing the custom code written for the mapper");
                    MappedBatchItemsToEnqueue outputItems = new MappedBatchItemsToEnqueue();
                    MappingOutput outputResult = this.ExecuteCustomCode(dataSource.DataSourceId, dataSource.Settings.MapperCustomCode, data, outputItems);

                    importedBatchEntry.Result = outputResult.Result;
                    importedBatchEntry.MapperMessage = outputResult.Message;

                    List<long> queueItemsIds = new List<long>();
                    int totalRecordsCount = 0;

                    if (outputItems.Count > 0)
                    {
                        foreach (var outputItem in outputItems)
                        {
                            try
                            {
                                _logger.WriteInformation("Enqueuing item '{0}' to stage '{1}'", outputItem.Item.GenerateDescription(), outputItem.StageName);

                                long queueItemId = queuesByStages[outputItem.StageName].Queue.EnqueueObject(outputItem.Item);
                                _logger.WriteInformation("Enqueued the item successfully");

                                queueItemsIds.Add(queueItemId);
                                totalRecordsCount += outputItem.Item.GetRecordCount();
                            }
                            catch(Exception ex)
                            {
                                _logger.WriteError("An error occured while enqueuing item in stage {0}. Exception details {1}", outputItem.StageName, ex.Message);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        _logger.WriteWarning("No mapped items to qneueue, the written custom code should specify at least one output item to enqueue items to");
                    }
                    data.OnDisposed();

                    importedBatchEntry.QueueItemsIds = string.Join(",", queueItemsIds);
                    importedBatchEntry.RecordsCount = totalRecordsCount;

                    long importedBatchId = _logger.LogImportedBatchEntry(importedBatchEntry);
                    _logger.LogEntry(Common.LogEntryType.Information, importedBatchId, "Finished running data source task with name '{0}'", dataSource.Name);

                    return (outputResult.Result == MappingResult.Valid);
                });
        }

        private MappingOutput ExecuteCustomCode(int dataSourceId, string customCode, IImportedData data, MappedBatchItemsToEnqueue outputItems)
        {
            MappingOutput outputResult = null;

            int strHashCode = Math.Abs(customCode.GetHashCode());

            string assemblyName = "Vanrise_Mappers_" + strHashCode;
            string className = "CustomMapper_" + strHashCode;

            Assembly generatedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.StartsWith(assemblyName));
            Type generatedType = null;

            if (generatedAssembly != null)
            {
                generatedType = generatedAssembly.GetType("Vanrise.Integration.Mappers." + className);
            }

            if (generatedType == null)
            {
                Assembly compiledAssembly;
                if (BuildGeneratedType(assemblyName, customCode, className, out compiledAssembly))
                {
                    generatedType = compiledAssembly.GetType("Vanrise.Integration.Mappers." + className);
                }
                else
                {
                    
                    outputResult = new MappingOutput()
                    {
                        Message = "Errors while building the code for the custom class",
                        Result = MappingResult.Invalid
                    };
                    
                    return outputResult;
                }
            }

            _logger.WriteInformation("Custom class is ready for use");

            try
            {
                DataMapper mapper = (DataMapper)generatedType.GetConstructor(Type.EmptyTypes).Invoke(null);
                mapper.SetLogger(_logger);
                outputResult = mapper.MapData(data, outputItems);
                _logger.WriteInformation("Mapped data successfully with output result {0}", outputResult.Result);
            }
            catch(Exception ex)
            {
                _logger.WriteError("An error occured while mapping data. Error details: {0}", ex.Message);
            }
            
            return outputResult;
        }

        private bool BuildGeneratedType(string assemblyName, string customCode, string className, out Assembly compiledAssembly)
        {
            string classDefinition = this.BuildCustomClass(customCode, className);

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions["CompilerVersion"] = "v4.0";
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

            CompilerParameters parameters = new CompilerParameters();
            parameters.OutputAssembly = assemblyName;
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(typeof(DataMapper).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Vanrise.Queueing.Entities.PersistentQueueItem).Assembly.Location);

            parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            string path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                parameters.ReferencedAssemblies.Add(info.FullName);
            }

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);

            bool compilationResult = (results.Errors.Count == 0);

            if(compilationResult)
            {
                compiledAssembly = results.CompiledAssembly;
            }
            else
            {
                _logger.WriteError("Errors while building the code for the custom class. Please make sure to build the custom code bound with this data source.");
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    CompilerError error = results.Errors[i];
                    if (error.IsWarning)
                        _logger.WriteWarning("Warning {0}: {1}", i, error.ErrorText);
                    else
                        _logger.WriteError("Error {0}: {1}", i, error.ErrorText);
                }
                
                foreach (CompilerError error in results.Errors)
                {
                    
                }
                compiledAssembly = null;
            }

            return compilationResult;
        }

        private string BuildCustomClass(string customCode, string className)
        {
            string code = (new StringBuilder()).Append(@"public override Vanrise.Integration.Entities.MappingOutput MapData(Vanrise.Integration.Entities.IImportedData data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
                                                            {").Append(customCode).Append("}").ToString();

            string classDefinition = new StringBuilder().Append(@"
                using System;
                using System.Collections.Generic;
                using System.IO;
                using Vanrise.Integration.Entities;

                namespace Vanrise.Integration.Mappers
                {
                    public class ").Append(className).Append(" : ").Append(typeof(DataMapper).FullName).Append(@"
                    {                        
                        ").Append(code).Append(@"
                    }
                }
                ").ToString();

            return classDefinition;
        }
    }
}
