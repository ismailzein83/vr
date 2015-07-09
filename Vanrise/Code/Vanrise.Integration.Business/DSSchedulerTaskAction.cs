using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
        CustomClassSettings _classSettings;

        public override void Execute(SchedulerTask task, Dictionary<string, object> evaluatedExpressions)
        {
            DataSourceManager dataManager = new DataSourceManager();
            Vanrise.Integration.Entities.DataSource dataSource = dataManager.GetDataSourcebyTaskId(task.TaskId);
            this.PrepareCustomClass(dataSource.Settings.MapperCustomCode);

            dataSource.Settings.Adapter.ImportData(ReceiveData);
        }

        private void ReceiveData(IImportedData data)
        {
            Vanrise.Queueing.PersistentQueueItem queueItem = this.ExecuteCustomCode(data);
        }

        private void PrepareCustomClass(string customCode)
        {
            this._classSettings = new CustomClassSettings();

            this._classSettings.ClassName = "CustomMapper_" + Math.Abs(customCode.GetHashCode());


            string code = (new StringBuilder()).Append(@"public Vanrise.Queueing.PersistentQueueItem MapData(Vanrise.Integration.Entities.IImportedData data)
                                                            {").Append(customCode).Append("}").ToString();


            this._classSettings.ClassDefinition = (new StringBuilder()).Append(@"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Runtime.InteropServices;
                using System.Drawing;
                using System.IO;

                namespace Vanrise.Integration.Business
                {
                    public class ").Append(this._classSettings.ClassName).Append(" : ").Append(typeof(IDataMapper).FullName).Append(@"
                    {                        
                        ").Append(code).Append(@"
                    }
                }
                ").ToString();
        }

        private Vanrise.Queueing.PersistentQueueItem ExecuteCustomCode(IImportedData data)
        {
            Vanrise.Queueing.PersistentQueueItem result = null;

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions["CompilerVersion"] = "v4.0";
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Web.dll");
            parameters.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);
            parameters.ReferencedAssemblies.Add(typeof(System.IO.Stream).Assembly.Location);
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");



            parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, this._classSettings.ClassDefinition);

            if (results.Errors.Count == 0)
            {
                Type generated = results.CompiledAssembly.GetType("TestConsole." + this._classSettings.ClassName);
                IDataMapper mapper = (IDataMapper)generated.GetConstructor(Type.EmptyTypes).Invoke(null);
                result = mapper.MapData(data);
            }

            return result;
        }
    }

    class CustomClassSettings
    {
        public string ClassName { get; set; }

        public string ClassDefinition { get; set; }
    }
}
