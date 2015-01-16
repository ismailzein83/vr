using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;

namespace TABS.Addons.PriceListExport
{
    public class CustomRateSheetGenerator
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.Addons.PriceListExport");

        /// <summary>
        /// The definition of a Node Code runner.
        /// </summary>
        public class NoCodeRunner : IRateSheetGenerator
        {
            public static NoCodeRunner Instance = new NoCodeRunner();
            #region IRateSheetGenerator Members

            public byte[] GetPricelistWorkbook(PriceList pricelist, CodeView codeView)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// The Custom Code Runner that does absolutely nothing
        /// </summary>
        public static IRateSheetGenerator None = NoCodeRunner.Instance;

        protected static string GetClassContent(string customCode, out string ClassName)
        {

            ClassName = "CustomRateSheetGenerator_" + Math.Abs(customCode.GetHashCode());

            string @ClassDefinition = (new StringBuilder()).Append(@"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Runtime.InteropServices;
                using xlsgen;
                using System.Drawing;

                namespace TABS.Addons.PriceListExport
                {
                    public class ").Append(ClassName).Append(" : ").Append(typeof(CustomRateSheetGenerator).FullName).Append(", TABS.Addons.PriceListExport.IRateSheetGenerator").Append(@"
                    {                        
                        ").Append(customCode).Append(@"
                    }
                }
                ").ToString();

            return ClassDefinition;
        }

        public static byte[] GetWorkSheet(string customCode, TABS.PriceList pricelist)
        { 
            return GetWorkSheet(customCode, pricelist, (CodeView)((byte)TABS.SystemParameter.CodeView.NumericValue)); 
        }

        public static byte[] GetWorkSheet(string customCode, TABS.PriceList pricelist, CodeView codeView)
        {
            //customCode = WebHelperLibrary.Utility.Decrypt(customCode, TABS.ObjectAssembler.PricelistCodeKey);
            byte[] bytes = null;
            IRateSheetGenerator codeGenerator = NoCodeRunner.Instance;

            string className = null;
            string classDefinition = GetClassContent(customCode, out className);

            try
            {
                Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                providerOptions["CompilerVersion"] = "v3.5";
                Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.IncludeDebugInformation = true;
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll"  );
                parameters.ReferencedAssemblies.Add("System.Web.dll");
                parameters.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Drawing.dll");



                parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
                parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                string refDir = Addons.AddonManager.AddonsLocation + "\\";
                foreach (AssemblyName referenced in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(refDir + referenced.Name + ".dll");
                    if (info.Exists)
                        parameters.ReferencedAssemblies.Add(info.FullName);
                }

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);

                if (results.Errors.Count == 0)
                {
                    Type generated = results.CompiledAssembly.GetType("TABS.Addons.PriceListExport." + className);
                    codeGenerator = (IRateSheetGenerator)generated.GetConstructor(Type.EmptyTypes).Invoke(null);
                    bytes = codeGenerator.GetPricelistWorkbook(pricelist, codeView);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(error.ErrorText);
                    }
                    log.Error(string.Format("ERROR Generating Custom RateSheet Generator For:\n{0}", classDefinition), new Exception(sb.ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("ERROR Generating Custom RateSheet Generator For:\n{0}", classDefinition), ex);
            }

            return bytes;
        }

        public static IRateSheetGenerator getRateSheetGenerator(string customCode)
        {
            IRateSheetGenerator codeGenerator = NoCodeRunner.Instance;

            string className = null;
            string classDefinition = GetClassContent(customCode, out className);

            try
            {
                Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                providerOptions["CompilerVersion"] = "v3.5";
                Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.IncludeDebugInformation = true;
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Web.dll");
                parameters.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Drawing.dll");



                parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
                parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                string refDir = Addons.AddonManager.AddonsLocation + "\\";
                foreach (AssemblyName referenced in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(refDir + referenced.Name + ".dll");
                    if (info.Exists)
                        parameters.ReferencedAssemblies.Add(info.FullName);
                }

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);

                if (results.Errors.Count == 0)
                {
                    Type generated = results.CompiledAssembly.GetType("TABS.Addons.PriceListExport." + className);
                    codeGenerator = (IRateSheetGenerator)generated.GetConstructor(Type.EmptyTypes).Invoke(null);
                    return codeGenerator;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(error.ErrorText);
                    }
                    log.Error(string.Format("ERROR Generating Custom RateSheet Generator For:\n{0}", classDefinition), new Exception(sb.ToString()));
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("ERROR Generating Custom RateSheet Generator For:\n{0}", classDefinition), ex);
                return null;
            }
        }
    }
}
