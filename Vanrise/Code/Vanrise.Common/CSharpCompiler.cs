using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class CSharpCompiler
    {
        public static bool TryCompileClass(string classDefinition, out CSharpCompilationOutput output)
        {
            output = new CSharpCompilationOutput();
            string assemblyName = String.Format("RuntimeAssembly_{0}", Guid.NewGuid().ToString().Replace("-",""));

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

            parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            string path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                parameters.ReferencedAssemblies.Add(info.FullName);
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!parameters.ReferencedAssemblies.Contains(assembly.Location))
                        parameters.ReferencedAssemblies.Add(assembly.Location);
                }
                catch (NotSupportedException ex)
                { }
            }

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);
            
            if (results.Errors != null && results.Errors.Count > 0)
            {
                output.ErrorMessages = new List<string>();
                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        output.ErrorMessages.Add(String.Format("Error {0}: {1}. Line Number {2}", error.ErrorNumber, error.ErrorText, error.Line));
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

        public static string GenerateUniqueNamespace(string originalNamespace)
        {
            return String.Format("{0}.Gen_{1}", originalNamespace, Guid.NewGuid().ToString().Replace("-", ""));
        }
    }

    public class CSharpCompilationOutput
    {
        public Assembly OutputAssembly { get; set; }

        public byte[] AssemblyFile { get; set; }

        public List<string> ErrorMessages { get; set; }
    }
}
