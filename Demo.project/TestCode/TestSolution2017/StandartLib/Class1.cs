using System;
using System.Data;
//using Vanrise.Common;
using System.Data.SqlClient;
using CoreWf;
using CoreWf.Statements;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using System.CodeDom.Compiler;

namespace StandartLib
{
    public class Class1
    {
        public void Test()
        {
            //CoreWf.WorkflowApplication app = new CoreWf.WorkflowApplication();

            var sequence = new Sequence
            {
                 Activities =
                {
                    new WriteLine { Text= new InArgument<string>("text from workflow")}
                }
            };

            var app = new WorkflowApplication(sequence);
            
            app.Run();
            Console.ReadKey();


            using (SqlConnection conn = new SqlConnection(""))
            {
                
            }
            
            //Serializer.Serialize(new Class2 { Prop = "rewrewrew" });
        }       

        class MyAct : CoreWf.AsyncCodeActivity
        {
            protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            protected override void CacheMetadata(CodeActivityMetadata metadata)
            {
                throw new NotImplementedException();
            }

            protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
            {
                throw new NotImplementedException();
            }
        }
        static string s_generatedCodePathInDevMode;
        static HashSet<string> expiredAssemblies = new HashSet<string>();

        public static bool TryCompileClass(string className, string classDefinition, out CSharpCompilationOutput output)
        {
            
            output = new CSharpCompilationOutput();
            string assemblyName = String.Format("RuntimeAssembly_{0}", Guid.NewGuid().ToString("N"));

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions["CompilerVersion"] = "v4.0";
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

            CompilerParameters parameters = new CompilerParameters();
            //  parameters.OutputAssembly = assemblyName;
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            parameters.IncludeDebugInformation = true;
            //parameters.ReferencedAssemblies.Add("System.dll");
            //parameters.ReferencedAssemblies.Add("System.Data.dll");

            HashSet<string> referencedAssembliesFullNames = new HashSet<string>();

            //parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            //parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            var runtimeBinderAssembly = typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly;
            referencedAssembliesFullNames.Add(runtimeBinderAssembly.FullName);
            parameters.ReferencedAssemblies.Add(runtimeBinderAssembly.Location);//this is needed for dynamic variables
            string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            //if (System.Web.HttpContext.Current != null)
            //    path = Path.Combine(path, "bin");
            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                Assembly.LoadFile(info.FullName);
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!expiredAssemblies.Contains(assembly.FullName) && !referencedAssembliesFullNames.Contains(assembly.FullName))
                    {
                        parameters.ReferencedAssemblies.Add(assembly.Location);
                        referencedAssembliesFullNames.Add(assembly.FullName);
                    }
                }
                catch (NotSupportedException ex)
                {
                }
            }

            CompilerResults results;
            if (!string.IsNullOrWhiteSpace(className) && !String.IsNullOrWhiteSpace(s_generatedCodePathInDevMode))
            {
                parameters.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
                parameters.TempFiles.KeepFiles = true;
                string outputFileName = Path.Combine(s_generatedCodePathInDevMode, String.Concat(className.Replace(" ", ""), Guid.NewGuid().ToString().Replace("-", ""), ".cs"));
                File.WriteAllText(outputFileName, classDefinition);
                results = provider.CompileAssemblyFromFile(parameters, outputFileName);
            }
            else
            {
                results = provider.CompileAssemblyFromSource(parameters, classDefinition);
            }
            if (results.Errors != null && results.Errors.Count > 0)
            {
                output.ErrorMessages = new List<string>();
                output.Errors = new List<CSharpCompilationError>();

                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        output.ErrorMessages.Add(String.Format("Error {0}: {1}. Line Number {2}", error.ErrorNumber, error.ErrorText, error.Line));
                        output.Errors.Add(new CSharpCompilationError() { ErrorNumber = error.ErrorNumber, ErrorText = error.ErrorText, LineNumber = error.Line });
                    }
                }
            }
            if (output.ErrorMessages != null && output.ErrorMessages.Count > 0)
                return false;
            else
            {
                output.OutputAssembly = results.CompiledAssembly;
                output.AssemblyFile = File.ReadAllBytes(results.PathToAssembly);
                return true;
            }
        }
    }

    public class Class2
    {
        public string Prop { get; set; }

    }


    public class CSharpCompilationOutput
    {
        public Assembly OutputAssembly { get; set; }

        public byte[] AssemblyFile { get; set; }

        public List<string> ErrorMessages { get; set; }

        public List<CSharpCompilationError> Errors { get; set; }
    }

    public class CSharpCompilationError
    {
        public string ErrorText { get; set; }

        public string ErrorNumber { get; set; }

        public int LineNumber { get; set; }
    }
}
