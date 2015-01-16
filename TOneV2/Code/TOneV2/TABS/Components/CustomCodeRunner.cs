using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

namespace TABS.Components
{
    public abstract class CustomCodeRunner : Extensibility.IRunnable
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.Components.CustomCodeRunner");

        /// <summary>
        /// The definition of a Node Code runner.
        /// </summary>
        public class NoCodeRunner : CustomCodeRunner { public static NoCodeRunner Instance = new NoCodeRunner(); }

        /// <summary>
        /// The Custom Code Runner that does absolutely nothing
        /// </summary>
        public static CustomCodeRunner None = NoCodeRunner.Instance;

        protected bool _IsRunning = false;
        protected bool _IsStopRequested = false;
        protected DateTime? _LastRun = null;
        protected TimeSpan? _LastRunDuration = null;
        protected bool? _IsLastRunSuccessful;
        protected Exception _Exception;

        #region IRunnable Members

        public virtual void Run()
        {
            // Do nothing
            _LastRun = DateTime.Now;
            _LastRunDuration = TimeSpan.MinValue;
        }

        public virtual bool Stop()
        {
            if (IsRunning)
                _IsStopRequested = true;
            else
                return false;
            return true;
        }


        public virtual bool Abort()
        {
            if (IsRunning && IsStopRequested)
            {
                _IsStopRequested = false;
                _IsRunning = false;
                return true;
            }
            else
                return false;
        }

        public virtual bool IsRunning
        {
            get { return _IsRunning; }
        }

        public virtual bool? IsLastRunSuccessful
        {
            get { return _IsLastRunSuccessful; }
            set { _IsLastRunSuccessful = value; }
        }

        public Exception Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }

        public virtual DateTime? LastRun
        {
            get { return _LastRun; }
        }

        public virtual TimeSpan? LastRunDuration
        {
            get { return _LastRunDuration; }
        }

        public virtual bool IsStopRequested
        {
            get { return _IsStopRequested; }
        }

        public string Status { get; set; }

        #endregion

        protected static string GetClassDefinition(string customCode, out string ClassName)
        {
            return GetClassDefinition(customCode, string.Empty, out ClassName);
        }

        /// <summary>
        /// Get the class definition of a custom code...
        /// </summary>
        /// <param name="customCode">The custom code to run</param>
        /// <param name="ClassName">The class name for the dynamic generated class of the given custom code</param>
        /// <returns></returns>
        protected static string GetClassDefinition(string customCode, string classDefinitions, out string ClassName)
        {
            ClassName = "CustomCodeRunner_" + Math.Abs(customCode.GetHashCode());

            string @ClassDefinition = (new StringBuilder()).Append(@"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Xml.Linq;
                using System.Data;
                using System.Runtime.InteropServices;
                using System.Web.Security;
                /* using TABS;
                using TABS.Addons.Runnables;
                using TABS.SpecialSystemParameters;
  

                using TABS.Components;*/
                // Not feasable to add reference to above namespaces

                namespace CustomCodeRunners
                {
                    public class ").Append(ClassName).Append(" : ").Append(typeof(CustomCodeRunner).FullName).Append(@"
                    {    
                        ").Append(classDefinitions).Append(
                        @"                    
                        public override void Run()
                        {
                            _IsRunning = true;
                            _IsStopRequested = false;
                            DateTime _start = DateTime.Now;
                            
                            // The custom Code goes here.
                            ").Append(customCode).Append(@"
                            
                            _IsStopRequested = false;
                            _IsRunning = false;
                            _LastRunDuration = DateTime.Now.Subtract(_start);
                        }
                    }
                }
                ").ToString();

            return ClassDefinition;
        }

        public static CustomCodeRunner Create(string customCode)
        {
            return CustomCodeRunner.Create(customCode, string.Empty);
        }

        public static CustomCodeRunner Create(string customCode, string classDefinitions)
        {
            CustomCodeRunner codeRunner = NoCodeRunner.Instance;

            string className = null;
            string classDefinition = GetClassDefinition(customCode, classDefinitions, out className);

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
                parameters.ReferencedAssemblies.Add("System.Xml.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                parameters.ReferencedAssemblies.Add("System.Web.Services.dll");
                parameters.ReferencedAssemblies.Add("System.ServiceProcess.dll");
                //parameters.ReferencedAssemblies.Add("System.Runtime.InteropServices.dll");
                //parameters.ReferencedAssemblies.Add("System.Runtime.InteropServices");
                //parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
                //parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                string refDir = Addons.AddonManager.AddonsLocation + "\\";
                foreach (string filename in System.IO.Directory.GetFiles(refDir, "*.dll"))
                {
                    if (filename.Contains("xlsgen"))
                        continue;//Added by siraj need to double check

                    System.IO.FileInfo info = new System.IO.FileInfo(filename);
                    if (info.Exists)
                    {
                        try
                        {
                            parameters.ReferencedAssemblies.Add(info.FullName);
                            log.InfoFormat("Custom Code Runner {0} Referenced Assembly: {1}", className, info.Name);
                        }
                        catch
                        {
                            log.InfoFormat("Custom Code Runner {0} Could Not Reference Assembly: {1}", className, info.Name);
                        }
                    }
                }
                foreach (Assembly referenced in TABS.Plugins.Framework.PluginManager.PluginAssemblies)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(refDir + referenced.GetName().Name + ".dll");
                    if (info.Exists)
                        parameters.ReferencedAssemblies.Add(info.FullName);
                }


                CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);

                if (results.Errors.Count == 0)
                {
                    Type generated = results.CompiledAssembly.GetType("CustomCodeRunners." + className);
                    codeRunner = (CustomCodeRunner)generated.GetConstructor(Type.EmptyTypes).Invoke(null);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(error.ErrorText);
                    }
                    log.Error(string.Format("ERROR Generating Custom Code For:\n{0}", classDefinition), new Exception(sb.ToString()));
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("ERROR Generating Custom Code For:\n{0}", classDefinition), ex);
            }
            return codeRunner;
        }
    }
}