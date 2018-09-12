using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPAPIDiscovery : VRAPIDiscovery
    {
        public override bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.BusinessProcess.Business.BPDefinitionManager.BPDefinitionWorkflowCacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override System.Collections.Generic.List<string> GetModuleNames(IVRAPIDiscoveryGetModuleNamesContext context)
        {
            return new System.Collections.Generic.List<string> { "DynamicBusinessProcess_BP" };
        }

        public override List<Type> GetControllerTypes(IVRAPIDiscoveryGetControllerTypesContext context)
        {
            List<Type> controllerTypes = new List<Type>();

            IEnumerable<BPDefinition> bpDefinitions = new BPDefinitionManager().GetBPDefinitions();
            foreach (var bpDefinition in bpDefinitions)
            {
                if (!bpDefinition.VRWorkflowId.HasValue)
                    continue;

                controllerTypes.Add(GetBPControllerType(bpDefinition));
            }

            return controllerTypes;
        }

        private Type GetBPControllerType(BPDefinition bpDefinition)
        {
            StringBuilder sb_CustomController = new StringBuilder(@"using System;
                        using System.Collections.Generic;
                        using System.Web.Http;
                        using Vanrise.Web.Base;
                        using Vanrise.BusinessProcess.Entities;
                        using Vanrise.BusinessProcess.Business;
        
                        namespace #Namespace#
                        {
                            [RoutePrefix(""api/DynamicBusinessProcess_BP/#ControllerName#"")]
                            public class #ControllerFullName# : BaseAPIController
                            {
                                [HttpPost]
                                [Route(""StartProcess"")]
                                public #StartProcessOutputClassName# StartProcess(#StartProcessInputClassName# input)
                                {
                                    #InputArgumentClassName# inputArguments;
                                    if(input != null && input.InputArguments != null)
                                        inputArguments = input.InputArguments;
                                    else
                                        inputArguments = new #InputArgumentClassName#();

                                    inputArguments.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

                                    var startProcessOutput = new #StartProcessOutputClassName#();
                                    CreateProcessInput createProcessInput = new CreateProcessInput() { InputArguments = inputArguments, StartProcessOutput = startProcessOutput };
                                    CreateProcessOutput createProcessOutput = new BPInstanceManager().CreateNewProcess(createProcessInput, true);

                                    startProcessOutput.ProcessId = createProcessOutput.ProcessInstanceId;
                                    return startProcessOutput;
                                }
                            }

                            public class #StartProcessOutputClassName#
                            {
                                public long ProcessId { get; set; }

                                #BPInstanceInsertHandlerCode#
                            }

                            public class #StartProcessInputClassName#
                            {
                                public #InputArgumentClassName# InputArguments { get; set; }
                            }

                            #InputArgumentClass#
                        }");


            string inputArgumentClassName;
            string inputArgumentClass = Helper.GetInputArgumentClassCode(bpDefinition, out inputArgumentClassName);

            string bpDefinitionTitle = bpDefinition.Title.Replace(" ", "");
            string controllerName = bpDefinitionTitle;
            string controllerFullName = string.Concat(controllerName, "Controller");
            string controllerNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.BusinessProcess.Web.DynamicControllers");
            string startProcessInputClassName = string.Concat("Start", bpDefinitionTitle, "Input");
            string startProcessOutputClassName = string.Concat("Start", bpDefinitionTitle, "Output");

            string bpInstanceInsertHandlerCode = null;
            if (bpDefinition.Configuration != null && bpDefinition.Configuration.BPInstanceInsertHandler != null)
            {
                var bpInstanceHandlerBeforeAPICompileContext = new BPInstanceHandlerBeforeAPICompileContext();
                bpDefinition.Configuration.BPInstanceInsertHandler.BeforeAPICompile(bpInstanceHandlerBeforeAPICompileContext);
                bpInstanceInsertHandlerCode = bpInstanceHandlerBeforeAPICompileContext.OutputArgumentCode;
            }
            bpInstanceInsertHandlerCode = !string.IsNullOrEmpty(bpInstanceInsertHandlerCode) ? bpInstanceInsertHandlerCode : string.Empty;

            sb_CustomController.Replace("#Namespace#", controllerNamespace);
            sb_CustomController.Replace("#ControllerName#", controllerName);
            sb_CustomController.Replace("#ControllerFullName#", controllerFullName);
            sb_CustomController.Replace("#InputArgumentClass#", inputArgumentClass);
            sb_CustomController.Replace("#InputArgumentClassName#", inputArgumentClassName);
            sb_CustomController.Replace("#StartProcessInputClassName#", startProcessInputClassName);
            sb_CustomController.Replace("#StartProcessOutputClassName#", startProcessOutputClassName);
            sb_CustomController.Replace("#BPInstanceInsertHandlerCode#", bpInstanceInsertHandlerCode);

            Common.CSharpCompilationOutput output;
            Common.CSharpCompiler.TryCompileClass(sb_CustomController.ToString(), out output);
            return output.OutputAssembly.GetType(string.Format("{0}.{1}", controllerNamespace, controllerFullName));
        }
    }
}