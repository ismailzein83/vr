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
        public const string DEVPROJECT_ASSEMBLY_PREFIX = "VRDEVPROJECT";//the value of this constant should not be changed anytime
        public const string DEVPROJECT_ASSEMBLY_SUFFIX = "ENDVRPROJ";//the value of this constant should not be changed anytime
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
            Assembly assembly;
            s_dynamicAssembliesByName.TryGetValue(args.Name.Substring(0, args.Name.IndexOf(',')), out assembly);
            return assembly;
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
                    //if (assembly.GetName().Name.StartsWith(DEVPROJECT_ASSEMBLY_PREFIX) && assembly.GetName().Name.EndsWith(DEVPROJECT_ASSEMBLY_SUFFIX))
                    //    continue;

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

            //var compiledAssemblyManager = BusinessManagerFactory.GetManager<IVRCompiledAssemblyManager>();
            //var compiledAssemblies = compiledAssemblyManager.GetAssembliesEffectiveForDevProjects();

            //if(compiledAssemblies != null)
            //{
            //    foreach (var compiledAssembly in compiledAssemblies)
            //    {                 
            //        references.Add(MetadataReference.CreateFromStream(new MemoryStream(compiledAssembly.AssemblyContent)));
            //        referencedAssembliesFullNames.Add(compiledAssembly.Name);
            //    }
            //}

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
                        CSharpCompilationError error = BuildCSharpCompilationError(diagnostic, outputFileName);
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

        public static bool TryCompileClass(string assemblyNamePrefix, string assemblyNameSuffix, CSharpCodeFileToCompileCollection codeFiles, List<Guid> excludedDevProjectIdsFromReferences, out CSharpCompilationOutput output, bool ignoreNamespacesCompilation = false)
        {
            output = null;
            return false;
            ////if (!ignoreNamespacesCompilation)
            ////{
            ////    IVRNamespaceManager vrNamespaceManager = BusinessManagerFactory.GetManager<IVRNamespaceManager>();
            ////    vrNamespaceManager.CompileVRNamespacesAssembly();
            ////}

            //output = new CSharpCompilationOutput();

            //string outputFileName = string.Empty;//this should be fixed
            //List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            //foreach (var codeFile in codeFiles.Values)
            //{

            //    if (!string.IsNullOrWhiteSpace(codeFile.Name) && !String.IsNullOrWhiteSpace(s_generatedCodePathInDevMode))
            //    {
            //        outputFileName = Path.Combine(s_generatedCodePathInDevMode, String.Concat(codeFile.Name.Replace(" ", ""), Guid.NewGuid().ToString().Replace("-", ""), ".cs"));
            //        File.WriteAllText(outputFileName, codeFile.Code);
            //    }

            //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeFile.Code, path: !string.IsNullOrWhiteSpace(outputFileName) ? outputFileName : codeFile.Name, encoding: System.Text.Encoding.UTF8);
            //    syntaxTrees.Add(syntaxTree);
            //}

            //string assemblyName;
            //if (assemblyNamePrefix != null)
            //{
            //    assemblyName = $"{assemblyNamePrefix}_{Guid.NewGuid().ToString("N")}";
            //}
            //else
            //{
            //    assemblyName = $"RuntimeAssembly_{Guid.NewGuid().ToString("N")}";
            //}

            //if (!string.IsNullOrWhiteSpace(assemblyNameSuffix))
            //    assemblyName += "_" + assemblyNameSuffix;

            //List<MetadataReference> references = new List<MetadataReference>();

            //string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            //if (VRWebContext.AreDllsInBinFolder())
            //    path = Path.Combine(path, "bin");

            //foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            //{
            //    FileInfo info = new FileInfo(fileName);
            //    Assembly.LoadFile(info.FullName);
            //}

            //HashSet<string> referencedAssembliesFullNames = new HashSet<string>();

            //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    try
            //    {
            //        if (assembly.GetName().Name.StartsWith(DEVPROJECT_ASSEMBLY_PREFIX) && assembly.GetName().Name.EndsWith(DEVPROJECT_ASSEMBLY_SUFFIX))
            //            continue;

            //        if (!string.IsNullOrEmpty(assembly.Location) && !expiredAssemblies.Contains(assembly.FullName) && !referencedAssembliesFullNames.Contains(assembly.FullName))
            //        {
            //            references.Add(MetadataReference.CreateFromFile(assembly.Location));
            //            referencedAssembliesFullNames.Add(assembly.FullName);
            //        }
            //    }
            //    catch (NotSupportedException ex)
            //    {
            //    }
            //}

            //var compiledAssemblyManager = BusinessManagerFactory.GetManager<IVRCompiledAssemblyManager>();
            //var compiledAssemblies = compiledAssemblyManager.GetAssembliesEffectiveForDevProjects();

            //if (compiledAssemblies != null)
            //{
            //    foreach (var compiledAssembly in compiledAssemblies)
            //    {
            //        if (excludedDevProjectIdsFromReferences != null && excludedDevProjectIdsFromReferences.Contains(compiledAssembly.DevProjectId))
            //            continue;

            //        references.Add(MetadataReference.CreateFromStream(new MemoryStream(compiledAssembly.AssemblyContent)));
            //        referencedAssembliesFullNames.Add(compiledAssembly.Name);
            //    }
            //}

            //CSharpCompilation compilation = CSharpCompilation.Create(
            //    assemblyName,
            //    syntaxTrees: syntaxTrees,
            //    references: references,
            //    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            //bool outputResult;
            //using (var ms = new MemoryStream())
            //{
            //    var pdbMs = new MemoryStream();
            //    EmitResult result = compilation.Emit(ms, pdbMs);

            //    if (!result.Success)
            //    {
            //        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
            //            diagnostic.IsWarningAsError ||
            //            diagnostic.Severity == DiagnosticSeverity.Error);

            //        output = new CSharpCompilationOutput() { Errors = new List<CSharpCompilationError>(), ErrorMessages = new List<string>() };
            //        foreach (Diagnostic diagnostic in failures)
            //        {
            //            CSharpCompilationError error = BuildCSharpCompilationError(diagnostic, outputFileName);
            //            output.ErrorMessages.Add(diagnostic.ToString());
            //            output.Errors.Add(error);
            //        }
            //        outputResult = false;
            //    }
            //    else
            //    {
            //        ms.Seek(0, SeekOrigin.Begin);

            //        byte[] byteArray = ms.ToArray();

            //        string formatFileName = string.Format("{0}.dll", assemblyName);
            //        string fullPath = Path.Combine(Path.GetTempPath(), formatFileName);
            //        File.WriteAllBytes(fullPath, byteArray);

            //        byte[] pdbByteArray = pdbMs.ToArray();
            //        string formatPdbFileName = string.Format("{0}.pdb", assemblyName);
            //        string pdbFullPath = Path.Combine(Path.GetTempPath(), formatPdbFileName);
            //        File.WriteAllBytes(pdbFullPath, pdbByteArray);

            //        Assembly assembly = Assembly.LoadFrom(fullPath);
            //        s_dynamicAssembliesByName.Add(assemblyName, assembly);

            //        output = new CSharpCompilationOutput() { AssemblyFile = byteArray, OutputAssembly = assembly };
            //        outputResult = true;
            //    }
            //}

            //return outputResult;
        }

        private static CSharpCompilationError BuildCSharpCompilationError(Diagnostic diagnostic, string outputFileName)
        {
            string diagnosticAsString = diagnostic.ToString();
            if (!string.IsNullOrEmpty(outputFileName) && diagnosticAsString.StartsWith(outputFileName))
                diagnosticAsString = diagnosticAsString.Remove(0, outputFileName.Length);

            string[] parts = diagnosticAsString.Split(':');

            string expectedLineNumber = parts[0].Split(',')[0].Replace("(", "");
            int lineNumber;
            int.TryParse(expectedLineNumber, out lineNumber);
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

            if (string.Compare(type.FullName, "System.Void", true) == 0)
                return "void";

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

    public class CSharpCodeFileToCompile
    {
        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class CSharpCodeFileToCompileCollection : Dictionary<string, CSharpCodeFileToCompile>
    {

    }
}
