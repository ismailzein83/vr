using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Integration.Business
{
    public class DSSchedulerTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, Dictionary<string, object> evaluatedExpressions)
        {
            DataSourceManager dataManager = new DataSourceManager();
            Vanrise.Integration.Entities.DataSource dataSource = dataManager.GetDataSourcebyTaskId(task.TaskId);

            BaseReceiveAdapter adapter = (BaseReceiveAdapter)Activator.CreateInstance(Type.GetType(dataSource.AdapterInfo.FQTN));

            adapter.ImportData(dataSource.Settings.AdapterArgument, data =>
                {
                    Vanrise.Queueing.PersistentQueueItem queueItem = this.ExecuteCustomCode(dataSource.Settings.MapperCustomCode, data);
                    Vanrise.Queueing.Entities.IPersistentQueue queue = Vanrise.Queueing.PersistentQueueFactory.Default.GetQueue(dataSource.Settings.QueueName);
                    queue.EnqueueObject(queueItem);
                });
        }

        private Vanrise.Queueing.PersistentQueueItem ExecuteCustomCode(string customCode, IImportedData data)
        {
            Vanrise.Queueing.PersistentQueueItem result = null;

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
                    //TODO: log build errors
                    return result;
                }
            }

            IDataMapper mapper = (IDataMapper)generatedType.GetConstructor(Type.EmptyTypes).Invoke(null);
            result = mapper.MapData(data);
            
            return result;
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
            parameters.ReferencedAssemblies.Add(typeof(IDataMapper).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(Vanrise.Queueing.PersistentQueueItem).Assembly.Location);

            parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            string path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                parameters.ReferencedAssemblies.Add(info.FullName);
            }

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);
            compiledAssembly = results.CompiledAssembly;

            return (results.Errors.Count == 0);
        }

        private string BuildCustomClass(string customCode, string className)
        {
            string code = (new StringBuilder()).Append(@"public Vanrise.Queueing.PersistentQueueItem MapData(Vanrise.Integration.Entities.IImportedData data)
                                                            {").Append(customCode).Append("}").ToString();

            string classDefinition = new StringBuilder().Append(@"
                using System;
                using System.Collections.Generic;
                using Vanrise.Integration.Entities;

                namespace Vanrise.Integration.Mappers
                {
                    public class ").Append(className).Append(" : ").Append(typeof(IDataMapper).FullName).Append(@"
                    {                        
                        ").Append(code).Append(@"
                    }
                }
                ").ToString();

            return classDefinition;
        }
    }
}
