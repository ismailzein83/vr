﻿using System;
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

            parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            parameters.ReferencedAssemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location);//this is needed for dynamic variables

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
                { 
                }
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
            return String.Format("{0}.Gen_{1}", originalNamespace, Guid.NewGuid().ToString("N"));
        }

        /// <summary>
        /// This method takes a type and produces a proper full type name for it, expanding generics properly.
        /// </summary>
        /// <param name="type">
        /// The type to produce the full type name for.
        /// </param>
        /// <returns>
        /// The type name for <paramref name="type"/> as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="type"/> is <c>null</c>.</para>
        /// </exception>
        public static String TypeToString(Type type)
        {
            #region Parameter Validation

            if (Object.ReferenceEquals(null, type))
                throw new ArgumentNullException("type");

            #endregion

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlyingType = type.GetGenericArguments()[0];
                    return String.Format("{0}?", TypeToString(underlyingType));
                }
                String baseName = type.FullName.Substring(0, type.FullName.IndexOf("`"));
                return baseName + "<" + String.Join(", ", (from paramType in type.GetGenericArguments()
                                                           select TypeToString(paramType)).ToArray()) + ">";
            }
            else
            {
                return type.FullName;
            }
        }
    }

    public class CSharpCompilationOutput
    {
        public Assembly OutputAssembly { get; set; }

        public byte[] AssemblyFile { get; set; }

        public List<string> ErrorMessages { get; set; }
    }
}
