using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Vanrise.Common
{
    public static class CSharpCompiler
    {
        private class LoadDynamicClass
        {
            public string Prop { get; set; }
        }

        static string s_generatedCodePathInDevMode = ConfigurationManager.AppSettings["VRDevMode_GeneratedCodePath"];
        static HashSet<string> expiredAssemblies = new HashSet<string>();

        static Dictionary<string, Assembly> s_dynamicAssembliesByName = new Dictionary<string, Assembly>();

        static CSharpCompiler()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            dynamic d = new LoadDynamicClass();//To force loading Microsoft.CSharp
            d.Prop = "Test";
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return s_dynamicAssembliesByName[args.Name.Substring(0, args.Name.IndexOf(','))];
        }

        public static bool TryCompileClass(string classDefinition, out CSharpCompilationOutput output, bool ignoreNamespacesCompilation = false)
        {
            return TryCompileClass(null, classDefinition, out output, ignoreNamespacesCompilation);
        }

        public static bool TryCompileClass(string className, string classDefinition, out CSharpCompilationOutput output, bool ignoreNamespacesCompilation = false)
        {
            if (!ignoreNamespacesCompilation)
            {
                IVRNamespaceManager vrNamespaceManager = BusinessManagerFactory.GetManager<IVRNamespaceManager>();
                vrNamespaceManager.CompileVRNamespacesAssembly();
            }

            output = new CSharpCompilationOutput();

            string outputFileName = string.Empty;

            if (!string.IsNullOrWhiteSpace(className) && !String.IsNullOrWhiteSpace(s_generatedCodePathInDevMode))
            {
                outputFileName = Path.Combine(s_generatedCodePathInDevMode, String.Concat(className.Replace(" ", ""), Guid.NewGuid().ToString().Replace("-", ""), ".cs"));
                File.WriteAllText(outputFileName, classDefinition);
            }

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classDefinition, path: outputFileName, encoding: System.Text.Encoding.UTF8);

            string assemblyName = String.Format("RuntimeAssembly_{0}", Guid.NewGuid().ToString("N"));

            List<MetadataReference> references = new List<MetadataReference>();

            string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            if (VRWebContext.AreDllsInBinFolder())
                path = Path.Combine(path, "bin");

            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                Assembly.LoadFile(info.FullName);
            }

            HashSet<string> referencedAssembliesFullNames = new HashSet<string>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!string.IsNullOrEmpty(assembly.Location) && !expiredAssemblies.Contains(assembly.FullName) && !referencedAssembliesFullNames.Contains(assembly.FullName))
                    {
                        references.Add(MetadataReference.CreateFromFile(assembly.Location));
                        referencedAssembliesFullNames.Add(assembly.FullName);
                    }
                }
                catch (NotSupportedException ex)
                {
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            bool outputResult;
            using (var ms = new MemoryStream())
            {
                var pdbMs = new MemoryStream();
                EmitResult result = compilation.Emit(ms, pdbMs);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    output = new CSharpCompilationOutput() { Errors = new List<CSharpCompilationError>(), ErrorMessages = new List<string>() };
                    foreach (Diagnostic diagnostic in failures)
                    {
                        CSharpCompilationError error = BuildCSharpCompilationError(diagnostic);
                        output.ErrorMessages.Add(diagnostic.ToString());
                        output.Errors.Add(error);
                    }
                    outputResult = false;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] byteArray = ms.ToArray();

                    string formatFileName = string.Format("{0}.dll", assemblyName);
                    string fullPath = Path.Combine(Path.GetTempPath(), formatFileName);
                    File.WriteAllBytes(fullPath, byteArray);

                    byte[] pdbByteArray = pdbMs.ToArray();
                    string formatPdbFileName = string.Format("{0}.pdb", assemblyName);
                    string pdbFullPath = Path.Combine(Path.GetTempPath(), formatPdbFileName);
                    File.WriteAllBytes(pdbFullPath, pdbByteArray);

                    Assembly assembly = Assembly.LoadFrom(fullPath);
                    s_dynamicAssembliesByName.Add(assemblyName, assembly);

                    output = new CSharpCompilationOutput() { AssemblyFile = byteArray, OutputAssembly = assembly };
                    outputResult = true;
                }
            }

            return outputResult;
        }

        private static CSharpCompilationError BuildCSharpCompilationError(Diagnostic diagnostic)
        {
            string diagnosticAsString = diagnostic.ToString();
            string[] parts = diagnosticAsString.Split(':');

            int lineNumber = int.Parse(parts[0].Split(',')[0].Replace("(", ""));
            string errorNumber = parts[1].Replace("error", "").Trim();

            return new CSharpCompilationError() { ErrorNumber = errorNumber, ErrorText = diagnostic.GetMessage(), LineNumber = lineNumber };
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

        public static void SetExpiredAssembly(string assemblyName)
        {
            expiredAssemblies.Add(assemblyName);
        }
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
