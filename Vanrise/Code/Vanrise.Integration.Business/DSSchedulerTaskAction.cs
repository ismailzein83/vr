﻿using System;
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

            dataSource.Settings.Adapter.ImportData(data =>
                {
                    Vanrise.Queueing.PersistentQueueItem queueItem = this.ExecuteCustomCode(dataSource.Settings.MapperCustomCode, data);
                    Vanrise.Queueing.Entities.IPersistentQueue queue = Vanrise.Queueing.PersistentQueueFactory.Default.GetQueue(dataSource.Settings.QueueName);
                    queue.EnqueueObject(queueItem);
                });
        }

        private string CreateCustomClass(string customCode, out string className)
        {
            className = "CustomMapper_" + Math.Abs(customCode.GetHashCode());


            string code = (new StringBuilder()).Append(@"public Vanrise.Queueing.PersistentQueueItem MapData(Vanrise.Integration.Entities.IImportedData data)
                                                            {").Append(customCode).Append("}").ToString();

            string classDefinition = new StringBuilder().Append(@"
                using System;
                using System.Collections.Generic;
                using Vanrise.Integration.Entities;

                namespace Vanrise.Integration.Business
                {
                    public class ").Append(className).Append(" : ").Append(typeof(IDataMapper).FullName).Append(@"
                    {                        
                        ").Append(code).Append(@"
                    }
                }
                ").ToString();

            return classDefinition;
        }

        private Vanrise.Queueing.PersistentQueueItem ExecuteCustomCode(string customCode, IImportedData data)
        {
            string className;

            string classDefinition = this.CreateCustomClass(customCode, out className);

            Vanrise.Queueing.PersistentQueueItem result = null;

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions["CompilerVersion"] = "v4.0";
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

            CompilerParameters parameters = new CompilerParameters();
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

            if (results.Errors.Count == 0)
            {
                Type generated = results.CompiledAssembly.GetType("Vanrise.Integration.Business." + className);
                IDataMapper mapper = (IDataMapper)generated.GetConstructor(Type.EmptyTypes).Invoke(null);
                result = mapper.MapData(data);
            }

            return result;
        }
    }
}
